#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using MaKore.JsonClasses;

namespace MaKore.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : BaseController
    {
        private readonly MaKoreContext _context;
        public IConfiguration _configuration;

        public UsersController(MaKoreContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            var q = from users in _context.Users
                    select users;

            List<JsonUser> sendUsers = new List<JsonUser>();

            if (q.Any())
            {
                foreach (var user in q)
                    sendUsers.Add(new JsonUser() { Id = user.UserName, Name = user.NickName, Server = "home", Last = "", LastDate = "" });
                return Json(sendUsers);
            }
            return BadRequest();
        }
    }
}