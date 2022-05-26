using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaKore.Services
{
    public class UserService : IUserService
    {
        private readonly MaKoreContext _context;

        public UserService(MaKoreContext context)
        {
            _context = context;
        }

        // ???? we need this ??
        public bool Create(string currUser, [Bind(new[] { "UserName, NickName, Password, ConversationList" })] User user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string currUser, string contact)
        {
            var q = from conversations in _context.Conversations
                    where conversations.RemoteUser.UserName == contact && conversations.User.UserName == currUser
                    select conversations.RemoteUser;

            if (q.Any())
            {
                _context.Remove(q.First());
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public bool Edit(string contact, RemoteUser ru)
        {
            var q = from remote in _context.RemoteUsers
                    where remote.UserName == contact
                    select remote;

            if (q.Any())
            {
                // each one of our remote users with this new id (which are the same person) need to be changed
                foreach (var r in q)
                {
                    r.Server = ru.Server;
                    r.NickName = ru.NickName;
                    r.UserName = ru.UserName;
                    _context.Update(r);
                }
                _context.SaveChanges();
                return true;            //204
            }
            return false;
        }


        public JsonUser Get(string currUser, string username)
        {
            var q = from conversations in _context.Conversations
                    where conversations.RemoteUser.UserName == username && conversations.User.UserName == currUser
                    select conversations.RemoteUser;

            if (q.Any())
            {
                RemoteUser remoteUser = q.First();
                Message lastMessage = getLastMessage(remoteUser, currUser);
                string content;
                string time;

                if (lastMessage != null)
                {
                    content = lastMessage.getContent();
                    time = lastMessage.Time;
                }
                else
                {
                    content = null;
                    time = null;
                }

                return new JsonUser()
                {
                    Id = remoteUser.UserName,
                    Name = remoteUser.NickName,
                    Server = remoteUser.Server,
                    Lastdate = time,
                    Last = content
                };
            }
            else { return null; }
        }


        public List<JsonUser> GetAll()
        {
            var q = from users in _context.Users
                    select users;

            List<JsonUser> sendUsers = new List<JsonUser>();

            if (q.Any())
            {
                foreach (var user in q)
                    sendUsers.Add(new JsonUser()
                    {
                        Id = user.UserName,
                        Name = user.NickName,
                        Server = Consts.localHost,
                        Last = null,
                        Lastdate = null
                    });

                return sendUsers;
            }
            return null;
        }


        public List<JsonUser> GetContacts(string currUser)
        {

            var qu = from conversations in _context.Conversations
                     where conversations.User.UserName == currUser
                     select conversations.RemoteUser;

            List<JsonUser> friends = new List<JsonUser>();

            // go over the user's friends
            foreach (var r in qu)
            {
                RemoteUser ru = r;
                Message lm = getLastMessage(ru, currUser);
                string c;
                string time;

                if (lm != null)
                {
                    c = lm.getContent();
                    time = lm.Time;
                }
                else
                {
                    c = null;
                    time = null;
                }


                friends.Add(
                    new JsonUser()
                    {
                        Id = r.UserName,
                        Name = r.NickName,
                        Server = r.Server,
                        Lastdate = time,
                        Last = c
                    });
            }
            return friends;
        }


        public JsonUser Get(string username)
        {
            var q = from user in _context.Users where user.UserName == username select user;

            if (q.Any())
            {
                User u = q.First();
                return new JsonUser()
                {
                    Id = u.UserName,
                    Name = u.NickName,
                    Server = Consts.localHost,
                    Lastdate = null,
                    Last = null
                };
            }
            return null;
        }


        private Message getLastMessage(RemoteUser ru, string name)
        {
            var q = from conv in _context.Conversations.Include(m => m.Messages)
                    where conv.User.UserName == name && conv.RemoteUser == ru
                    select conv;

            if (q.Any())
            {
                Conversation c = q.First();
                if ((c != null) && (c.Messages.Count != 0))
                    return c.Messages.OrderByDescending(m => m.Id).FirstOrDefault();
            }
            return null;
        }


        public bool IsLocalUser(string id)
        {
            var q = from u in _context.Users
                    where u.UserName == id
                    select u;

            if (q.Any())
            {
                return true;
            }
            return false;
        }


        public bool IsRemoteUser(string id)
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
    }
}
