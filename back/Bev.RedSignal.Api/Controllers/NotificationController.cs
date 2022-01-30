using System;
using System.Collections.Generic;
using Bev.RedSignal.Api.Models;
using Bev.RedSignal.Api.Services.Interfaces;
using Bev.RedSignal.Api.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Bev.RedSginal.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly IUserAccessor _userAccessor;
        private readonly IUserQueryString _userQueryString;
        private readonly IHubContext<RedHub> _redHubcontext;

        public NotificationController(ILogger<NotificationController> logger,
                                      IUserAccessor userAccessor,
                                      IUserQueryString userQueryString,
                                      IHubContext<RedHub> redHubcontext)
        {
            _logger = logger;
            _userAccessor = userAccessor;
            _userQueryString = userQueryString;
            _redHubcontext = redHubcontext;
        }

        [HttpPost]
        public IActionResult PostNotification([FromBody] Notification notification)
        {
            if (string.IsNullOrWhiteSpace(notification.Message))
            {
                return BadRequest();
            }

            var username = _userAccessor.GetCurrentUsername();
            _logger.LogInformation("Sending notification to: " + username);

            var message = $"{username}: {notification.Message}";

            _redHubcontext.Clients.User(username).SendAsync("ReceiveMessage", message);

            return Ok();
        }

        [HttpPost("qs")]
        public IActionResult PostNotificationQueryString([FromBody] NotificationQuery notificationQuery)
        {
            if (string.IsNullOrWhiteSpace(notificationQuery.Message) ||
                    string.IsNullOrWhiteSpace(notificationQuery.Ad))
            {
                return BadRequest();
            }

            foreach (KeyValuePair<string, string> user in _userQueryString.DictUsers)
            {
                Console.WriteLine("--> Querystring user: " + user.Key);
                Console.WriteLine("--> ConnectionId user: " + user.Value);

                if (user.Key == notificationQuery.Ad)
                {
                    var message = $"{user.Key}: {notificationQuery.Message}";
                    _logger.LogInformation("Sending notification to: " + user.Key);

                    _redHubcontext.Clients.Client(user.Value).SendAsync("ReceiveMessage", message);
                }
            }


            return Ok();
        }
    }
}
