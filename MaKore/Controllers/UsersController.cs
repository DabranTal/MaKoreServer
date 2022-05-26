#nullable disable
using Microsoft.AspNetCore.Mvc;
using MaKore.JsonClasses;
using MaKore.Services;
using MaKore.Models;

namespace MaKore.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : BaseController
    {
        public IConfiguration _configuration;
        public IUserService _service;

        public UsersController(MaKoreContext context, IConfiguration config)
        {
            _configuration = config;
            _service = new UserService(context);
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            List<JsonUser> users = _service.GetAll();

            if (users != null)
            {
                return Json(users);
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

            JsonUser u = _service.Get(userName);
            if (u != null)
            {
                return Json(u);
            }
            return BadRequest();
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
                JsonUser one = _service.Get(userName, id);

                if (one != null)
                {
                    return Json(one);
                }
                return NotFound();
            }


            // we didn't get a friend's id. we return all of the user's friends
            List<JsonUser> friends = _service.GetContacts(userName);

            if (friends != null)
            {
                return Json(friends);
            }
            return BadRequest();
        }


        // PUT: /contacts/id
        [HttpPut("contacts/{id}")]
        public async Task<IActionResult> Put(string id, [Bind("name, server")] jsonContactsPut pl)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            RemoteUser ru = new RemoteUser()
            {
                
                UserName = id,
                NickName = pl.name,
                Server = pl.server
            };

            // WHAT CAN CHANGE ?!?!?! WHAT STAYES THE SAME ?!?!?!
            if (ModelState.IsValid)
            {
                if (_service.Edit(id, ru) == true)
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

            if (_service.Delete(userName, id) == true)
            {
                return StatusCode(204);
            }
            return BadRequest();

        }
    }
}