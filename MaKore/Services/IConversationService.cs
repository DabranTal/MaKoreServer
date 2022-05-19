using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaKore.Services
{
    public interface IConversationService
    {
        public bool Create(string currUser, RemoteUser remoteUser);
    }
}
