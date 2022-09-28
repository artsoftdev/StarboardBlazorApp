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
    public class NotificationsService
    {
        #region Private Variables
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NFTService> _logger;
        private readonly IUserConnectionManager _userConnectionManager;

        private IHubContext<NotificationHub> _hubContext;

        public NotificationsService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor accessor,
            LinkGenerator generator,
            ILogger<NFTService> logger,
            IHubContext<NotificationHub> hubContext,
            IUserConnectionManager userConnectionManager)
        {
            _context = context;
            _userManager = userManager;
            _accessor = accessor;
            _generator = generator;
            _logger = logger;
            _hubContext = hubContext;
            //_userConnectionManager = userConnectionManager;
            _userConnectionManager = userConnectionManager;
        }

        #endregion

        public async Task<System.Collections.Generic.IEnumerable<Notification>> GetNotificationbyUserHeaderId(Guid nftId)
        {
            var notifications = await _context.Notification.Where(x => x.UserProfileHeaderId == nftId && x.Active == true).ToListAsync();
            return notifications;
        }

        public async Task<System.Collections.Generic.IEnumerable<Notification>> GetRecentNotificationbyUserHeaderId(Guid nftId)
        {
            var notifications = await _context.Notification.Where(x => x.UserProfileHeaderId == nftId && x.Active == true)
                .Include(x => x.NFTData)
                .Include(x => x.NFTData.NFT).ToListAsync();
            return notifications;
        }

        public async Task ReadNewNotifications(Guid newNotificationsProfileHeaderId)
        {
            _context.Notification.Where(x => x.UserProfileHeaderId == newNotificationsProfileHeaderId).ToList()
                .ForEach(y => y.IsRead = true);

            await _context.SaveChangesAsync();
        }

        public async Task SendNotification(string userId)
        {
            var connections = _userConnectionManager.GetUserConnections(userId);

            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNofitication", "ABC is abc");
                }
            }

            // Notify clients in the group
            //await _hubContext.Clients.All.SendAsync("ReceiveNofitication", "ABC is abc");
        }
    }
}
