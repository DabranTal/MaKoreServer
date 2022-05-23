using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaKore.Services
{
    public interface IUserService
    {

        public List<JsonUser> GetAll();

        public List<JsonUser> GetContacts(string currUser);

        public JsonUser Get(string currUser, string username);

        public JsonUser Get(string currUser);

        public bool Create(string currUser, [Bind("UserName, NickName, Password, ConversationList")] User user);

        public bool Edit(string contact, RemoteUser ru);

        public bool Delete(string currUser, string contact);
        
        public bool IsLocalUser(string id);

        public bool IsRemoteUser(string id);
    }
}
