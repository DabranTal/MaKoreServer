#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using MaKore.JsonClasses;
using MaKore.Services;

namespace MaKore.Controllers
{
    [ApiController]
    [Route("api")]
    public class ConversationsController : BaseController
    {
        public MaKoreContext _context;
        public IUserService _serviceU;
        public IConversationService _serviceC;
        public IConfiguration _configuration;

        public ConversationsController(MaKoreContext context, IConfiguration config)
        {
            _serviceU = new UserService(context);
            _serviceC = new ConversationService(context);
            _configuration = config;
            _context = context;
        }

        [HttpPost("contacts")]
        public async Task<IActionResult> Contacts([Bind("Id, Name, Server")] JsonRemoteUser remoteUser)
        {
            
            RemoteUser ru = new RemoteUser()
            {
                UserName = remoteUser.Id,
                NickName = remoteUser.Name,
                Server = remoteUser.Server
            };
            return AddConversation(ru);
        }


        public IActionResult AddConversation(RemoteUser remoteUser)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            // new conv between our user and another user
            string res = _serviceC.Create(userName, remoteUser);
            if (res == "true")
            {
                // triger firebase notification on reciever 
                var q = from fb in _context.FireBaseMap
                        where fb.UserName == remoteUser.UserName
                        select fb;
                foreach (var fb in q)
                {
                    notifyFireBase(fb.Token, "addConversation", userName + " added");

                }
                return StatusCode(201);
            }
            return BadRequest();
        }

        [HttpGet("validation/{otherName}/{server}")]
        public async Task<IActionResult> Status(string otherName, string server)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            bool isOurUser = _serviceU.IsLocalUser(otherName);
            bool doHaveConv = _serviceC.IsThereConv(userName, otherName, server);


            // check if the other user is also our AND the otherName is not the user itself AND they do not already have a conv
            if ((isOurUser == true) && (otherName != userName) && (doHaveConv == false))
            {
                // regular add = 1
                return Json(1);

            }
            else if ((isOurUser == true) && (otherName != userName) && (doHaveConv == true))
            {
                // they do have a conv already = 2
                return Json(2);

            }
            else if ((isOurUser == true) && (otherName == userName))
            {
                // try to add himself = 3
                return Json(3);
            }
            else if ((isOurUser == false) && (server == Consts.localHost))
            {
                // try to add a not existing user (in our server) - 4
                return Json(4);

            }
            else if ((isOurUser == false) && (server != Consts.localHost) && (doHaveConv == true))
            {
                // already have a conversation - 2
                return Json(2);   
               
            }
            else
            {
                // ((isOurUser == false) && (server != Consts.localHost) && (doHaveConv == false))
                // INVITATION don't have a conversation, add - 6
                return Json(6);
            }
        }


    }
}