using MaKore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaKore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly MaKoreContext _context;
        public IConfiguration _configuration;

        public TransferController(MaKoreContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
        }
        public class MyPayload
        {
            [Newtonsoft.Json.JsonProperty("from, to, content")]
            public string from { get; set; }
            public string to { get; set; }
            public string content { get; set; }

        }

        private bool isLocalUser(string id)
        {
            var q = from conv in _context.Users
                    where conv.UserName == id
                    select conv;
            if (q.Any())
            {
                return true;
            }
            return false;
        }

        private bool isRemoteUser(string id)
        {
            var q = from conv in _context.RemoteUsers
                    where conv.UserName == id
                    select conv;
            if (q.Any())
            {
                return true;
            }
            return false;
        }

        private bool doesConversationExist(string id, string id2)
        {
            var q = from conv in _context.Conversations.Include(p=>p.User).Include(t => t.RemoteUser)
                    where conv.User.UserName == id
                    select conv;
            var q2 = from remote in _context.RemoteUsers.Where(x => x.UserName == id2)
                     select remote;
            int partnerId = -1;
            foreach (var row in q)
            {
                foreach (var row2 in q2)
                {
                    if (row.User.UserName == id && row.RemoteUserId == row2.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private void convWithRemote(string localUser, string remote)
        {
            var q = from user in _context.Users where user.UserName == localUser select user;
            if (q.Any())
            {
                User user = q.First();
                var q2 = from userRemote in _context.RemoteUsers.Where(x => x.UserName == remote)
                         select userRemote;
                string server = "";
                string nickname = "";
                if (q2.Any())
                {
                    server = q2.First().Server;
                    nickname = q2.First().NickName;
                    RemoteUser ru = new RemoteUser()
                    {
                        UserName = remote,
                        NickName = nickname,
                        Server = server
                    };
                    Conversation conv1 = new Conversation() { Messages = new List<Message>(), User = user, RemoteUser = ru };
                    _context.Add(conv1);
                    ru.Conversation = conv1;
                    ru.ConversationId = conv1.Id;
                    _context.RemoteUsers.Add(ru);
                    _context.SaveChanges();

                }
            }
        }


        private RemoteUser createRemoteFromLocal(string username)
        {
            var q = from user in _context.Users where user.UserName == username select user;
            string server = "";
            string nickname = "";
            if (q.Any())
            {
                server = "localhost:3000";
                nickname = q.First().NickName;
                RemoteUser ru = new RemoteUser()
                {
                    UserName = username,
                    NickName = nickname,
                    Server = server
                };
                return ru;
            }
            return null;
        }

        private async void convWithLocals(string localUser, string localUser2)
        {
            var quser1 = from user in _context.Users where user.UserName == localUser select user;
            var quser2 = from user in _context.Users where user.UserName == localUser2 select user;

            if (quser1.Any())
            {
                User user1 = quser1.First();
                RemoteUser ru2 = createRemoteFromLocal(localUser2);
                Conversation conv1 = new Conversation() { Messages = new List<Message>(), User = user1, RemoteUser = ru2 };
                _context.Add(conv1);
                ru2.Conversation = conv1;
                ru2.ConversationId = conv1.Id;
                _context.RemoteUsers.Add(ru2);
                _context.SaveChanges();

            }
            if (quser2.Any())
            {
                User user2 = quser2.First();
                RemoteUser ru1 = createRemoteFromLocal(localUser);
                Conversation conv1 = new Conversation() { Messages = new List<Message>(), User = user2, RemoteUser = ru1 };
                _context.Add(conv1);
                ru1.Conversation = conv1;
                ru1.ConversationId = conv1.Id;
                _context.RemoteUsers.Add(ru1);
                _context.SaveChanges();
            }
             await _context.SaveChangesAsync();
    }




        [HttpPost]
        [ActionName("transfer")]
        public IActionResult Transfer([Bind("from, to, content")] MyPayload mpl)
        {
            string username = mpl.from;
            string partner = mpl.to;
            // Make Sure conversation exist other case create her
            if ((isRemoteUser(username) || isLocalUser(username)) && (isRemoteUser(partner) || isLocalUser(partner)))
            {
                if (!(doesConversationExist(username, partner) || doesConversationExist(partner, username)))
                {
                    if (isLocalUser(username))
                    {
                        if (isLocalUser(partner))
                        {
                            convWithLocals(username, partner);
                        }
                        else
                        {
                            convWithRemote(username, partner);
                        }
                    }
                    else if (isLocalUser(partner))
                    {
                        if (isRemoteUser(username))
                        {
                            convWithRemote(partner, username);
                        }
                    }

                }
            }
            var q = from conv in _context.Conversations
                    where conv.User.UserName == username && conv.RemoteUser.UserName == partner
                    select conv;
            // case the message is from remote user
            if (!(q.Any()))
            {
                q = from conv in _context.Conversations
                    where conv.User.UserName == partner && conv.RemoteUser.UserName == username
                    select conv;
            }

            string newContent;
            newContent = username + ":" + mpl.content;

            string time = Message.getTime();

            if (q.Any())
            {
                Conversation conversation = q.First();
                Message newMessage = new Message()
                {
                    Content = newContent,
                    Time = time,
                    ConversationId = conversation.Id
                };

                if (conversation.Messages == null)
                {
                    conversation.Messages = new List<Message>();
                }

                conversation.Messages.Add(newMessage);
                _context.Add(newMessage);
                _context.SaveChanges();


                var q2 = from user in _context.Users
                         where user.UserName == partner
                         select user;

                // the remote user is also our client (our user)
                if (q2.Any())
                {
                    var q3 = from conv in _context.Conversations
                             where conv.User.UserName == partner && conv.RemoteUser.UserName == username
                             select conv;

                    if (q3.Any())
                    {
                        Conversation contraConversation = q3.First();
                        Message duplicatedMessage = new Message()
                        {
                            Content = newContent,
                            Time = time,
                            ConversationId = contraConversation.Id
                        };

                        if (contraConversation.Messages == null)
                        {
                            contraConversation.Messages = new List<Message>();
                        }

                        contraConversation.Messages.Add(duplicatedMessage);
                        _context.Add(duplicatedMessage);
                        _context.SaveChanges();
                    }
                }
                return StatusCode(201);
            }
            return BadRequest();
        }
    }
}