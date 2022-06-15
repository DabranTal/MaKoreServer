#nullable disable
using Microsoft.AspNetCore.Mvc;
using MaKore.Models;
using MaKore.JsonClasses;
using MaKore.Services;

namespace MaKore.Controllers
{

    [ApiController]
    [Route("api/contacts/{id}/[action]")]

    public class MessagesController : BaseController
    {
        public IMessageService _service;
        public IConfiguration _configuration;
        public MaKoreContext _context;


        public MessagesController(MaKoreContext context, IConfiguration config)
        {
            _configuration = config;
            _context = context;
            _service = new MessageService(context);
        }

        // GET: contacts/{id}/messages/{id2?}
        [HttpGet("{id2?}")]
        [ActionName("messages")]
        public async Task<IActionResult> GetAllMessages(string id, int id2)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string name = UserNameFromJWT(authHeader, _configuration);

            if (id2 != 0)
            {
                JsonMessage m = _service.Get(id, id2);
                if (m != null)
                {
                    return Json(m);
                }
                return BadRequest();
            }

            List<JsonMessage> messages = _service.GetAll(name, id);
            if (messages != null)
            {
                return Json(messages);
            }
            return BadRequest();
        }


        // POST: contacts/{id}/messages
        [HttpPost]
        [ActionName("messages")]
        public async Task<IActionResult> AddMessage(string id, [Bind("Id, Content, Created, Sent")] JsonMessage message)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string username = UserNameFromJWT(authHeader, _configuration);

            if (_service.Create(username, id, message) == true)
            {
                // triger firebase notification on reciever 
                var q = from fb in _context.FireBaseMap
                        where fb.UserName == id
                        select fb;
                foreach (var fb in q)
                {
                    notifyFireBase(fb.Token, "New message from " + username, message.Content.ToString());

                }
                return StatusCode(201);
            }
            return BadRequest();
        }


        // PUT contacts/{id}/messages/{id2}
        [HttpPut("{id2}")]
        [ActionName("messages")]
        public async Task<IActionResult> EditMessage(string id, int id2, [Bind("Id, Content, Created, Sent")] JsonMessage message)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string name = UserNameFromJWT(authHeader, _configuration);

            if (_service.Edit(id2, message) == true)
            {
                return StatusCode(201);
            }
            return BadRequest();
        }


        // DELETE: contacts/{id}/messages/{id2}
        [HttpDelete("{id2}")]
        [ActionName("messages")]
        public async Task<IActionResult> RemoveMessage(string id, int id2)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string name = UserNameFromJWT(authHeader, _configuration);


            if (_service.Delete(id2) == true)
            {
                return NoContent();
            }
            return BadRequest();
        }


    
    }
}