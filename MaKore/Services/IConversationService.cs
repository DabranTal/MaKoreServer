using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaKore.Services
{
    public interface IConversationService
    {
        public string Create(string currUser, RemoteUser remoteUser);

        public bool DoesConversationExist(string id, string id2);

        public void ConvWithRemote(string localUser, string remote);

        public RemoteUser CreateRemoteFromLocal(string username);

        public void ConvWithLocals(string localUser, string localUser2);

        public bool IsThereConv(string userName, string otherName, string server);
    }
}
