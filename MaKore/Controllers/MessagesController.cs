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
    [Route("api/contacts/{id}/messages")]
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
        [HttpGet("{id2?}")]
        public async Task<IActionResult> GetMessages(string id, int id2)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);


            // if we got id2 we need to return the specific message with id: id2
            if (id2 != 0)
            {
                var q = from message in _context.Messages
                                        where message.Id == id2
                                        select message;

                if (q.Any())
                {
                    Message message = q.First();
                    bool sent;
                    string sender = message.getSenderFromMessage();

                    if (sender == id)
                    { 
                        sent = false;
                    } else if (sender == userName)
                    {
                        sent = true;
                    } else
                    {
                        return BadRequest();
                    }

                    return Json(new JsonMessage() { Content = message.getContentFromMessage(),
                        Created = Message.getTime(), Id = message.Id, Sent = sent });

                } else
                {
                    return BadRequest();
                }
            }


            // if we didn't get id2 we return all the messages between userName and id
            var qu = from conversations in _context.Conversations
                           where conversations.User.UserName == userName && conversations.RemoteUser.UserName == id
                           select conversations.Messages.ToList();

            if (qu.Any())
            {
                List<Message> messages = qu.First();
                var messagesList = new List<JsonMessage>();


                foreach (Message message in messages)
                {
                    string sender = message.getSenderFromMessage();
                    string content = message.getContentFromMessage();
                    string time = message.Time;
                    int Id = message.Id;

                    bool sent;
                    if (sender == id)
                    {
                        sent = false;
                    }
                    else if (sender == userName)
                    {
                        sent = true;
                    }
                    else
                    {
                        return BadRequest();
                    }

                    messagesList.Add(new JsonMessage() { Content = content, Id = Id, Sent = sent, Created = time });
                }

                return Json(messagesList);
            } 
            return BadRequest();            
        }




        // POST: Messages
        [HttpPost, ActionName("messages")]
        public async Task<IActionResult> SetMessageContent(string id, [Bind("Id, Content, Created, Sent")] JsonMessage jsonMessage)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string username = UserNameFromJWT(authHeader, _configuration);

            if (username == null)
            {
                return BadRequest();
            }

            var q = from conv in _context.Conversations
                    where conv.User.UserName == username && conv.RemoteUser.UserName == id
                    select conv;

            if (q.Any())
            {
                Conversation conversation = q.First();
                string content = jsonMessage.Sent + ":" + jsonMessage.Content;
                Message message = new Message() { Content = content, Time = jsonMessage.Created };
                conversation.Messages.Add(message);

                _context.Add(message);
                await _context.SaveChangesAsync();

                // ??????????????????????????????????????????
                return StatusCode(201);    // 201
            }
            return BadRequest(); 

        }

        // GET: Messages/Details/5
        [HttpPut, ActionName("messages")]
        public async Task<IActionResult> RemoveMessage(string id, int? id2)
        {
            var q = from message in _context.Messages
                    where message.Id == id2
                    select message;

            if (q.Any())
            {
                //Message message = q.First() as Message;
                Message message = q.First();
                _context.Messages.Remove(message);
                _context.SaveChanges();
                return NoContent();                //204
            } else
            {
                return BadRequest();
            }

        }
    }
}
