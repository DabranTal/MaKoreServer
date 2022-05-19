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
                var q = from message in _context.Messages
                        where message.Id == id2
                        select message;
                if (q.Any())
                {
                    Message mess = q.First();

                    bool sent;

                    if (mess.getSender() == id) { sent = true; } else { sent = false; }

                    return Json(new JsonMessage() { Content = mess.getContent(), Created = Message.getTime(), Id = mess.Id, Sent = sent });
                }
                return BadRequest();
            }


            var qu = from conversations in _context.Conversations
                     where conversations.User.UserName == name && conversations.RemoteUser.UserName == id
                     select conversations.Messages.ToList();

            if (qu.Any())
            {
                List<Message> messages = qu.First();

                var messagesList = new List<JsonMessage>();

                foreach (Message message in messages)
                {
                    string sender = message.getSender();
                    string content = message.getContent();
                    string time = message.Time;
                    int Id = message.Id;

                    bool sent;
                    if (sender == name) { sent = true; } else { sent = false; }

                    messagesList.Add(new JsonMessage() { Content = content, Id = Id, Sent = sent, Created = time });
                }
                return Json(messagesList);
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

            var q = from conv in _context.Conversations
                    where conv.User.UserName == username && conv.RemoteUser.UserName == id
                    select conv;

            string newContent;
            newContent = username + "æ" + message.Content;

            string time = Message.getTime();

            if (q.Any())
            {
                Conversation conversation = q.First();
                Message newMessage = new Message()
                {
                    Content = newContent,
                    Time = time,
                    ConversationId = conversation.Id
                };

                if (conversation.Messages == null)
                {
                    conversation.Messages = new List<Message>();
                }

                conversation.Messages.Add(newMessage);
                _context.Add(newMessage);
                _context.SaveChanges();


                var q2 = from user in _context.Users
                         where user.UserName == id
                         select user;

                // the remote user is also our client (our user)
                if (q2.Any())
                {
                    var q3 = from conv in _context.Conversations
                             where conv.User.UserName == id && conv.RemoteUser.UserName == username
                             select conv;

                    if (q3.Any())
                    {
                        Conversation contraConversation = q3.First();
                        Message duplicatedMessage = new Message()
                        {
                            Content = newContent,
                            Time = time,
                            ConversationId = contraConversation.Id
                        };

                        if (contraConversation.Messages == null)
                        {
                            contraConversation.Messages = new List<Message>();
                        }

                        contraConversation.Messages.Add(duplicatedMessage);
                        _context.Add(duplicatedMessage);
                        _context.SaveChanges();
                    }
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


            var q = from mess in _context.Messages
                    where mess.Id == id2
                    select mess;

            if (q.Any())
            {
                Message m = q.First();
                string sender = m.getSender();
                m.Content = sender + "æ" + message.Content;
                _context.SaveChanges();
                // ????????
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

            var q = from mess in _context.Messages
                    where mess.Id == id2
                    select mess;

            if (q.Any())
            {
                Message m = q.First();
                _context.Remove(m);
                _context.SaveChanges();
                return NoContent();          //204
            }
            return BadRequest();



        }
    }
}