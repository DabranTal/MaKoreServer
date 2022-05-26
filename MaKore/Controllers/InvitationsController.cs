#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using MaKore.JsonClasses;
using MaKore.Services;
using Microsoft.AspNetCore.SignalR;
using MaKore.Hubs;

namespace MaKore.Controllers
{

    [ApiController]
    [Route("api")]
    public class InvitationsController : BaseController
    {
        public IInvitationsService _service;
        public IConfiguration _configuration;
        public IHubContext<MessagesHub> _hub;

        public InvitationsController(MaKoreContext context, IConfiguration config, IHubContext<MessagesHub> hub)
        {
            _service = new InvitationsService(context);
            _configuration = config;
            _hub = hub;
        }

        public async Task sendFriend(JsonHubChat immediateChat) {
            await _hub.Clients.Group(immediateChat.userName).SendAsync("ReciveFriend", immediateChat.remoteUserName, immediateChat.nickName, immediateChat.userName);
        }

        // a different server serfs here, thus no authantication. Someone wants to talk to OUR user
        [HttpPost("invitations")]
        public async Task<IActionResult> Invitations([Bind("From,To,Server")] JsonInvitation info)
        {
            if (_service.CreateOnInvitation(info) == true)
            {
                JsonHubChat jsc = new JsonHubChat()
                {
                    userName = info.To,
                    remoteUserName = info.From,
                    nickName = info.To
                };
                await sendFriend(jsc);
                return StatusCode(201);
            }
            return BadRequest();
        }
    }
}