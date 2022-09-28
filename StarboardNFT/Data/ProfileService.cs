using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using StarboardNFT.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using StarboardNFT.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace StarboardNFT.Data
{
    public class ProfileService
    {
        #region Private Variables
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor accessor,
            LinkGenerator generator,
            ILogger<ProfileService> logger)
        {
            _context = context;
            _userManager = userManager;
            _accessor = accessor;
            _generator = generator;
            _logger = logger;
        }

        #endregion

        public async Task<Guid> GetProfileHeaderIdAsync(string ethAddress)
        {
            var profileHeaderId = Guid.Empty;
            var profileSettings = await _context.UserProfile.Where(x => x.EthAddress == ethAddress && x.Active == true).FirstOrDefaultAsync();

            if (profileSettings != null)
                profileHeaderId = profileSettings.UserProfileHeaderId;
            return profileHeaderId;
        }

        public async Task<List<UserProfileHeader>> GetFollowingUsersByEthAddress(string ethAddress)
        {
            Guid profileHeaderId = await GetProfileHeaderIdAsync(ethAddress);

            List<UserProfileHeader> profileHeaderList = new List<UserProfileHeader>();
            var query = _context.UserProfileHeader.Join(_context.UserFollowing, profileHeader => profileHeader.Id, uFollowing => uFollowing.MainUserProfileHeaderId, (profileHeader, uFollowing) => new { PHeader = profileHeader, UFollwoing = uFollowing })
                .Where(pHeaderAndUFollowing => pHeaderAndUFollowing.PHeader.Id == profileHeaderId);

            profileHeaderList = await query.Select(m => new UserProfileHeader
            {
                Id = m.UFollwoing.FollowingUserProfileHeaderId,
                UserName = m.UFollwoing.FollowingUserProfileHeader.UserName,
                UserPhoto = m.UFollwoing.FollowingUserProfileHeader.UserPhoto
            }).ToListAsync();

            return profileHeaderList;
        }

        public async Task<List<UserProfileHeader>> GetFollowingUsersByProfileHeaderId(Guid mainProfileHeaderID)
        {
            List<UserProfileHeader> profileHeaderList = new List<UserProfileHeader>();
            var query = _context.UserProfileHeader.Join(_context.UserFollowing, profileHeader => profileHeader.Id, uFollowing => uFollowing.MainUserProfileHeaderId, (profileHeader, uFollowing) => new { PHeader = profileHeader, UFollwoing = uFollowing })
                .Where(pHeaderAndUFollowing => pHeaderAndUFollowing.PHeader.Id == mainProfileHeaderID);

            profileHeaderList = await query.Select(m => new UserProfileHeader
            {
                Id = m.UFollwoing.FollowingUserProfileHeaderId,
                UserName = m.UFollwoing.FollowingUserProfileHeader.UserName,
                UserPhoto = m.UFollwoing.FollowingUserProfileHeader.UserPhoto
            }).ToListAsync();

            return profileHeaderList;
        }

        public async Task<bool> GetResultOfFollow(Guid mainProfileHeaderID, Guid selfProfileHeaderID)
        {
            bool bFollow = false;

            var followingCounts = await _context.UserFollowing.Where(x => x.MainUserProfileHeaderId == mainProfileHeaderID && x.FollowingUserProfileHeaderId == selfProfileHeaderID)
                .CountAsync();

            if (followingCounts > 0) bFollow = true;

            return bFollow;
        }

        public async Task<bool> UnFollowUserProfile(Guid mainProfileHeaderID, Guid selfProfileHeaderID)
        {
            bool bResult = false;

            var following = await _context.UserFollowing.Where(x => x.MainUserProfileHeaderId == mainProfileHeaderID && x.FollowingUserProfileHeaderId == selfProfileHeaderID)
                .FirstOrDefaultAsync();

            _context.UserFollowing.Remove(following);
            await _context.SaveChangesAsync();
            bResult = true;

            return bResult;
        }

        public async Task<List<UserProfileHeader>> GetFollowersByEthAddress(string ethAddress)
        {
            Guid profileHeaderId = await GetProfileHeaderIdAsync(ethAddress);

            List<UserProfileHeader> profileHeaderList = new List<UserProfileHeader>();
            var query = _context.UserProfileHeader.Join(_context.UserFollowing, profileHeader => profileHeader.Id, uFollowing => uFollowing.MainUserProfileHeaderId, (profileHeader, uFollowing) => new { PHeader = profileHeader, UFollwoing = uFollowing })
                .Where(pHeaderAndUFollowing => pHeaderAndUFollowing.UFollwoing.FollowingUserProfileHeaderId == profileHeaderId);

            profileHeaderList = await query.Select(m => new UserProfileHeader
            {
                Id = m.PHeader.Id,
                UserName = m.PHeader.UserName,
                UserPhoto = m.PHeader.UserPhoto
            }).ToListAsync();

            return profileHeaderList;
        }

        public async Task<UserProfileHeader> GetProfileHeaderByIDAsync(Guid userProfileHeaderID)
        {
            var profileSettings = await _context.UserProfileHeader.Where(x => x.Id == userProfileHeaderID && x.Active == true).FirstOrDefaultAsync();

            return profileSettings;
        }

        public async Task<UserProfileHeader> GetProfileHeaderByUsernameAsync(string username)
        {
            var profileSettings = await _context.UserProfileHeader.Where(x => x.UserName == username && x.Active == true).FirstOrDefaultAsync();

            return profileSettings;
        }

        public async Task<UserProfileHeader> GetProfileAsync(Guid profileHeaderId)
        {
            var profileSettings = await _context.UserProfileHeader.Where(x => x.Id == profileHeaderId && x.Active == true).FirstOrDefaultAsync();

            if(profileSettings == null)
            {
                profileSettings = new UserProfileHeader();
            }

            return profileSettings;
        }

        public async Task<System.Collections.Generic.IEnumerable<UserProfile>> GetAllProfileAsync(Guid hId)
        {
            var profileSettings = await _context.UserProfile.Where(x => x.UserProfileHeaderId == hId && x.Active == true).ToListAsync();

            return profileSettings;
        }

        public async Task<UserProfileHeader> GetDefaultProfileByhIdAsync(Guid hId)
        {
            var profileSettings = await _context.UserProfileHeader.Where(x => x.Id == hId && x.Active == true).FirstOrDefaultAsync();

            if (profileSettings == null)
            {
                profileSettings = new UserProfileHeader();
            }

            return profileSettings;
        }

        public async Task<UserProfileHeader> GetFeaturedProfile()
        {
            UserProfileHeader profileSettings = new UserProfileHeader();
            profileSettings = await _context.UserProfileHeader.Where(x => x.IsFeatured == true && x.Active == true).FirstOrDefaultAsync();

            if (profileSettings == null)
            {
                profileSettings = await _context.UserProfileHeader.Where(x => x.Active == true).FirstOrDefaultAsync();
            }

            return profileSettings;
        }

        public async Task<Guid> CopyDefaultProfileToNewProfileAsync(UserProfile objProfileSettings)
        {
            Guid profile_headerId = objProfileSettings.UserProfileHeaderId;

            var profileSettings = await _context.UserProfile.Where(x => x.EthAddress == objProfileSettings.EthAddress && x.Active == true).FirstOrDefaultAsync();

            if (profile_headerId == Guid.Empty) return profile_headerId;

            if (profileSettings == null)
            {
                UserProfile aProfileSettings = new UserProfile();
                aProfileSettings.EthAddress = objProfileSettings.EthAddress;

                aProfileSettings.UserProfileHeaderId = objProfileSettings.UserProfileHeaderId;
                profile_headerId = objProfileSettings.UserProfileHeaderId;

                await _context.UserProfile.AddAsync(aProfileSettings);
            }

            await _context.SaveChangesAsync();

            var profileHeader = await _context.UserProfileHeader.Where(x => x.Id == profile_headerId && x.Active == true).FirstOrDefaultAsync();
            EmailUtility.SendEmail(profileHeader.Email, "Starboard - Linked Account", "Welcome to Starboard. You account is linked successfully!", "alert", true, "Linked Account", "https://nft.starboard.org/viewprofile");

            return profile_headerId;
        }

        public async Task<Guid> SaveProfileAsync(UserProfileHeader objProfileSettings)
        {
            var success = false;
            Guid profile_headerId = Guid.Empty;
            UserProfileHeader aProfileSettings = new UserProfileHeader();
            var profileSettings = await _context.UserProfileHeader.Where(x => x.UserName == objProfileSettings.UserName && x.Active == true).FirstOrDefaultAsync();

            if (profileSettings == null)
            {
                aProfileSettings.EmailNotification = objProfileSettings.EmailNotification;
                aProfileSettings.Email = objProfileSettings.Email;
                aProfileSettings.SubscribedNewsletter = objProfileSettings.SubscribedNewsletter;
                aProfileSettings.TermsAgree = objProfileSettings.TermsAgree;
                aProfileSettings.UserName = objProfileSettings.UserName;
                aProfileSettings.UserPhoto = objProfileSettings.UserPhoto;
                aProfileSettings.UserBanner = objProfileSettings.UserBanner;
                aProfileSettings.UserOverview = objProfileSettings.UserOverview;
                aProfileSettings.UserSkills = objProfileSettings.UserSkills;

                await _context.UserProfileHeader.AddAsync(aProfileSettings);
                profile_headerId = aProfileSettings.Id;
                success = true;
            }
            else if(objProfileSettings.Id == Guid.Empty)
            {
                profile_headerId = Guid.Empty;
                return profile_headerId;
            }
            else
            {
                profileSettings.EmailNotification = objProfileSettings.EmailNotification;
                profileSettings.Email = objProfileSettings.Email;
                profileSettings.SubscribedNewsletter = objProfileSettings.SubscribedNewsletter;
                profileSettings.TermsAgree = objProfileSettings.TermsAgree;
                profileSettings.UserName = objProfileSettings.UserName;
                profileSettings.UserPhoto = objProfileSettings.UserPhoto;
                profileSettings.UserBanner = objProfileSettings.UserBanner;
                profileSettings.UserOverview = objProfileSettings.UserOverview;
                profileSettings.UserSkills = objProfileSettings.UserSkills;

                profile_headerId = objProfileSettings.Id;
                success = true;
            }

            await _context.SaveChangesAsync();

            EmailUtility.SendEmail(objProfileSettings.Email, "Starboard - Registeration", "Welcome to Starboard. You are registered as a user of our website successfully!", "alert", true, "Registeration", "https://nft.starboard.org/viewprofile");

            return profile_headerId;
        }

        public async Task<Guid> SaveUserProfileAsync(UserProfile objProfileSettings)
        {
            var success = false;
            Guid profile_headerId = Guid.Empty;
            var profileSettings = _context.UserProfile.Where(x => x.EthAddress == objProfileSettings.EthAddress && x.Active == true).FirstOrDefault();

            if (profileSettings == null)
            {
                UserProfile aProfileSettings = new UserProfile();
                aProfileSettings.EthAddress = objProfileSettings.EthAddress;
                aProfileSettings.UserProfileHeaderId = objProfileSettings.UserProfileHeaderId;

                await _context.UserProfile.AddAsync(aProfileSettings);
                success = true;
            }
            else
            {
                success = true;
            }


            await _context.SaveChangesAsync();
            return profile_headerId;
        }

        public async Task<Guid> CreateProfileHeaderAsync(Guid profileHeaderId)
        {
            UserProfileHeader aProfileHeader = new UserProfileHeader();

            if (profileHeaderId == Guid.Empty)
            {
                await _context.UserProfileHeader.AddAsync(aProfileHeader);
                await _context.SaveChangesAsync();
            }

            return aProfileHeader.Id;
        }

        public async Task<bool> ResetDefaultProfileAsync(Guid profileHeaderId)
        {
            var success = true;
            var profileSettings = _context.UserProfile.Where(x => x.UserProfileHeaderId == profileHeaderId && x.Active == true);

            await _context.SaveChangesAsync();

            return success;
        }

        public async Task<int> FollowUserProfile(Guid mainProfileHeaderId, Guid followingProfileHeaderId)
        {
            int success = 0;

            var nftData = await _context.NFTData.Where(x => x.Active == true)
                .Include(x => x.NFT)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var following = new UserFollowing();

            var followListCount = await _context.UserFollowing.Where(x => x.MainUserProfileHeaderId == mainProfileHeaderId && x.FollowingUserProfileHeaderId == followingProfileHeaderId).CountAsync();

            if (followListCount == 0 && mainProfileHeaderId != followingProfileHeaderId)
            {
                following.MainUserProfileHeaderId = mainProfileHeaderId;
                following.FollowingUserProfileHeaderId = followingProfileHeaderId;

                await _context.UserFollowing.AddAsync(following);

                //add record in activity table
                Activity activity = new Activity();
                activity.Title = "New follow";
                activity.Description = "New follow!";
                activity.NFTDataId = nftData.Id;
                activity.Category = Activity.Categories.MyActivity;
                activity.filterCategory = Activity.FilterCategories.Followings;
                activity.UserProfileHeaderId = mainProfileHeaderId;

                await _context.Activity.AddAsync(activity);
                success = 1;

                await _context.SaveChangesAsync();
            }

            if (mainProfileHeaderId == followingProfileHeaderId) success = 2;
            
            return success;
        }
    }
}
