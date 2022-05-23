

using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MaKore.Hubs
{
    public class MessagesHub : Hub
    {
        private readonly MaKoreContext _context;
        private int groupId;

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
                }
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

        public async Task registerToAllGrouop(JsonHubChat register)
        {
            var q = from u in _context.Users
                    where u.UserName != register.userName
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

