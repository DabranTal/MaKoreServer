using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaKore.Services
{
    public interface IInvitationsService
    {
        public bool CreateOnInvitation(JsonInvitation info);
    }
}
