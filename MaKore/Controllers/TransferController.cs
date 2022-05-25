using MaKore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaKore.JsonClasses;
using MaKore.Services;
using Microsoft.AspNetCore.SignalR;
using MaKore.Hubs;

namespace MaKore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        public MaKoreContext _context;
        public IConfiguration _configuration;
        public IUserService _serviceU;
        public IConversationService _serviceC;
        public IMessageService _serviceM;
        public IHubContext<MessagesHub> _hub;

        public TransferController(MaKoreContext context, IConfiguration config, IHubContext<MessagesHub> hub)
        {    
            _context = context;
            _configuration = config;
            _serviceU = new UserService(context);
            _serviceC = new ConversationService(context);
            _serviceM = new MessageService(context);
            _hub = hub;
        }

        public async Task sendMessage(JsonHub immediateMessage)
        {
            int id = 0;
            var q = from ru in _context.RemoteUsers
                    where ru.UserName == immediateMessage.remoteUserName
                    select ru;
            List<int> ruNumber = new List<int>();
            foreach (var ru in q)
            {
                ruNumber.Add(ru.Id);
            }
            for (int i = 0; i < ruNumber.Count; i++)
            {
                var p = from conv in _context.Conversations.Include(p => p.User)
                        where conv.RemoteUserId == ruNumber[i] && conv.User.UserName == immediateMessage.userName
                        select conv;

                if (p.Any())
                {
                    foreach (var conv in p)
                    {
                        id = conv.Id;
                    }
                }
            }

            string convId = id.ToString();
            await _hub.Clients.Group(convId).SendAsync("ReciveMessage", immediateMessage.message, immediateMessage.x, immediateMessage.userName);
        }


        [HttpPost]
        [ActionName("transfer")]
        public async Task<IActionResult> Transfer([Bind("From, To, Content")] JsonTransfer mpl)
        {

            // TO CHECK
            string username = mpl.From;
            string partner = mpl.To;

            // Make Sure conversation exist other case create her
            if ((_serviceU.IsRemoteUser(username) || _serviceU.IsLocalUser(username)) && (_serviceU.IsRemoteUser(partner) || _serviceU.IsLocalUser(partner)))
            {
                if (!(_serviceC.DoesConversationExist(username, partner) || _serviceC.DoesConversationExist(partner, username)))
                {
                    if (_serviceU.IsLocalUser(username))
                    {
                        if (_serviceU.IsLocalUser(partner))
                        {
                            _serviceC.ConvWithLocals(username, partner);
                        }
                        else
                        {
                            _serviceC.ConvWithRemote(username, partner);
                        }
                    }
                    else if (_serviceU.IsLocalUser(partner))
                    {
                        if (_serviceU.IsRemoteUser(username))
                        {
                            _serviceC.ConvWithRemote(partner, username);
                        }
                    }
                }
            }

            if (_serviceM.Transfer(username, partner, mpl) == true)
            {
                Random rand = new Random();
                var random = rand.NextDouble().ToString();
                JsonHub jh = new JsonHub() {
                    message = mpl.Content,
                    remoteUserName = username,
                    userName = partner,
                    x = random 
                };
                await sendMessage(jh);
                return StatusCode(201);
            }
            return BadRequest();
        }
    }
}