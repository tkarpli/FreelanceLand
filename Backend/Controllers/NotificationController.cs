﻿using Backend.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces.ServiceInterfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public INotificationService _notifService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(INotificationService notifService, IHubContext<NotificationHub> hubContext)
        {
            _notifService = notifService;
            _hubContext = hubContext;
        }

        [HttpPost("getNotifications")]
        public async Task GetNotifications([FromBody] UserAccountDTO user)
        {
            var dto = await _notifService.GetNotificationsAsync(user.Id);
            await Response.WriteAsync(JsonConvert.SerializeObject(dto, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpPost("getCount")]
        public async Task<int> GetCount([FromBody] UserAccountDTO user)
        {
            var count = await _notifService.GetNotificationCountAsync(user.Id);
            return count;
        }

        [HttpPost("deleteNotifications")]
        public async Task DeleteNotifications([FromBody] UserAccountDTO user)
        {
            await _notifService.DeleteNotificationsAsync(user.Id);
            await Response.WriteAsync(JsonConvert.SerializeObject(null, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpPost("deleteNotification")]
        public async Task DeleteNotification([FromBody] NotificationDTO notification)
        {
            await _hubContext.Clients.All.SendAsync("deleteNotification", notification.UserId);
            await _notifService.DeleteNotificationAsync(notification.Id);
            var dto = await _notifService.GetNotificationsAsync(notification.UserId);
            await Response.WriteAsync(JsonConvert.SerializeObject(dto, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
    }
}