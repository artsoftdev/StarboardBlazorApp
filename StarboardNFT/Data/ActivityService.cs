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
using Microsoft.AspNetCore.SignalR;
using StarboardNFT.Hubs;
using StarboardNFT.Interface;

namespace StarboardNFT.Data
{
    public class ActivityService
    {
        #region Private Variables
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NFTService> _logger;
        private readonly IUserConnectionManager _userConnectionManager;

        public ActivityService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor accessor,
            LinkGenerator generator,
            ILogger<NFTService> logger,
            IUserConnectionManager userConnectionManager)
        {
            _context = context;
            _userManager = userManager;
            _accessor = accessor;
            _generator = generator;
            _logger = logger;
            _userConnectionManager = userConnectionManager;
        }

        #endregion

        public async Task<System.Collections.Generic.IEnumerable<Activity>> GetActivitybyUserHeaderId(Guid nftId)
        {
            var activities = await _context.Activity.Where(x => x.UserProfileHeaderId == nftId && x.Category == Activity.Categories.MyActivity && x.Active == true)
                .Include(x => x.NFTData)
                .Include(x => x.NFTData.NFT).ToListAsync();
            return activities;
        }

        public async Task<System.Collections.Generic.IEnumerable<Activity>> GetActivitybyUserHeaderIdAndFilter(Guid nftId)
        {
            var activities = await _context.Activity.Where(x => x.UserProfileHeaderId == nftId && x.Category == Activity.Categories.MyActivity && x.Active == true)
                .Include(x => x.NFTData)
                .Include(x => x.NFTData.NFT).ToListAsync();
            return activities;
        }

        public async Task<System.Collections.Generic.IEnumerable<Activity>> GetFollwingbyUserHeaderId(Guid nftId)
        {
            var following = await _context.Activity.Where(x => x.UserProfileHeaderId == nftId && x.Category == Activity.Categories.Following && x.Active == true)
                .Include(x => x.NFTData)
                .Include(x => x.NFTData.NFT).ToListAsync();
            return following;
        }

        public async Task<int> SaveLikeNFTActivity(Guid NftDataId, Guid UserProfileHeaderId)
        {
            int result = 0;
            var NFTItem = await _context.NFTData.Where(x => x.Id == NftDataId && x.Active == true)
                .Include(x => x.NFT)
                .ThenInclude(x => x.UserProfile)
                .FirstOrDefaultAsync();

            if (UserProfileHeaderId == NFTItem.NFT.UserProfile.UserProfileHeaderId)
                return 2;

            var nftLikeListCount = await _context.NFTLikes.Where(x => x.UserProfileHeaderId == UserProfileHeaderId && x.NFTDataId == NftDataId && x.Active == true).CountAsync();

            if (nftLikeListCount == 0)
            {
                //add record in activity table
                Activity activity = new Activity();
                activity.Title = "New like Received";
                activity.Description = "New like received for your NFT.";
                activity.Category = Activity.Categories.MyActivity;
                activity.filterCategory = Activity.FilterCategories.Likes;
                activity.NFTDataId = NftDataId;
                activity.UserProfileHeaderId = NFTItem.NFT.UserProfile.UserProfileHeaderId;

                _context.Activity.Add(activity);

                NFTLikes nftLike = new NFTLikes();
                nftLike.NFTDataId = NftDataId;
                nftLike.UserProfileHeaderId = UserProfileHeaderId;

                _context.NFTLikes.Add(nftLike);

                NFTItem.NFT.LikeCount = NFTItem.NFT.LikeCount + 1;
                result = 1;
            }

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<int> SaveUnLikeNFT(Guid NftDataId, Guid UserProfileHeaderId)
        {
            int result = 0;
            var NFTItem = await _context.NFTData.Where(x => x.Id == NftDataId && x.Active == true)
                .Include(x => x.NFT)
                .ThenInclude(x => x.UserProfile)
                .FirstOrDefaultAsync();

            var unlikeNFT = await _context.NFTLikes.Where(x => x.UserProfileHeaderId == UserProfileHeaderId && x.NFTDataId == NftDataId && x.Active == true).FirstOrDefaultAsync();

            if (unlikeNFT != null)
            {
                _context.NFTLikes.Remove(unlikeNFT);
                NFTItem.NFT.LikeCount = NFTItem.NFT.LikeCount - 1;
            }

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task SaveFavoriteNFT(Guid NftDataId, Guid UserProfileHeaderId)
        {
            var NFTItem = await _context.NFTData.Where(x => x.Id == NftDataId && x.Active == true)
                .Include(x => x.NFT)
                .FirstOrDefaultAsync();

            var nftFavoriteListCount = await _context.NFTFavorites.Where(x => x.UserProfileId == NFTItem.NFT.UserProfileId && x.NFTDataId == NftDataId && x.Active == true).CountAsync();

            if (nftFavoriteListCount == 0)
            {
                NFTFavorites nftFavorite = new NFTFavorites();
                nftFavorite.NFTDataId = NftDataId;
                nftFavorite.UserProfileId = NFTItem.NFT.UserProfileId;

                _context.NFTFavorites.Add(nftFavorite);

                NFTItem.NFT.FavoriteCount = NFTItem.NFT.FavoriteCount + 1;
                await _context.SaveChangesAsync();
            }
        }
    }
}
