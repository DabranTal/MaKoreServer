#nullable disable
using Microsoft.AspNetCore.Mvc;
using MaKore.JsonClasses;
using MaKore.Services;

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

        [HttpGet("existingUser/{id}")]
        public async Task<IActionResult> existingUser(string id)
        {
            if (_service.IsLocalUser(id) == true)
            {
                return Ok();
            }
            return BadRequest();
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
    }
}