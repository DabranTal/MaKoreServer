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
    //[Route("api/contacts/{id}/messages")]
    [Route("api/contacts/{id}/[action]")]

    public class MessagesController : BaseController
    {
        private readonly MaKoreContext _context;
        public IConfiguration _configuration;

        public MessagesController(MaKoreContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
        }

        // GET: Messages
        //[HttpGet("{id2?}")]
        [HttpGet("{id2?}")]
        [ActionName("messages")]

        public async Task<IActionResult> GetAllMessages(string id, int id2)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string name = UserNameFromJWT(authHeader, _configuration);

            if (id2 != 0)
            {
                var q = from message in _context.Messages
                        where message.Id == id2
                        select message;
                Message mess = q.First();

                bool sent;

                if (mess.getSenderFromMessage() == id) { sent = true; } else { sent = false; }

                return Json(new JsonMessage() { Content = mess.getContentFromMessage(), Created = Message.getTime(), Id = mess.Id, Sent = sent });
            }
            //var messages = await _context.Messages.ToListAsync();
            var qu = from conversations in _context.Conversations
                     where conversations.User.UserName == name && conversations.RemoteUser.UserName == id
                     select conversations.Messages.ToList();
            List<Message> messages = qu.First();

            var messagesList = new List<JsonMessage>();

            foreach (Message message in messages)
            {
                string sender = message.getSenderFromMessage();
                string content = message.getContentFromMessage();
                string time = message.Time;
                int Id = message.Id;

                bool sent;
                // ????????????????????????????????
                if (sender == name) { sent = true; } else { sent = false; }

                messagesList.Add(new JsonMessage() { Content = content, Id = Id, Sent = sent, Created = time });
            }

            return Json(messagesList);
        }

        // POST: Messages
        [HttpPost, ActionName("messages")]
        public async Task<IActionResult> SetMessageContent(string id, [Bind("content")] string content)
        {
            string username = HttpContext.Session.GetString("username");

            var conversation = (Conversation)from conv in _context.Conversations
                                             where conv.User.UserName == username && conv.RemoteUser.UserName == id
                                             select conv;

            string newContent = username + ":" + content;

            conversation.Messages.Add(new Message() { Content = newContent, Id = conversation.getNextId(), Time = Message.getTime() });

            // ??????????????????????????????????????????
            return StatusCode(201);    // 201
        }

        // GET: Messages/Details/5
        [HttpPut, ActionName("messages")]
        public async Task<IActionResult> RemoveMessage(string id, int? id2, [Bind("content")] string content)
        {
            Message mess = (Message)from message in _context.Messages
                                    where message.Id == id2
                                    select message;
            _context.Messages.Remove(mess);
            _context.SaveChanges();
            return NoContent();    //204
        }
    }
}