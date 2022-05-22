
using MaKore.Models;

namespace MaKore.Services
{
    public class ConversationService : IConversationService
    {
        private readonly MaKoreContext _context;


        public ConversationService (MaKoreContext context)
        {
            _context = context;
        }

        public bool Create(string currUser, RemoteUser remoteUser)
        {

            var q = from user in _context.Users where user.UserName == currUser select user;

            if (q.Any())
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
                return true;
            }
            return false;
        }
    }
}
