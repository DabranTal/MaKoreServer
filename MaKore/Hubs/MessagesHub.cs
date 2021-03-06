

using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MaKore.Hubs
{
    public class MessagesHub : Hub
    {
        private readonly MaKoreContext _context;

        public MessagesHub(MaKoreContext context)
        {
            _context = context;
        }

        public async Task registerToListener(JsonHubRegister userName)
        {
            var q = from ru in _context.RemoteUsers
                    where ru.UserName == userName.userName
                    select ru;
            List<int> ruNumber = new List<int>();
            foreach (var ru in q)
            {
                ruNumber.Add(ru.Id);
            }
            for (int i = 0; i < ruNumber.Count; i++)
            {
                var p = from conv in _context.Conversations
                        where conv.RemoteUserId == ruNumber[i]
                        select conv;
                int id = 0;
                if (p.Any())
                {
                    foreach (var conv in p)
                    {
                        id = conv.Id;
                    }
                    string convId = id.ToString();
                    await Groups.AddToGroupAsync(Context.ConnectionId, convId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, ruNumber[i].ToString());
                }
            }
            // find all Local user conversation
            var q2 = from conv in _context.Conversations.Include(p => p.RemoteUser)
                    where conv.User.UserName == userName.userName
                     select conv;

            // find specific conversation with the "remote user"
            foreach (var conv in q2)
            {
               await Groups.AddToGroupAsync(Context.ConnectionId, conv.RemoteUser.Id.ToString());
            }

            // register User to all his conversation

            foreach (var conv in q2)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conv.Id.ToString());
            }
        }


        public async Task immediateSeenMessage(JsonHub immediateMessage)
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
            await Clients.Group(convId).SendAsync("ReciveMessage", immediateMessage.message, immediateMessage.x, immediateMessage.userName);
        }

        public async Task registerToAllGroup(JsonHubChat register)
        {
            var q = from u in _context.Users
                    select u;
            if (q.Any())
            {

                foreach (var u in q)
                {
                    string id = "";
                    id = u.UserName;
                    await Groups.AddToGroupAsync(Context.ConnectionId, id);
                }
            }
        }

        public async Task addRegister(string remote)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, remote);
        }

        public async Task newRegister(JsonHubChat newRegister)
        {
            await Clients.All.SendAsync("newRegisterUser", newRegister.userName);
        }

        public async Task immediateSennFriend(JsonHubChat immediateChat)
        {
            await Clients.Group(immediateChat.userName).SendAsync("ReciveFriend", immediateChat.userName, immediateChat.nickName, immediateChat.remoteUserName);
        }
    }
}

