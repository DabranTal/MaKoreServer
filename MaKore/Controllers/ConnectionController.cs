#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using MaKore.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MaKore.JsonClasses;

namespace MaKore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ConnectionController : BaseController
    {
        private readonly MaKoreContext _context;
        public IConfiguration _configuration;



        public ConnectionController(MaKoreContext context, IConfiguration config)
        {
            _configuration = config;
            _context = context;
        }


        // GET: Username
        [HttpGet]
        [ActionName("getusername")]
        public async Task<IActionResult> Index()
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string UserName = UserNameFromJWT(authHeader, _configuration);
            if (UserName == null)
            {
                return NotFound();
            }
            else
            {
                return Json(UserName);
            }
        }

        private bool IsExistUser(string username, string password)
        {
            var isRegistered = _context.Users.Where(m => m.UserName == username && m.Password == password);
            if (isRegistered.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private IActionResult GetJwtToken(string username)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
            var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub,DateTime.UtcNow.ToString()),
                    new Claim("NameIdentifier", username)
                };
            var token = new JwtSecurityToken(
                _configuration["JWTParams:Issuer"],
                _configuration["JWTParams:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: mac);
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));

        }

        [HttpPost]
        [ActionName("login")]
        //public IActionResult Post(string username, string passowrd)
        public IActionResult Login([Bind("UserName,Passowrd")] User user)
        {

            string username = user.UserName;
            string password = user.Password;

            //validate username and password
            if (IsExistUser(username, password))
            {
                return GetJwtToken(username);
            }
            else
            {
                return BadRequest();
            }
        }




        [HttpPost]
        [ActionName("register")]
        //public IActionResult Post(string username, string passowrd)
        public async Task<IActionResult> Register([Bind("UserName,NickName, Passowrd")] User user)
        {
            if (ModelState.IsValid)
            {
                var isTakenUserName = from userName in _context.Users.Where(m => m.UserName == user.UserName) select userName;
                if (isTakenUserName.Any())
                {
                    return BadRequest();
                }
                else
                {
                    user.ConversationList = new List<Conversation>();
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    string username = user.UserName;
                    return GetJwtToken(username);

                }
            }
            return View(user);
        }
    }
}
