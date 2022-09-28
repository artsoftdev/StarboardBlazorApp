using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarboardNFT.Data;
using StarboardNFT.Hubs;
using StarboardNFT.Interface;
using StarboardNFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using StarboardNFT.Utilities;

namespace StarboardNFT.Engines
{
    public class AuctionEngine
    {
        #region Public and Private Variables
        public static bool AListFirstRun = false;
        public static bool QueueProcessing = false;
        public Queue<AuctionBidQueue> AuctionBidQueueList = new Queue<AuctionBidQueue>();
        public static List<Auction> AuctionLiveList = new List<Auction>();
        //public static NFTService nftService;

        private IHostEnvironment _hostEnv;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHubContext<NotificationHub> _hubContext;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ApplicationDbContext _context;
        private readonly ProfileService _profileService;

        public  AuctionEngine(
            IHostEnvironment hostEnv,
            IServiceScopeFactory scopeFactory,
            IHubContext<NotificationHub> hubContext,
            ApplicationDbContext context)
        {
            _hostEnv = hostEnv;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _profileService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProfileService>();
            _userConnectionManager = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUserConnectionManager>();
            _context = context;
        }

        #endregion

        public void PopulateAuctionBidQueue()
        {
            if (AListFirstRun == false)
            {
                //AuctionLiveListPopulate();
                AListFirstRun = true;
            }
            //Can only add auctions from AuctionBidQueue table if queue is not processing. 
            if(QueueProcessing == false)
            {
                //Lock the Queue
                LockAuctionBidQueue();
                //add AuctionBidQueue to AuctionBidQueueList
                var auctionBigQueue = _context.AuctionBidQueue.Where(x => x.Active == true && x.IsProcessed == false).OrderBy(x => x.CreateTimeInTicks).ToList();

                if(auctionBigQueue.Count != 0)
                {
                    foreach (AuctionBidQueue abq in auctionBigQueue)
                    {
                        AuctionBidQueueList.Enqueue(abq);
                    }

                    //Process the bid  Queue
                    ProcessAuctionBidQueue();
                    //Clear the Auction Bid Queue List
                    ClearAuctionBidQueue();
                    
                }
                //Unlock the queue to be processed again
                UnlockAuctionBidQueue();

            }
        }
        public void ProcessAuctionBidQueue()
        {
            //During process no new bids can be added to queue. They will await in the table for once this is done. 
            foreach (AuctionBidQueue auctionBQ in AuctionBidQueueList)
            {
                //Process the Bid
                //var auction = _context.Auction.Where(x => x.Id == auctionBQ.AuctionId).FirstOrDefault();
                Auction auctionRecord = _context.Auction.Where(x => x.Id == auctionBQ.AuctionId)
                    .Include(x => x.NFTData)
                    .Include(x => x.NFTData.NFT)
                    .Include(x => x.NFTData.NFT.UserProfile)
                    .FirstOrDefault();

                if (auctionBQ.IsBuyItNow == true || auctionRecord.NFTData.FiatBuyOutPrice <= auctionBQ.FiatBidAmount)
                {
                    if (auctionRecord.IsAuctionOver == false)
                    {
                        auctionRecord.IsAuctionOver = true;
                        auctionRecord.MaxBidPrice = auctionBQ.MaxBidAmount;
                        auctionRecord.CurrentBidPrice = auctionBQ.MaxBidAmount;

                        auctionRecord.NFTData.IsSaleEnded = true;
                        auctionRecord.NFTData.SalePurchaseDate = DateTime.UtcNow.AddSeconds(1);
                        auctionRecord.NFTData.USDPurchaseAmount = auctionBQ.MaxBidAmount;

                        // register purchase amount
                        auctionRecord.NFTData.USDPurchaseAmount = auctionRecord.CurrentBidPrice;
                        auctionRecord.NFTData.EthPurchaseAmount = auctionRecord.CurrentBidPrice / Startup.CoinPriceDict["ETH"];
                        auctionRecord.NFTData.EthPurchaseAddress = auctionRecord.CurrentWinningUserId.ToString();

                        //Remove the Auction Bid Queue record from table so its not processed again
                        _context.AuctionBidQueue.Remove(auctionBQ);

                        //Save both changes
                        _context.SaveChanges();

                        BuyItOut(auctionRecord, auctionBQ);

                        //    var NFTData = await _context.NFTData.Where(x => x.Id == auction.NFTDataId && x.Active == true).FirstOrDefaultAsync();
                    }
                }
                else
                {
                    string bidStatus = "A";
                    //if (auctionRecord.NFTData.HasBuyoutPrice == true)
                    //    bidStatus = "R";
                    if (auctionRecord.CurrentBidPrice > auctionBQ.MaxBidAmount || auctionRecord.IsAuctionOver == true)
                        bidStatus = "R";

                    AuctionBid auctionBid = new AuctionBid();

                    auctionBid.AuctionId = auctionBQ.AuctionId;
                    auctionBid.BidAmount = auctionBQ.BidAmount;
                    auctionBid.BidStatus = bidStatus; //Put an A if the auction is accepted or an R if it is rejected! DO NOT LEAVE BLANK DANIIL
                    auctionBid.BidUserId = auctionBQ.BidUserId;
                    auctionBid.EthBidAmount = auctionBQ.EthBidAmount;
                    auctionBid.EthMaxBidAmount = auctionBQ.EthMaxBidAmount;
                    auctionBid.FiatBidAmount = auctionBQ.FiatBidAmount;
                    auctionBid.FiatMaxBidAmount = auctionBQ.FiatMaxBidAmount;
                    auctionBid.IsAutoBid = auctionBQ.IsAutoBid;
                    auctionBid.IsBuyItNow = auctionBQ.IsBuyItNow;
                    auctionBid.MaxBidAmount = auctionBQ.MaxBidAmount;

                    if (auctionRecord.NFTData.NFT.UserProfile.UserProfileHeaderId == auctionBQ.BidUserId)
                        bidStatus = "R";

                    //auctionBid.FiatBidAmount = auctionRecord.CurrentBidPrice + auctionRecord.IncrementAmount;

                    if (bidStatus == "A")
                    {
                        //Add the Auction Bid to the table
                        _context.AuctionBid.Add(auctionBid);

                        //add record in activity table
                        Activity activity = new Activity();
                        activity.Title = "New Bid Received";
                        activity.Description = auctionRecord.NFTData.NFT.Title + "  received new bid.";
                        activity.NFTDataId = auctionRecord.NFTDataId;
                        activity.UserProfileHeaderId = auctionBid.BidUserId;

                        _context.Activity.Add(activity);

                        decimal incrementBidAmount = GetIncrementBidAmount(auctionBQ.BidAmount, auctionRecord.Id);

                        //Auto bid(Someone place a bid, but if there is someone have max bid price higher than new bidder's price, aucto bid system works)
                        if (auctionBid.FiatMaxBidAmount <= auctionRecord.MaxBidPrice)
                        {
                            if (auctionBid.FiatMaxBidAmount != auctionRecord.MaxBidPrice)
                            {

                                AuctionBid autoAuctionBid = new AuctionBid();

                                var ethPrice = Startup.CoinPriceDict["ETH"];

                                autoAuctionBid.AuctionId = auctionRecord.Id;
                                autoAuctionBid.BidAmount = auctionBid.FiatMaxBidAmount + incrementBidAmount;
                                autoAuctionBid.BidStatus = bidStatus; //Put an A if the auction is accepted or an R if it is rejected! DO NOT LEAVE BLANK DANIIL
                                autoAuctionBid.BidUserId = auctionRecord.CurrentWinningUserId;
                                autoAuctionBid.EthBidAmount = auctionRecord.CurrentBidPrice / ethPrice;
                                autoAuctionBid.EthMaxBidAmount = auctionRecord.MaxBidPrice / ethPrice;
                                autoAuctionBid.FiatBidAmount = auctionBid.FiatMaxBidAmount + incrementBidAmount;
                                autoAuctionBid.FiatMaxBidAmount = auctionRecord.MaxBidPrice;
                                autoAuctionBid.IsAutoBid = true;
                                autoAuctionBid.IsBuyItNow = false;
                                autoAuctionBid.MaxBidAmount = auctionRecord.MaxBidPrice;

                                _context.AuctionBid.Add(autoAuctionBid);

                                //add record in activity table
                                activity.Title = "New Bid Received";
                                activity.Description = auctionRecord.NFTData.NFT.Title + "  received new bid.";
                                activity.NFTDataId = auctionRecord.NFTDataId;
                                activity.UserProfileHeaderId = auctionBid.BidUserId;

                                _context.Activity.Add(activity);

                                auctionRecord.CurrentBidPrice = auctionBid.FiatMaxBidAmount + incrementBidAmount;
                            }
                            else
                            {
                                auctionRecord.CurrentBidPrice = auctionBid.FiatMaxBidAmount;
                            }

                            Notification outBidNotification = new Notification();
                            outBidNotification.Title = "Out Bid";
                            outBidNotification.Description = "You are out bid";
                            outBidNotification.NFTDataId = auctionRecord.NFTDataId;
                            outBidNotification.NFTData = auctionRecord.NFTData;
                            //Aaron Changes this
                            outBidNotification.UserProfileHeaderId = auctionBid.BidUserId;

                            _context.Notification.Add(outBidNotification);

                            string outBidMessageJsonString = JsonConvert.SerializeObject(outBidNotification, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });

                            var outBidConnections = _userConnectionManager.GetUserConnections(outBidNotification.UserProfileHeaderId.ToString());
                            var connectedUser = _profileService.GetProfileAsync(outBidNotification.UserProfileHeaderId).Result;

                            if (outBidConnections != null && outBidConnections.Count > 0)
                            {
                                foreach (var connectionId in outBidConnections)
                                {
                                    _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", outBidMessageJsonString);
                                }
                            }

                            EmailUtility.SendEmail(connectedUser.Email, "Starboard - Out Bid", outBidNotification.Description, "alert", true, "Out Bid", "https://nft.starboard.org/placebid/" + auctionRecord.Id);
                        }

                        //This case is if bidder's new max bid price is higher than current auction bid price
                        else
                        {
                            bool bSameBidUser = false;
                            if (auctionBid.BidUserId == auctionRecord.CurrentWinningUserId)
                                bSameBidUser = true;

                            //If bidder is not current winning bid.
                            if (bSameBidUser == false)
                            {
                                if (auctionRecord.CurrentWinningUserId != auctionRecord.NFTData.NFT.UserProfile.UserProfileHeaderId)
                                {
                                    Notification outBidNotification = new Notification();
                                    outBidNotification.Title = "Out Bid";
                                    outBidNotification.Description = "You are out bid";
                                    outBidNotification.NFTDataId = auctionRecord.NFTDataId;
                                    outBidNotification.NFTData = auctionRecord.NFTData;
                                    //Aaron Changes this
                                    outBidNotification.UserProfileHeaderId = auctionRecord.CurrentWinningUserId;

                                    _context.Notification.Add(outBidNotification);

                                    string outBidMessageJsonString = JsonConvert.SerializeObject(outBidNotification, Formatting.Indented,
                                        new JsonSerializerSettings
                                        {
                                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                        });

                                    var outBidConnections = _userConnectionManager.GetUserConnections(outBidNotification.UserProfileHeaderId.ToString());
                                    var outBidConnectedUser = _profileService.GetProfileAsync(outBidNotification.UserProfileHeaderId).Result;

                                    if (outBidConnections != null && outBidConnections.Count > 0)
                                    {
                                        foreach (var connectionId in outBidConnections)
                                        {
                                            _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", outBidMessageJsonString);
                                        }
                                    }

                                    EmailUtility.SendEmail(outBidConnectedUser.Email, "Starboard - Out Bid", outBidNotification.Description, "alert", true, "Out Bid", "https://nft.starboard.org/placebid/" + auctionRecord.Id);
                                }

                                Notification winningNotification = new Notification();
                                winningNotification.Title = "Winning Bid";
                                winningNotification.Description = "You are on winning bid";
                                winningNotification.NFTDataId = auctionRecord.NFTDataId;
                                winningNotification.NFTData = auctionRecord.NFTData;
                                winningNotification.UserProfileHeaderId = auctionBid.BidUserId;

                                _context.Notification.AddAsync(winningNotification);

                                string messageJsonString1 = JsonConvert.SerializeObject(winningNotification, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                    });

                                var connections = _userConnectionManager.GetUserConnections(winningNotification.UserProfileHeaderId.ToString());
                                var connectedUser = _profileService.GetProfileAsync(winningNotification.UserProfileHeaderId).Result;

                                if (connections != null && connections.Count > 0)
                                {
                                    foreach (var connectionId in connections)
                                    {
                                        _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", messageJsonString1);
                                    }
                                }

                                EmailUtility.SendEmail(connectedUser.Email, "Starboard - Winning Bid", winningNotification.Description, "alert", true, "Winning Bid", "https://nft.starboard.org/placebid/" + auctionRecord.Id);

                                auctionBid.FiatBidAmount = auctionRecord.CurrentBidPrice + incrementBidAmount;
                            }

                            //If bidder is not current winning bid.
                            else
                            {
                                auctionBid.FiatBidAmount = auctionRecord.CurrentBidPrice;
                            }

                            auctionRecord.CurrentWinningUserId = auctionBid.BidUserId;
                            auctionRecord.CurrentBidPrice = auctionBid.FiatBidAmount;
                            auctionRecord.MaxBidPrice = auctionBid.FiatMaxBidAmount;

                            if (IsFirstBid(auctionBid.AuctionId) == true)
                            {
                                Notification firstBidNotification = new Notification();
                                firstBidNotification.Title = "First Bid";
                                firstBidNotification.Description = "First Bid for your NFT";
                                firstBidNotification.NFTDataId = auctionRecord.NFTDataId;
                                firstBidNotification.NFTData = auctionRecord.NFTData;
                                firstBidNotification.UserProfileHeaderId = auctionRecord.NFTData.NFT.UserProfile.UserProfileHeaderId;

                                _context.Notification.AddAsync(firstBidNotification);

                                string messageJsonString1 = JsonConvert.SerializeObject(firstBidNotification, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                    });

                                var connections = _userConnectionManager.GetUserConnections(firstBidNotification.UserProfileHeaderId.ToString());
                                var connectedUser = _profileService.GetProfileAsync(firstBidNotification.UserProfileHeaderId).Result;

                                if (connections != null && connections.Count > 0)
                                {
                                    foreach (var connectionId in connections)
                                    {
                                        _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", messageJsonString1);
                                    }
                                }

                                EmailUtility.SendEmail(connectedUser.Email, "Starboard - First Bid", firstBidNotification.Description, "alert", true, "First Bid", "https://nft.starboard.org/placebid/" + auctionRecord.Id);
                            }
                        }

                        //Buyout case
                        if (auctionRecord.IsReserveMet == false && auctionRecord.NFTData.FiatReservePrice == auctionBQ.BidAmount)
                        {
                            auctionRecord.IsReserveMet = true;
                        }
                    }

                    //Remove the Auction Bid Queue record from table so its not processed again
                    _context.AuctionBidQueue.Remove(auctionBQ);

                    //Save both changes
                    _context.SaveChanges();

                    var updatePriceConnections = _context.AuctionBid.Where(x => x.AuctionId == auctionRecord.Id).Select(x => x.BidUserId).Distinct().ToList();

                    //var updatePriceConnections = _userConnectionManager.GetUserConnections(firstBidNotification.UserProfileHeaderId.ToString());

                    string messageJsonString = JsonConvert.SerializeObject(auctionRecord, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });

                    _hubContext.Clients.All.SendAsync("UpdateBidPrice", messageJsonString);


                    var AuctionBidList = _context.AuctionBid.Where(x => x.AuctionId == auctionRecord.Id && x.Active == true).ToList();

                    messageJsonString = JsonConvert.SerializeObject(AuctionBidList, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });

                    _hubContext.Clients.All.SendAsync("UpdateAuctionBidList", messageJsonString);
                }
            }
        }

        public void LockAuctionBidQueue()
        {
            //This locks the queue
            QueueProcessing = true;
        }

        public void UnlockAuctionBidQueue()
        {
            //This will unlock the queue
            QueueProcessing = false;
        }

        public void ClearAuctionBidQueue()
        {
            //This will clear ALL elements in the queue. This should only be called once the queue has been successfully processed. 
            AuctionBidQueueList.Clear();
            var processedAuctionBidList = _context.AuctionBidQueue.Where(x => x.Active == true && x.IsProcessed == true).ToList();
            _context.AuctionBidQueue.RemoveRange(processedAuctionBidList);
            _context.SaveChanges();
        }

        public void AuctionLiveListPopulate()
        {
            var auctionList = _context.Auction.Where(x => x.Active == true && x.IsAuctionOver == false).ToList();
            AuctionLiveList.AddRange(auctionList);
        }

        public bool IsFirstBid(Guid auctionId)
        {
            var auctionBidCount = _context.AuctionBid.Where(x => x.AuctionId == auctionId && x.Active == true).Count();

            if (auctionBidCount == 0) return true;
            else return false;
        }

        public void BuyItOut(Auction auctionRecord, AuctionBidQueue auctionBQ)
        {
            Notification buyOutNotification = new Notification();
            buyOutNotification.Title = "Buy out";
            buyOutNotification.Description = "You bought out " + auctionRecord.NFTData.NFT.Title;
            buyOutNotification.NFTDataId = auctionRecord.NFTDataId;
            buyOutNotification.NFTData = auctionRecord.NFTData;
            buyOutNotification.UserProfileHeaderId = auctionBQ.BidUserId;

            _context.Notification.AddAsync(buyOutNotification);

            //To the person bought out

            string outBidMessageJsonString = JsonConvert.SerializeObject(buyOutNotification, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            var outBidConnections = _userConnectionManager.GetUserConnections(buyOutNotification.UserProfileHeaderId.ToString());
            var connectedUser = _profileService.GetProfileAsync(buyOutNotification.UserProfileHeaderId).Result;

            if (outBidConnections != null && outBidConnections.Count > 0)
            {
                foreach (var connectionId in outBidConnections)
                {
                    _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", outBidMessageJsonString);
                    _hubContext.Clients.Client(connectionId).SendAsync("BuyOut", outBidMessageJsonString);
                }
            }

            EmailUtility.SendEmail(connectedUser.Email, "Starboard - Buy out", buyOutNotification.Description, "alert", true, "Out Bid", "https://nft.starboard.org/placebid/" + auctionRecord.Id);

            var otherBidders = _context.AuctionBid.Where(x => x.AuctionId == auctionRecord.Id).Select(x => x.BidUserId).Distinct().ToList();

            // To owner

            buyOutNotification = new Notification();
            buyOutNotification.Title = "Buy out";
            buyOutNotification.Description = "Your NFT " + auctionRecord.NFTData.NFT.Title + " is buyout";
            buyOutNotification.NFTDataId = auctionRecord.NFTDataId;
            buyOutNotification.NFTData = auctionRecord.NFTData;
            buyOutNotification.UserProfileHeaderId = auctionRecord.NFTData.NFT.UserProfile.UserProfileHeaderId;

            _context.Notification.AddAsync(buyOutNotification);

            outBidConnections = _userConnectionManager.GetUserConnections(auctionRecord.NFTData.NFT.UserProfile.UserProfileHeaderId.ToString());
            connectedUser = _profileService.GetProfileAsync(auctionRecord.NFTData.NFT.UserProfile.UserProfileHeaderId).Result;

            if (outBidConnections != null && outBidConnections.Count > 0)
            {
                foreach (var connectionId in outBidConnections)
                {
                    _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", outBidMessageJsonString);
                    _hubContext.Clients.Client(connectionId).SendAsync("BuyOut", outBidMessageJsonString);
                }
            }

            EmailUtility.SendEmail(connectedUser.Email, "Starboard - Buy out", buyOutNotification.Description, "alert", true, "Out Bid", "https://nft.starboard.org/placebid/" + auctionRecord.Id);

            //To other bidders

            foreach (var bidderUserId in otherBidders)
            {
                if (bidderUserId == auctionBQ.BidUserId)
                    continue;

                buyOutNotification = new Notification();
                buyOutNotification.Title = "Buy out";
                buyOutNotification.Description = auctionRecord.NFTData.NFT.Title + "is buyout";
                buyOutNotification.NFTDataId = auctionRecord.NFTDataId;
                buyOutNotification.NFTData = auctionRecord.NFTData;
                buyOutNotification.UserProfileHeaderId = bidderUserId;

                _context.Notification.AddAsync(buyOutNotification);

                outBidConnections = _userConnectionManager.GetUserConnections(bidderUserId.ToString());
                connectedUser = _profileService.GetProfileAsync(bidderUserId).Result;

                if (outBidConnections != null && outBidConnections.Count > 0)
                {
                    foreach (var connectionId in outBidConnections)
                    {
                        _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", outBidMessageJsonString);
                        _hubContext.Clients.Client(connectionId).SendAsync("BuyOut", outBidMessageJsonString);
                    }
                }

                EmailUtility.SendEmail(connectedUser.Email, "Starboard - Buy out", buyOutNotification.Description, "alert", true, "Out Bid", "https://nft.starboard.org/placebid/" + auctionRecord.Id);
            }
        }

        public decimal GetIncrementBidAmount(decimal CurrentBidPrice, Guid auctionId)
        {
            double incrementBidAmount = 0;
            double CurBidPrice = (double)CurrentBidPrice;
            if (CurBidPrice > 0.01 && CurBidPrice < 0.99)
                incrementBidAmount = 0.05;
            else if (CurBidPrice >= 1.00 && CurBidPrice <= 4.99)
                incrementBidAmount = 0.25;
            else if (CurBidPrice >= 5.00 && CurBidPrice <= 24.99)
                incrementBidAmount = 0.50;
            else if (CurBidPrice >= 25.00 && CurBidPrice <= 99.99)
                incrementBidAmount = 1.00;
            else if (CurBidPrice >= 100 && CurBidPrice <= 249.99)
                incrementBidAmount = 2.50;
            else if (CurBidPrice >= 250 && CurBidPrice <= 499.99)
                incrementBidAmount = 5.00;
            else if (CurBidPrice >= 500.00 && CurBidPrice <= 999.99)
                incrementBidAmount = 10.00;
            else if (CurBidPrice >= 1000.00 && CurBidPrice <= 2499.99)
                incrementBidAmount = 25.00;
            else if (CurBidPrice >= 2500.00 && CurBidPrice <= 4999.99)
                incrementBidAmount = 50.00;
            else if (CurBidPrice <= 5000.00)
                incrementBidAmount = 100.00;

            if (auctionId != Guid.Empty)
            {
                var auction = _context.Auction.Where(x => x.Id == auctionId).FirstOrDefault();
                if ((decimal)incrementBidAmount != auction.IncrementAmount)
                {
                    auction.IncrementAmount = (decimal)incrementBidAmount;
                    _context.SaveChanges();
                }
            }

            return (decimal)incrementBidAmount;
        }
    }
}
