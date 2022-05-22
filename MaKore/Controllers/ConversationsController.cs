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
        public IUserService _serviceU;
        public IConversationService _serviceC;
        public IConfiguration _configuration;

        public ConversationsController(MaKoreContext context, IConfiguration config)
        {
            _serviceU = new UserService(context);
            _serviceC = new ConversationService(context);
            _configuration = config;
        }


        // GET: /contacts + /contacts/:id
        [HttpGet("contacts/{id?}")]
        public async Task<IActionResult> GetContacts(string? id)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            // if we got a friend's id we return only it's details
            if ((id != null) && (userName != null))
            {
                JsonUser one = _serviceU.Get(userName, id);

                if (one != null)
                {
                    return Json(one);
                }
                return NotFound();
            }


            // we didn't get a friend's id. we return all of the user's friends
            List<JsonUser> friends = _serviceU.GetContacts(userName);

            if (friends != null)
            {
                return Json(friends);
            }
            return BadRequest();
        }


        // GET : /me
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            JsonUser u = _serviceU.Get(userName);
            if (u != null)
            {
                return Json(u);
            }
            return BadRequest();
        }


        // POST: /addConversation
        [HttpPost("contacts")]
        public async Task<IActionResult> Contacts([Bind("id, name, server")] JsonRemoteUser remoteUser)
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
            if (_serviceC.Create(userName, remoteUser) == true)
            {
                return StatusCode(201);
            }
            return BadRequest();
        }


        // PUT: /contacts/id
        [HttpPut("contacts/{id}")]
        public async Task<IActionResult> Put(string contact, [Bind("UserName, NickName, Server")] RemoteUser ru)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            // WHAT CAN CHANGE ?!?!?! WHAT STAYES THE SAME ?!?!?!
            if (ModelState.IsValid)
            { 
                if (_serviceU.Edit(contact, ru) == true)
                {
                    return StatusCode(201);
                }
            }
            return BadRequest();
        }


        // DELETE: /contacts/id
        [HttpDelete("contacts/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            if (_serviceU.Delete(userName, id) == true)
            {
                return StatusCode(201);
            }
            return BadRequest();

        }
    }
}