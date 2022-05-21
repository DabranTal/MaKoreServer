using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaKore.Services
{
    public interface IMessageService
    {

        public List<JsonMessage> GetAll(string currUser, string id);

        public JsonMessage Get(string currUser, int id);

        public bool Create(string currUser, string id, JsonMessage message);

        public bool Edit(int id, JsonMessage message);

        public bool Delete(int id);

    }
}
