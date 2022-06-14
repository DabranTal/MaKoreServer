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
    public class TransferController : BaseController
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
            if (_serviceU.IsLocalUser(immediateMessage.userName))
            {
                // invoke the partner chat
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
                // invoke the sendder user chat

                // get the conversations where the sennder is not the remote user
                var q2 = from conv in _context.Conversations.Include(p=>p.User)
                        where conv.User.UserName == immediateMessage.userName
                        select conv;

                // in all the selected conversations if the remote user is the getter user take the conversation id
                foreach (var conv in q2)
                {
                    if (conv.RemoteUser.UserName == immediateMessage.remoteUserName)
                    {
                        await _hub.Clients.Group(conv.Id.ToString()).SendAsync("ReciveMessage", immediateMessage.message, immediateMessage.x, immediateMessage.userName);
                        break;
                    }
                }
            } else {
                // find all Local user conversation
                var q = from conv in _context.Conversations.Include(p => p.RemoteUser)
                        where conv.User.UserName == immediateMessage.remoteUserName
                        select conv;

                // find specific conversation with the "remote user"
               foreach (var conv in q)
                {
                    if (conv.RemoteUser.UserName == immediateMessage.userName)
                    {
                        id = conv.RemoteUser.Id;
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
            string from = mpl.From;
            string to = mpl.To;

            // Make Sure conversation exist other case create her
            if ((_serviceU.IsRemoteUser(from) || _serviceU.IsLocalUser(from)) && (_serviceU.IsRemoteUser(to) || _serviceU.IsLocalUser(to)))
            {
                if (!(_serviceC.DoesConversationExist(from, to) || _serviceC.DoesConversationExist(to, from)))
                {
                    if (_serviceU.IsLocalUser(from))
                    {
                        if (_serviceU.IsLocalUser(to))
                        {
                            _serviceC.ConvWithLocals(from, to);
                        }
                        else
                        {
                            _serviceC.ConvWithRemote(from, to);
                        }
                    }
                    else if (_serviceU.IsLocalUser(to))
                    {
                        if (_serviceU.IsRemoteUser(from))
                        {
                            _serviceC.ConvWithRemote(to, from);
                        }
                    }
                }
            }

            if (_serviceM.Transfer(from, to, mpl) == true)
            {
                Random rand = new Random();
                var random = rand.NextDouble().ToString();
                JsonHub jh = new JsonHub()
                {
                    message = mpl.Content,
                    remoteUserName = to,
                    userName = from,
                    x = random
                };
                await sendMessage(jh);
                string reciever = from;
                var q = from fb in _context.FireBaseMap
                        where fb.UserName == reciever
                        select fb;
                foreach (var fb in q)
                {
                    notifyFireBase(fb.Token);

                }
                return StatusCode(201);
            }
            return BadRequest();
        }
    }
}