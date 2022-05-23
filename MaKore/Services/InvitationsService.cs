
using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.EntityFrameworkCore;

namespace MaKore.Services
{
    public class InvitationsService : IInvitationsService
    {
        private readonly MaKoreContext _context;


        public InvitationsService(MaKoreContext context)
        {
            _context = context;
        }


        // another server wants to communicate with our user
        public bool CreateOnInvitation(JsonInvitation info)
        {
            // first we find our user the "other" wanted to talk to
            var q = from user in _context.Users
                    where user.UserName == info.To
                    select user;

            if (q.Any())
            {
                User u = q.First();

                // add him as a remote user
                RemoteUser ru = new RemoteUser()
                {
                    UserName = info.From,
                    NickName = info.From,
                    Server = info.Server,
                    Conversation = null
                };
                _context.Add(ru);

                // add a new conversation between our user to the other user
                Conversation conv = new Conversation()
                {
                    RemoteUser = ru,
                    Messages = new List<Message>(),
                    User = u,
                };
                _context.Add(conv);

                ru.Conversation = conv;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
