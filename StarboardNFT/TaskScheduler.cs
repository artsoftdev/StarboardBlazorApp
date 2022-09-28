using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarboardNFT.Data;
using StarboardNFT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using StarboardNFT.Hubs;
using StarboardNFT.Interface;
using StarboardNFT.Engines;
using StarboardNFT.Utilities;

namespace StarboardNFT
{
    public class TaskScheduler : IHostedService, IDisposable
    {
        private readonly ILogger<TaskScheduler> logger;
        private Timer timer;
        private Timer AuctionStartTimer;
        private Timer AuctionEngineTimer;
        private Timer AuctionEndTimer;
        private IHostEnvironment _hostEnv;
        private readonly IServiceScopeFactory _scopeFactory;
        private ApplicationDbContext dBContext;
        private readonly ProfileService _profileService;

        private IHubContext<NotificationHub> _hubContext;
        private readonly IUserConnectionManager _userConnectionManager;

        public TaskScheduler(ILogger<TaskScheduler> logger,
            IHostEnvironment hostEnv,
            IServiceScopeFactory scopeFactory,
            IHubContext<NotificationHub> hubContext,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            _hostEnv = hostEnv;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _profileService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProfileService>();
            _userConnectionManager = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUserConnectionManager>();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            logger.LogInformation("Timer Started");

            //1st Timer - 30 second intervals
            timer = new Timer(o => {
                //logger.LogInformation("Timer Started");
                //Debug.WriteLine("Timer Started");
                //This will not run in development.
                if (!_hostEnv.IsDevelopment())
                {
                    //CoinMarketCapAPI();
                    CryptoCompareAPI();
                }
                else
                {
                    Startup.CoinPriceDict["ETH"] = 1000.0M;
                }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(30));

            //2nd Timer - 10 second intervals 
            AuctionStartTimer = new Timer(o => {
                
                
                AuctionTimerCheck();
                
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(10));

            //3rd Timer - 200 millisecond intervals 
            AuctionEngineTimer = new Timer(o => {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    AuctionEngine auctionEngine = new AuctionEngine(_hostEnv, _scopeFactory, _hubContext, dbContext);
                    auctionEngine.PopulateAuctionBidQueue();
                }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(200));

            //2nd Timer - 10 second intervals 
            AuctionEndTimer = new Timer(o => {


                AuctionEndTimerCheck();

            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(700));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timer Stopped...");

            return Task.CompletedTask;
        }

        private async void CryptoCompareAPI()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("https://min-api.cryptocompare.com/data/price?fsym=ETH&tsyms=USD&api_key=ff1a1c431fe4b4d291dbe0d34720c5086b5b3791e2632499966ad62734b50a44");
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        var jData = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(data);

                        var price = jData["USD"];

                        Startup.CoinPriceDict["ETH"] = price;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                //something failed
            }
        }

        private void CreateDBContext()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dBContext = dbContext;

            }

        }

        private async void AuctionTimerCheck()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var utcDateTimeNow = DateTime.UtcNow.AddMinutes(5);
                var fiveMinAuction = await dbContext.Auction.Where(x => x.IsFiveMinuteNotify == false && x.IsAuctionOver == false && x.NFTData.SaleEndtDate.Value < DateTime.UtcNow.AddMinutes(5) && x.Active == true)
                    .Include(x => x.NFTData.NFT).Include(x => x.NFTData.NFT.UserProfile)
                    .FirstOrDefaultAsync();

                if (fiveMinAuction != null)
                {
                    fiveMinAuction.IsFiveMinuteNotify = true;
                    var SpecificAuctionBiduserList = await dbContext.AuctionBid.Where(x => x.AuctionId == fiveMinAuction.Id).Select(x => x.BidUserId).Distinct().ToListAsync();
                    SpecificAuctionBiduserList.Add(fiveMinAuction.NFTData.NFT.UserProfile.UserProfileHeaderId);

                    foreach (var user in SpecificAuctionBiduserList)
                    {
                        Notification notification = new Notification();
                        notification.Title = "End auction";
                        notification.Description = fiveMinAuction.NFTData.NFT.Title + " sale is ended soon!";
                        notification.NFTDataId = fiveMinAuction.NFTDataId;
                        //notification.NFTData = auction;
                        notification.UserProfileHeaderId = user;

                        await dbContext.Notification.AddAsync(notification);

                        string messageJsonString = JsonConvert.SerializeObject(notification, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        var connections = _userConnectionManager.GetUserConnections(user.ToString());

                        var connectedUser = await dbContext.UserProfileHeader.Where(x => x.Id == user && x.Active == true).FirstOrDefaultAsync();

                        if (connections != null && connections.Count > 0)
                        {
                            foreach (var connectionId in connections)
                            {
                                await _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", messageJsonString);
                            }
                        }

                        await dbContext.SaveChangesAsync();
                        EmailUtility.SendEmail(connectedUser.Email, "Starboard - End Auction soon", notification.Description, "alert", true, "End Auction", "https://nft.starboard.org/placebid/" + fiveMinAuction.Id);
                    }
                }

                var NotStartedAuctionList = await dbContext.NFTData.Where(x => x.IsSaleStarted == false && x.SaleStartDate != null && x.SaleStartDate.Value < DateTime.UtcNow)
                .Include(x => x.NFT).Include(x => x.NFT.UserProfile)
                .AsNoTracking()
                .ToListAsync();

                foreach (var auction in NotStartedAuctionList)
                {
                    // Notification for Auction sale started
                    var startedAuction = await dbContext.NFTData.Where(x => x.Id == auction.Id).Include(x => x.NFT.UserProfile).FirstAsync();
                    startedAuction.IsSaleStarted = true;
                    await dbContext.SaveChangesAsync();

                    AuctionEngine auctionEngine = new AuctionEngine(_hostEnv, _scopeFactory, _hubContext, dbContext);

                    Auction newAuction = new Auction();
                    newAuction.CurrentBidPrice = auction.FiatStartPrice;
                    newAuction.MaxBidPrice = auction.FiatStartPrice;
                    newAuction.IncrementAmount = auctionEngine.GetIncrementBidAmount(auction.FiatStartPrice, auction.Id);
                    newAuction.NFTDataId = auction.Id;
                    newAuction.CurrentWinningUserId = auction.NFT.UserProfile.UserProfileHeaderId;

                    await dbContext.Auction.AddAsync(newAuction);

                    Notification notification = new Notification();
                    notification.Title = "New Sale Started";
                    notification.Description = auction.NFT.Title + " sale is started";
                    notification.NFTDataId = auction.Id;
                    //notification.NFTData = auction;
                    notification.UserProfileHeaderId = auction.NFT.UserProfile.UserProfileHeaderId;

                    await dbContext.Notification.AddAsync(notification);

                    //add record in activity table
                    Activity activity = new Activity();
                    activity.Title = "New Sale Started";
                    activity.Description = auction.NFT.Title + " sale is started";
                    activity.NFTDataId = auction.Id;
                    activity.Category = Activity.Categories.MyActivity;
                    activity.filterCategory = Activity.FilterCategories.Sales;
                    activity.UserProfileHeaderId = auction.NFT.UserProfile.UserProfileHeaderId;

                    await dbContext.Activity.AddAsync(activity);

                    //add record in activity table as following
                    var followers = await dbContext.UserFollowing.Where(x => x.FollowingUserProfileHeaderId == auction.NFT.UserProfile.UserProfileHeaderId).ToListAsync();
                    foreach (var follower in followers)
                    {
                        //add record in activity table as following
                        activity = new Activity();
                        activity.Title = "New Sale Started";
                        activity.Description = auction.NFT.Title + " sale is started";
                        activity.NFTDataId = auction.Id;
                        activity.Category = Activity.Categories.Following;

                        var UserProfileHeader = await dbContext.UserProfileHeader.Where(x => x.Id == follower.MainUserProfileHeaderId).FirstOrDefaultAsync();
                        activity.UserProfileHeaderId = UserProfileHeader.Id;

                        await dbContext.Activity.AddAsync(activity);
                    }

                    string messageJsonString = JsonConvert.SerializeObject(notification, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    var connectedUser = await dbContext.UserProfileHeader.Where(x => x.Id == notification.UserProfileHeaderId && x.Active == true).FirstOrDefaultAsync();

                    var connections = _userConnectionManager.GetUserConnections(notification.NFTData.NFT.UserProfile.UserProfileHeaderId.ToString());

                    if (connections != null && connections.Count > 0)
                    {
                        foreach (var connectionId in connections)
                        {
                            await _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", messageJsonString);
                        }
                    }
                    await dbContext.SaveChangesAsync();
                    EmailUtility.SendEmail(connectedUser.Email, "Starboard - New Sale Started", notification.Description, "alert", true, "New Sale Started", "https://nft.starboard.org/placebid/" + newAuction.Id);
                }

                if (NotStartedAuctionList.Count() > 0)
                {
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async void AuctionEndTimerCheck()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var endedAuction = await dbContext.Auction.Where(x => x.IsAuctionOver == false && x.NFTData.SaleEndtDate != null && x.NFTData.SaleEndtDate.Value < DateTime.UtcNow && x.NFTData.IsSaleStarted == true && x.Active == true)
                        .Include(x => x.NFTData.NFT).Include(x => x.NFTData.NFT.UserProfile)
                        .FirstOrDefaultAsync();

                if (endedAuction != null)
                {
                    var SpecificAuctionBiduserList = await dbContext.AuctionBid.Where(x => x.AuctionId == endedAuction.Id).Select(x => x.BidUserId).Distinct().ToListAsync();
                    SpecificAuctionBiduserList.Add(endedAuction.NFTData.NFT.UserProfile.UserProfileHeaderId);

                    foreach (var user in SpecificAuctionBiduserList)
                    {
                        Notification notification = new Notification();
                        notification.Title = "End auction";
                        notification.Description = endedAuction.NFTData.NFT.Title + " auction is over!";
                        notification.NFTDataId = endedAuction.NFTDataId;
                        //notification.NFTData = auction;
                        notification.UserProfileHeaderId = user;

                        await dbContext.Notification.AddAsync(notification);

                        //add record in activity table
                        Activity activity = new Activity();
                        activity.Title = "End auction";
                        activity.Description = endedAuction.NFTData.NFT.Title + " auction is over!";
                        activity.NFTDataId = endedAuction.NFTDataId;
                        activity.Category = Activity.Categories.MyActivity;
                        activity.filterCategory = Activity.FilterCategories.Sales;
                        activity.UserProfileHeaderId = user;

                        await dbContext.Activity.AddAsync(activity);

                        var followers = await dbContext.UserFollowing.Where(x => x.FollowingUserProfileHeaderId == user).ToListAsync();
                        foreach (var follower in followers)
                        {
                            //add record in activity table as following
                            activity = new Activity();
                            activity.Title = "End auction";
                            activity.Description = endedAuction.NFTData.NFT.Title + " auction is over!";
                            activity.NFTDataId = endedAuction.NFTDataId;
                            activity.Category = Activity.Categories.Following;

                            var UserProfileHeader = await dbContext.UserProfileHeader.Where(x => x.Id == follower.MainUserProfileHeaderId).FirstOrDefaultAsync();
                            activity.UserProfileHeaderId = UserProfileHeader.Id;

                            await dbContext.Activity.AddAsync(activity);
                        }

                        string messageJsonString = JsonConvert.SerializeObject(notification, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        var connections = _userConnectionManager.GetUserConnections(user.ToString());

                        var connectedUser = await dbContext.UserProfileHeader.Where(x => x.Id == user && x.Active == true).FirstOrDefaultAsync();

                        if (connections != null && connections.Count > 0)
                        {
                            foreach (var connectionId in connections)
                            {
                                await _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", messageJsonString);
                            }
                        }
                        endedAuction.IsAuctionOver = true;
                        endedAuction.NFTData.IsSaleEnded = true;

                        // register purchase amount
                        endedAuction.NFTData.USDPurchaseAmount = endedAuction.CurrentBidPrice;
                        endedAuction.NFTData.EthPurchaseAmount = endedAuction.CurrentBidPrice / Startup.CoinPriceDict["ETH"];
                        endedAuction.NFTData.EthPurchaseAddress = endedAuction.CurrentWinningUserId.ToString();

                        await dbContext.SaveChangesAsync();

                        //add record in activity table
                        activity = new Activity();
                        activity.Title = "Successful Purchase of an NFT";
                        activity.Description = endedAuction.NFTData.NFT.Title + " is purchased by you successfully.";
                        activity.NFTDataId = endedAuction.NFTDataId;
                        activity.Category = Activity.Categories.MyActivity;
                        activity.filterCategory = Activity.FilterCategories.Purchase;
                        activity.UserProfileHeaderId = endedAuction.CurrentWinningUserId;

                        await dbContext.Activity.AddAsync(activity);

                        EmailUtility.SendEmail(connectedUser.Email, "Starboard - Sale is ended", notification.Description, "alert", true, "Auction is time out", "https://nft.starboard.org/placebid/" + endedAuction.Id);
                    }
                }
            }
        }
    }
}
