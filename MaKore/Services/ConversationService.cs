
using MaKore.Models;
using Microsoft.EntityFrameworkCore;

namespace MaKore.Services
{
    public class ConversationService : IConversationService
    {
        private readonly MaKoreContext _context;


        public ConversationService (MaKoreContext context)
        {
            _context = context;
        }

        public string Create(string currUser, RemoteUser remoteUser)
        {

            var q = from user in _context.Users where user.UserName == currUser select user;

            var q1 = from conver in _context.Conversations
                     where conver.User.UserName == currUser && conver.RemoteUser.UserName == remoteUser.UserName && conver.RemoteUser.Server == remoteUser.Server
                     select conver;



            // check the user exists AND he doesnt have a conv with the ru already AND he doesn't add himself
            if ((q.Any()) && (!q1.Any()) && ((currUser != remoteUser.UserName) && (remoteUser.Server == Consts.localHost)))
            {
                User u = q.First();
                Conversation conv = new Conversation() { Messages = new List<Message>(), User = u, RemoteUser = remoteUser };
                _context.Add(conv);
                remoteUser.Conversation = conv;
                remoteUser.ConversationId = conv.Id;
                _context.RemoteUsers.Add(remoteUser);

                // check if the other user is also ours
                var q2 = from user in _context.Users
                         where user.UserName == remoteUser.UserName && remoteUser.Server == "localhost:5018"
                         select user;

                if (q2.Any())
                {
                    User u1 = q2.First();
                    RemoteUser ru = new RemoteUser()
                    {
                        UserName = currUser,
                        NickName = u.NickName,
                        Server = Consts.localHost
                    };

                    Conversation conv1 = new Conversation() { Messages = new List<Message>(), User = u1, RemoteUser = ru };
                    _context.Add(conv1);
                    ru.Conversation = conv1;
                    ru.ConversationId = conv1.Id;
                    _context.RemoteUsers.Add(ru);
                }
                _context.SaveChanges();


                return "true";

                // he added an existing friend
            } else if (q1.Any())
            {
                return "alreadyExists";
                // he added himself
            } else if ((currUser == remoteUser.UserName) && (remoteUser.Server == Consts.localHost))
            {
                return "yourself";
            }
            return "false";
        }

        public bool DoesConversationExist(string id, string id2)
        {
            var q = from conv in _context.Conversations.Include(p => p.User).Include(t => t.RemoteUser)
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


        public void ConvWithRemote(string localUser, string remote)
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


        public RemoteUser CreateRemoteFromLocal(string username)
        {
            var q = from user in _context.Users where user.UserName == username select user;
            string server = "";
            string nickname = "";
            if (q.Any())
            {
                server = "localhost:5018";
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

        public async void ConvWithLocals(string localUser, string localUser2)
        {
            var quser1 = from user in _context.Users where user.UserName == localUser select user;
            var quser2 = from user in _context.Users where user.UserName == localUser2 select user;

            if (quser1.Any())
            {
                User user1 = quser1.First();
                RemoteUser ru2 = CreateRemoteFromLocal(localUser2);
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
                RemoteUser ru1 = CreateRemoteFromLocal(localUser);
                Conversation conv1 = new Conversation() { Messages = new List<Message>(), User = user2, RemoteUser = ru1 };
                _context.Add(conv1);
                ru1.Conversation = conv1;
                ru1.ConversationId = conv1.Id;
                _context.RemoteUsers.Add(ru1);
                _context.SaveChanges();
            }
            await _context.SaveChangesAsync();
        }

    }
}
