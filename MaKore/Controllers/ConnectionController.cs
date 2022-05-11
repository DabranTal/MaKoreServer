#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace MaKore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController : Controller
    {
        private readonly MaKoreContext _context;
        public IConfiguration _configuration;

        public ConnectionController(MaKoreContext context, IConfiguration config)
        {
            _configuration = config;
            _context = context;
        }


        // GET: Users
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Users.ToListAsync());
        }

        [HttpPost]
        //public IActionResult Post(string username, string passowrd)
        public IActionResult Post([Bind("UserName,Passowrd")] User user)
        {

            //bool h = MaKore.Controllers.UsersController.IsExistUser(username, password);

            //validate username and password
            if (true)
            {
                string username = user.UserName;
                string password = user.Password;


                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
{
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub,DateTime.UtcNow.ToString()),
                    new Claim("UserName", username)
                };
                var token = new JwtSecurityToken(
                    _configuration["JWTParams:Issuer"],
                    _configuration["JWTParams:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    signingCredentials: mac);
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
