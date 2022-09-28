using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StarboardNFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarboardNFT.Interface;
using StarboardNFT.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StarboardNFT.Utilities;
using System.IO;

namespace StarboardNFT.Data
{
    public class NFTService
    {
        #region Private Variables
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NFTService> _logger;

        private IHubContext<NotificationHub> _hubContext;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ProfileService _profileService;

        public NFTService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor accessor,
            LinkGenerator generator,
            ILogger<NFTService> logger,
            IHubContext<NotificationHub> hubContext,
            IUserConnectionManager userConnectionManager,
            ProfileService profileService)
        {
            _context = context;
            _userManager = userManager;
            _accessor = accessor;
            _generator = generator;
            _logger = logger;
            _hubContext = hubContext;
            _profileService = profileService;
            _userConnectionManager = userConnectionManager;
        }

        #endregion

        public async Task<Guid> OnNFTPublish(NFT nft_data)
        {
            var success = false;

            NFT nft = new NFT();
            NFTData nftData = new NFTData();
            nft.Title = nft_data.Title;
            nft.Description = nft_data.Description;
            nft.Category = nft_data.Category;
            nft.FileName = nft_data.FileName;
            nft.FileContent = nft_data.FileContent;
            nft.FileType = nft_data.FileType;
            nft.UserProfileId = nft_data.UserProfileId;
            nft.TotalNumberOfMintedNFT = nft_data.TotalNumberOfMintedNFT;

            await _context.NFT.AddAsync(nft);
            
            nftData.NFTId = nft.Id;
            await _context.NFTData.AddAsync(nftData);
            success = true;

            using (var ms = new MemoryStream(nft_data.FileContent))
            {
                using (var fs = new FileStream("./wwwroot/NFTs/" + nft_data.FileName, FileMode.Create))
                {
                    ms.WriteTo(fs);
                }
            }

            await _context.SaveChangesAsync();
            return nftData.NFTId;
        }

        public async Task<bool> OnNFTDataPublish(NFTData nft_data)
        {
            var success = false;

            var nftData = _context.NFTData.Where(x => x.NFTId == nft_data.NFTId && x.Active == true).Include(x => x.NFT.UserProfile.UserProfileHeader).FirstOrDefault();

            //NFTData nftData = new NFTData();
            nftData.SaleStartDate = nft_data.SaleStartDate;
            nftData.SaleEndtDate = nft_data.SaleEndtDate;
            nftData.FiatStartPrice = nft_data.FiatStartPrice;
            nftData.HasBuyoutPrice = nft_data.HasBuyoutPrice;
            nftData.FiatBuyOutPrice = nft_data.FiatBuyOutPrice;
            nftData.HasReservePrice = nft_data.HasReservePrice;
            nftData.FiatReservePrice = nft_data.FiatReservePrice;
            nftData.Royalty = nft_data.Royalty;
            nftData.Tags = nft_data.Tags;
            nftData.NFTId = nft_data.NFTId;
            nftData.UniqueNumberOfMintedNFT = nft_data.UniqueNumberOfMintedNFT;

            //await _context.NFTData.AddAsync(nftData);
            success = true;

            await _context.SaveChangesAsync();

            if (nft_data.SaleStartDate.Value < DateTime.UtcNow)
            {
                nftData.IsSaleStarted = true;
                await _context.SaveChangesAsync();

                Auction newAuction = new Auction();
                newAuction.CurrentBidPrice = nft_data.FiatStartPrice;
                newAuction.MaxBidPrice = nft_data.FiatStartPrice;
                newAuction.IncrementAmount = GetIncrementBidAmount(newAuction.CurrentBidPrice, Guid.Empty);
                newAuction.NFTDataId = nft_data.Id;
                newAuction.CurrentWinningUserId = nft_data.NFT.UserProfile.UserProfileHeaderId;

                await _context.Auction.AddAsync(newAuction);
                //await _context.SaveChangesAsync();

                //nftData.AuctionId = newAuction.Id;

                Notification notification = new Notification();
                notification.Title = "New Sale Started";
                notification.Description = nft_data.NFT.Title + " sale is started";
                notification.NFTDataId = nft_data.Id;
                notification.NFTData = nft_data;
                notification.UserProfileHeaderId = nft_data.NFT.UserProfile.UserProfileHeaderId;

                await _context.Notification.AddAsync(notification);

                //add record in activity table
                Activity activity = new Activity();
                activity.Title = "New Sale Started";
                activity.Description = nft_data.NFT.Title + " sale is started";
                activity.NFTDataId = nft_data.Id;
                activity.Category = Activity.Categories.MyActivity;
                activity.filterCategory = Activity.FilterCategories.Sales;
                activity.UserProfileHeaderId = nft_data.NFT.UserProfile.UserProfileHeaderId;

                await _context.Activity.AddAsync(activity);

                var followers = await _context.UserFollowing.Where(x => x.FollowingUserProfileHeaderId == nft_data.NFT.UserProfile.UserProfileHeaderId).ToListAsync();
                foreach (var follower in followers)
                {
                    //add record in activity table as following
                    activity = new Activity();
                    activity.Title = "New Sale Started";
                    activity.Description = nft_data.NFT.Title + " sale is started";
                    activity.NFTDataId = nft_data.Id;
                    activity.Category = Activity.Categories.Following;

                    var UserProfileHeader = await _context.UserProfile.Where(x => x.Id == follower.MainUserProfileHeaderId).FirstOrDefaultAsync();
                    activity.UserProfileHeaderId = UserProfileHeader.UserProfileHeaderId;

                    await _context.Activity.AddAsync(activity);
                }

                string messageJsonString = JsonConvert.SerializeObject(notification, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                var connections = _userConnectionManager.GetUserConnections(notification.NFTData.NFT.UserProfile.UserProfileHeaderId.ToString());

                var connectedUser = await _profileService.GetProfileAsync(notification.UserProfileHeaderId);

                if (connections != null && connections.Count > 0)
                {
                    foreach (var connectionId in connections)
                    {
                        await _hubContext.Clients.Client(connectionId).SendAsync("NewAuctionNofitication", messageJsonString);
                    }
                }

                EmailUtility.SendEmail(connectedUser.Email, "Starboard - sale is started", notification.Description, "alert", true, "New Sale Started", "https://nft.starboard.org/placebid/" + newAuction.Id);

                await _context.SaveChangesAsync();
            }

            return success;
        }

        public async Task<Guid> GetUserProfileId(string ethAddress)
        {
            Guid userProfileId = Guid.Empty;
            var userProfile = _context.UserProfile.Where(x => x.EthAddress == ethAddress && x.Active == true).FirstOrDefault();

            userProfileId = userProfile.Id;

            return userProfileId;
        }

        public async Task<List<NFTData>> GetAllNFT()
        {
            var nftList = new List<NFTData>();
            nftList = await _context.NFTData
                .Include(x => x.NFT)
                .Take(10).ToListAsync();

            return nftList;
        }

        public async Task<NFT> GetNFTbyId(string nftId)
        {
            NFT nft = new NFT();
            nft = _context.NFT.Where(x => x.Id == new Guid(nftId) && x.Active == true).FirstOrDefault();

            return nft;
        }

        public async Task<NFTData> GetNFTDatabyNFTId(string nftId)
        {
            NFTData nftData = new NFTData();
            nftData = await _context.NFTData.Where(x => x.NFTId == new Guid(nftId) && x.Active == true).FirstOrDefaultAsync();

            return nftData;
        }

        public List<Guid> GetAllLikeNFTDataIDByUserProfileID(Guid UserProfileHeaderID)
        {
            List<NFTData> nftDataList = new List<NFTData>();

            var nftIdlist = _context.NFTLikes.Where(x => x.UserProfileHeaderId == UserProfileHeaderID && x.Active == true).Select(x => x.NFTDataId).ToList();
            nftDataList = _context.NFTLikes.Where(x => x.UserProfileHeaderId == UserProfileHeaderID
            && x.Active == true)
                .Join(_context.NFTData, n => n.NFTDataId, nd => nd.Id, (n, nd) => new { n, nd })
                .Select(nftDataWithNFTLike => new NFTData 
                { 
                    Id = nftDataWithNFTLike.n.NFTDataId
                }).ToList();

            return nftIdlist;
        }

        public List<NFTData> GetRandomNFTData(Guid headerID, List<string> selectedTagList, int position, int amount)
        {
            var followingProfileList = _context.UserFollowing.Where(x => x.FollowingUserProfileHeaderId == headerID).Select(x => x.MainUserProfileHeaderId).ToList();

            //var nftDataList
            List<NFTData> nftDataList = new List<NFTData>();

            if (selectedTagList.Count() == 0)
            {
                nftDataList = _context.NFTData.Where(x => !followingProfileList.Contains(x.NFT.UserProfile.UserProfileHeaderId) && x.Active == true)
                    .Include(y => y.NFT)
                    .ThenInclude(y => y.UserProfile.UserProfileHeader)
                    .Select(nft_data => new NFTData
                    {
                        Id = nft_data.Id,
                        SaleStartDate = nft_data.SaleStartDate,
                        SaleEndtDate = nft_data.SaleEndtDate,
                        FiatStartPrice = nft_data.FiatStartPrice,
                        HasBuyoutPrice = nft_data.HasBuyoutPrice,
                        FiatBuyOutPrice = nft_data.FiatBuyOutPrice,
                        HasReservePrice = nft_data.HasReservePrice,
                        FiatReservePrice = nft_data.FiatReservePrice,
                        Royalty = nft_data.Royalty,
                        Tags = nft_data.Tags,
                        NFTId = nft_data.NFTId,
                        NFT = nft_data.NFT,
                        UniqueNumberOfMintedNFT = nft_data.UniqueNumberOfMintedNFT,
                        Auctions = nft_data.Auctions.Where(x => x.Active == true).ToList()
                    })
                    //.AsNoTracking()
                    .ToList();
            }
            else
            {
                nftDataList = _context.NFTData
                .Include(y => y.NFT)
                .ThenInclude(y => y.UserProfile.UserProfileHeader)
                .AsEnumerable()
                .Where(x => !followingProfileList.Contains(x.NFT.UserProfile.UserProfileHeaderId) && selectedTagList.Any(t => x.Tags.Contains(t)) == true && x.Active == true)
                .Select(nft_data => new NFTData
                {
                    Id = nft_data.Id,
                    SaleStartDate = nft_data.SaleStartDate,
                    SaleEndtDate = nft_data.SaleEndtDate,
                    FiatStartPrice = nft_data.FiatStartPrice,
                    HasBuyoutPrice = nft_data.HasBuyoutPrice,
                    FiatBuyOutPrice = nft_data.FiatBuyOutPrice,
                    HasReservePrice = nft_data.HasReservePrice,
                    FiatReservePrice = nft_data.FiatReservePrice,
                    Royalty = nft_data.Royalty,
                    Tags = nft_data.Tags,
                    NFTId = nft_data.NFTId,
                    NFT = nft_data.NFT,
                    UniqueNumberOfMintedNFT = nft_data.UniqueNumberOfMintedNFT,
                    Auctions = nft_data.Auctions.Where(x => x.Active == true).ToList()
                })
                //.AsNoTracking()
                .ToList();
            }

            if (nftDataList.Count >= position + amount)
            {
                nftDataList = nftDataList.GetRange(position, amount);
            }
            else
                nftDataList = new List<NFTData>();

            return nftDataList;
        }
        public async Task<List<NFTData>> GetAllRandomNFTData(Guid headerID)
        {
            var followingProfileList = _context.UserFollowing.Where(x => x.FollowingUserProfileHeaderId == headerID).Select(x => x.MainUserProfileHeaderId).ToList();

            //var nftDataList
            List<NFTData> nftDataList = new List<NFTData>();

            nftDataList = _context.NFTData.Where(x => !followingProfileList.Contains(x.NFT.UserProfile.UserProfileHeaderId) && x.Active == true)
                    .Include(y => y.NFT)
                    .ThenInclude(y => y.UserProfile.UserProfileHeader)
                    .Select(nft_data => new NFTData
                    {
                        Id = nft_data.Id,
                        SaleStartDate = nft_data.SaleStartDate,
                        SaleEndtDate = nft_data.SaleEndtDate,
                        FiatStartPrice = nft_data.FiatStartPrice,
                        HasBuyoutPrice = nft_data.HasBuyoutPrice,
                        FiatBuyOutPrice = nft_data.FiatBuyOutPrice,
                        HasReservePrice = nft_data.HasReservePrice,
                        FiatReservePrice = nft_data.FiatReservePrice,
                        Royalty = nft_data.Royalty,
                        Tags = nft_data.Tags,
                        NFTId = nft_data.NFTId,
                        NFT = nft_data.NFT,
                        UniqueNumberOfMintedNFT = nft_data.UniqueNumberOfMintedNFT,
                        Auctions = nft_data.Auctions.Where(x => x.Active == true).ToList()
                    })
                    //.AsNoTracking()
                    .ToList();

            return nftDataList;
        }

        public async Task<NFTData> GetFeaturedNFTData()
        {
            NFTData nftData = new NFTData();    
            nftData = await _context.NFTData.Where(x => x.IsFeatured == true && x.Active == true)
                .Include(x => x.NFT)
                .Include(x => x.NFT.UserProfile)
                .Include(x => x.NFT.UserProfile.UserProfileHeader)
                .FirstOrDefaultAsync();

            if (nftData == null)
            {
                nftData = await _context.NFTData.Where(x => x.Active == true)
                    .Include(x => x.NFT)
                    .Include(x => x.NFT.UserProfile)
                    .Include(x => x.NFT.UserProfile.UserProfileHeader)
                    .FirstOrDefaultAsync();
            }

            return nftData;
        }

        public async Task<bool> CancelSale(string nftId)
        {
            NFTData nftData = new NFTData();
            nftData = _context.NFTData.Where(x => x.NFTId == new Guid(nftId) && x.Active == true).FirstOrDefault();
            nftData.Active = false;

            NFTData newNftData = new NFTData();
            newNftData.NFTId = nftData.NFTId;
            await _context.NFTData.AddAsync(newNftData);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<NFT>> GetNFTsByUserName(string username)
        {
            List<NFT> nftData = new List<NFT>();
            List<object> nftData1 = new List<object>();

            var result = _context.UserProfileHeader.Where(profileHeader => profileHeader.UserName == username)
                .Select(profileHeader => new { profileHeader, UserProfiles = profileHeader.UserProfiles.ToList() })
                .AsEnumerable()
                .Select(uProfile =>
                {
                    uProfile.profileHeader.UserProfiles = uProfile.UserProfiles;
                    return uProfile.profileHeader.UserProfiles;
                }).FirstOrDefault().ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                var nftItem = _context.NFT.Where(x => x.UserProfileId == result[i].Id).ToList();
                var nftItem1 = _context.NFT.Join(_context.NFTData, n => n.Id, nd => nd.NFTId, (n, nd) => new { n, nd })
                    .Select(m => new {
                        NftId = m.n.Id,
                        NftTitle = m.n.Title,
                        NftFileContent = m.n.FileContent,
                        NftDataSaleState = m.nd.SaleStartDate
                    }).ToList();

                nftData.AddRange(nftItem);
                nftData1.AddRange(nftItem1);
            }

            return nftData;
        }

        public List<UserProfileHeader> GetGroupNFTsByUserProfileHeaderID(Guid selfProfileheader_Id)
        {
            List<UserProfileHeader> userProfileList = new List<UserProfileHeader>();

            List<UserProfileHeader> nftList = _context.UserProfileHeader.Join(_context.NFT, profileHeader => profileHeader.Id, nft => nft.UserProfile.UserProfileHeaderId, (profileHeader, nft) => new { profileHeader }).Select(x => x.profileHeader).ToList();
            userProfileList = nftList
                .GroupBy(x => x.Id).ToList()
                .GroupJoin(_context.UserFollowing, userHeader => userHeader.FirstOrDefault().Id, userFollowing => userFollowing.MainUserProfileHeaderId, (userHeader, userFollowing) => new { userHeader, userFollowing })
                .Where(x => !(x.userFollowing.Count() > 0 && x.userFollowing.FirstOrDefault().FollowingUserProfileHeaderId == selfProfileheader_Id))
                .Select(x => x.userHeader.FirstOrDefault()).ToList();
            //nftList = allNftList.GroupBy(x => x.UserProfile.UserProfileHeaderId).ToList();

            return userProfileList;
        }

        public async Task<List<Auction>> GetOnSaleNFTDataAuctionGroupByUserProfileHeaderID(Guid headerID, Guid selfProfileheader_Id)
        {
            List<Auction> nftDataAuctionList = new List<Auction>();

            nftDataAuctionList = await _context.NFTData
                .Where(x => x.IsSaleEnded == false && x.Active == true)
                .Include(x => x.NFT)
                .Include(x => x.NFT.UserProfile)
                .Include(x => x.NFT.UserProfile.UserProfileHeader)
                .Join(_context.Auction, n => n.Id, nd => nd.NFTDataId, (n, nd) => new { n, nd })
                .Select(m => new Auction
                {
                    Id = m.nd.Id,
                    NFTDataId = m.nd.NFTDataId,
                    NFTData = m.nd.NFTData,
                    CurrentWinningUserId = m.nd.CurrentWinningUserId
                })
                .Where(x => x.NFTData.NFT.UserProfile.UserProfileHeaderId == headerID)
                .ToListAsync();

            var nftDataList = await _context.NFTData.Include(x => x.NFT).Include(x => x.NFT.UserProfile.UserProfileHeader).ToListAsync();

            //.Join(_context.AuctionBid, n => n.Id, nd => nd.AuctionId, (n, nd) => new { n, nd })
            //    .Select(m => new AuctionBid
            //    {
            //        Id = m.nd.Id,
            //        AuctionId = m.nd.AuctionId,
            //        Auction = m.nd.Auction,
            //        BidUserId = m.nd.BidUserId
            //    })
            //var queryGroup = query.Where(x => x.Auction.NFTData.NFT.UserProfile.UserProfileHeaderId == selfProfileheader_Id).GroupBy(x => x.BidUserId).ToList();
            //nftDataAuctionBidList = queryGroup;

            //var nftDataList = await _context.NFTData.Where(x => x.NFT.UserProfile.UserProfileHeaderId == headerID && 
            //x.IsSaleStarted == true && x.IsSaleEnded == false && x.Active == true)
            //    .Include(x => x.NFT)
            //    .AsNoTracking()
            //    .ToListAsync();

            return nftDataAuctionList;
        }

        public async Task<List<Auction>> GetCreatedNFTDataAuctionGroupByUserProfileHeaderID(Guid headerID, Guid selfProfileheader_Id)
        {
            List<Auction> nftDataAuctionList = new List<Auction>();

            nftDataAuctionList = await _context.NFTData
                .Where(x => x.Active == true)
                .Include(x => x.NFT)
                .Include(x => x.NFT.UserProfile)
                .Include(x => x.NFT.UserProfile.UserProfileHeader)
                .Join(_context.Auction, n => n.Id, nd => nd.NFTDataId, (n, nd) => new { n, nd })
                .Select(m => new Auction
                {
                    Id = m.nd.Id,
                    NFTDataId = m.nd.NFTDataId,
                    NFTData = m.nd.NFTData,
                    CurrentWinningUserId = m.nd.CurrentWinningUserId
                })
                .Where(x => x.NFTData.NFT.UserProfile.UserProfileHeaderId == headerID)
                .ToListAsync();

            return nftDataAuctionList;
        }

        public async Task<List<NFTData>> GetCreatedNFTDataByUserProfileHeaderID(Guid headerID)
        {
            var nftDataList = await _context.NFTData.Where(x => x.NFT.UserProfile.UserProfileHeaderId == headerID && x.Active == true)
                .Include(x => x.NFT)
                .Include(x => x.NFT.UserProfile.UserProfileHeader)
                .AsNoTracking()
                .ToListAsync();

            return nftDataList;
        }

        public async Task<List<NFTData>> GetOnSaleNFTDataByUserProfileHeaderID(Guid headerID)
        {
            var nftDataList = await _context.NFTData.Where(x => x.NFT.UserProfile.UserProfileHeaderId == headerID && x.IsSaleStarted == true && x.IsSaleEnded == false && x.Active == true)
                .Include(x => x.NFT)
                .Include(x => x.NFT.UserProfile.UserProfileHeader)
                .AsNoTracking()
                .ToListAsync();

            return nftDataList;
        }

        public async Task<List<NFTData>> GetNFTDataByUserName(string username)
        {
            var nftDataList = await _context.NFTData.Where(x => x.NFT.UserProfile.UserProfileHeader.UserName == username && x.Active == true)
                .Include(x => x.NFT)
                .Include(x => x.NFT.UserProfile)
                .Include(x => x.NFT.UserProfile.UserProfileHeader)
                .AsNoTracking()
                .ToListAsync();

            return nftDataList;
        }

        public List<NFTData> GetNFTDataByUserFollowingProfileHeaderID(Guid headerID, List<string> selectedTagList, int position, int amount)
        {
            var followingProfileList = _context.UserFollowing.Where(x => x.FollowingUserProfileHeaderId == headerID).Select(x => x.MainUserProfileHeaderId).ToList();

            //var nftDataList
            List<NFTData> nftDataList = new List<NFTData>();

            if (selectedTagList.Count() == 0)
            {
                nftDataList = _context.NFTData.Where(x => followingProfileList.Contains(x.NFT.UserProfile.UserProfileHeaderId) && x.Active == true)
                    .Include(y => y.NFT)
                    .ThenInclude(y => y.UserProfile.UserProfileHeader)
                    .Select(nft_data => new NFTData
                    {
                        Id = nft_data.Id,
                        SaleStartDate = nft_data.SaleStartDate,
                        SaleEndtDate = nft_data.SaleEndtDate,
                        FiatStartPrice = nft_data.FiatStartPrice,
                        HasBuyoutPrice = nft_data.HasBuyoutPrice,
                        FiatBuyOutPrice = nft_data.FiatBuyOutPrice,
                        HasReservePrice = nft_data.HasReservePrice,
                        FiatReservePrice = nft_data.FiatReservePrice,
                        Royalty = nft_data.Royalty,
                        Tags = nft_data.Tags,
                        NFTId = nft_data.NFTId,
                        NFT = nft_data.NFT,
                        UniqueNumberOfMintedNFT = nft_data.UniqueNumberOfMintedNFT,
                        Auctions = nft_data.Auctions.Where(x => x.Active == true).ToList()
                    })
                    //.AsNoTracking()
                    .ToList();
            }
            else
            {
                //var anftDataList = _context.NFTData.AsEnumerable().Where(x => followingProfileList.Contains(x.NFT.UserProfile.UserProfileHeaderId) && selectedTagList.Any(y => x.Tags.Contains(y)) == true && x.Active == true).ToList();
                nftDataList = _context.NFTData
                .Include(y => y.NFT)
                .ThenInclude(y => y.UserProfile.UserProfileHeader)
                .AsEnumerable().Where(x => followingProfileList.Contains(x.NFT.UserProfile.UserProfileHeaderId) && selectedTagList.Any(y => x.Tags.Contains(y)) == true && x.Active == true)
                .Select(nft_data => new NFTData
                {
                    Id = nft_data.Id,
                    SaleStartDate = nft_data.SaleStartDate,
                    SaleEndtDate = nft_data.SaleEndtDate,
                    FiatStartPrice = nft_data.FiatStartPrice,
                    HasBuyoutPrice = nft_data.HasBuyoutPrice,
                    FiatBuyOutPrice = nft_data.FiatBuyOutPrice,
                    HasReservePrice = nft_data.HasReservePrice,
                    FiatReservePrice = nft_data.FiatReservePrice,
                    Royalty = nft_data.Royalty,
                    Tags = nft_data.Tags,
                    NFTId = nft_data.NFTId,
                    NFT = nft_data.NFT,
                    UniqueNumberOfMintedNFT = nft_data.UniqueNumberOfMintedNFT,
                    Auctions = nft_data.Auctions.Where(x => x.Active == true).ToList()
                })
                //.AsNoTracking()
                .ToList();
            }

            if (nftDataList.Count >= position + amount)
            {
                nftDataList = nftDataList.GetRange(position, amount);
            }
            else
                nftDataList = new List<NFTData>();

            return nftDataList;
        }

        public List<NFTData> GetAllNFTDataByUserFollowingProfileHeaderID(Guid headerID)
        {
            var followingProfileList = _context.UserFollowing.Where(x => x.FollowingUserProfileHeaderId == headerID).Select(x => x.MainUserProfileHeaderId).ToList();

            //var nftDataList
            List<NFTData> nftDataList = new List<NFTData>();

            nftDataList = _context.NFTData.Where(x => followingProfileList.Contains(x.NFT.UserProfile.UserProfileHeaderId) && x.Active == true)
            .Include(y => y.NFT)
            .ThenInclude(y => y.UserProfile.UserProfileHeader)
            .Select(nft_data => new NFTData
            {
                Id = nft_data.Id,
                SaleStartDate = nft_data.SaleStartDate,
                SaleEndtDate = nft_data.SaleEndtDate,
                FiatStartPrice = nft_data.FiatStartPrice,
                HasBuyoutPrice = nft_data.HasBuyoutPrice,
                FiatBuyOutPrice = nft_data.FiatBuyOutPrice,
                HasReservePrice = nft_data.HasReservePrice,
                FiatReservePrice = nft_data.FiatReservePrice,
                Royalty = nft_data.Royalty,
                Tags = nft_data.Tags,
                NFTId = nft_data.NFTId,
                NFT = nft_data.NFT,
                UniqueNumberOfMintedNFT = nft_data.UniqueNumberOfMintedNFT,
                Auctions = nft_data.Auctions.Where(x => x.Active == true).ToList()
            })
            //.AsNoTracking()
            .ToList();

            return nftDataList;
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

            if (auctionId != Guid.Empty) {
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

    public static class StringExtensions
    {
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length > 0)
            {
                foreach (string value in values)
                {
                    if (str.Contains(value))
                        return true;
                }
            }

            return false;
        }
    }
}
