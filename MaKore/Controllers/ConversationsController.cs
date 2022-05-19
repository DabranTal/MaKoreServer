#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MaKore.JsonClasses;
using System.Net;

namespace MaKore.Controllers
{
    [ApiController]
    [Route("api")]
    public class ConversationsController : BaseController
    {
        private readonly MaKoreContext _context;
        public IConfiguration _configuration;

        public ConversationsController(MaKoreContext context, IConfiguration config)
        {
            _context = context;
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
                var q = from conversations in _context.Conversations
                        where conversations.RemoteUser.UserName == id && conversations.User.UserName == userName
                        select conversations.RemoteUser;

                if (q.Any())
                {
                    RemoteUser remoteUser = q.First();
                    Message lastMessage = getLastMessage(remoteUser, userName);
                    string content;
                    string time;

                    if (lastMessage != null)
                    {
                        content = lastMessage.getContent();
                        time = lastMessage.Time;
                    }
                    else
                    {
                        content = "";
                        time = "";
                    }

                    return Json(new JsonUser()
                    {
                        Id = remoteUser.UserName,
                        Name = remoteUser.NickName,
                        Server = remoteUser.Server,
                        LastDate = time,
                        Last = content
                    });
                }
                else { return BadRequest(); }
            }

            // we didn't get a friend's id. we return all of the user's friends
            var qu = from conversations in _context.Conversations
                     where conversations.User.UserName == userName
                     select conversations.RemoteUser;

            List<JsonUser> friends = new List<JsonUser>();

            // go over the user's friends
            foreach (var r in qu)
            {
                RemoteUser ru = r;
                Message lm = getLastMessage(ru, userName);
                string c;
                string time;

                if (lm != null)
                {
                    c = lm.getContent();
                    time = lm.Time;
                }
                else
                {
                    c = "";
                    time = "";
                }


                friends.Add(
                    new JsonUser()
                    {
                        Id = r.UserName,
                        Name = r.NickName,
                        Server = r.Server,
                        LastDate = time,
                        Last = c
                    });
            }

            return Json(friends);
        }


        // GET : /me
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            var q = from user in _context.Users where user.UserName == userName select user;

            if (q.Any())
            {
                User u = q.First();
                return Json(new JsonUser()
                {
                    Id = u.UserName,
                    Name = u.NickName,
                    Server = "localhost:5018",
                    LastDate = "",
                    Last = ""
                });
            }
            return BadRequest();
        }

        // POST: /addConversation
        [HttpPost("addConversation")]
        public async Task<IActionResult> AddConversation([Bind("UserName, NickName, Server")] RemoteUser remoteUser)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            var q = from user in _context.Users where user.UserName == userName select user;

            if (q.Any())
            {
                User u = q.First();
                Conversation conv = new Conversation() { Messages = new List<Message>(), User = u, RemoteUser = remoteUser };
                _context.Add(conv);
                remoteUser.Conversation = conv;
                remoteUser.ConversationId = conv.Id;
                _context.RemoteUsers.Add(remoteUser);

                var q2 = from user in _context.Users where user.UserName == remoteUser.UserName select user;

                // this is our user !
                if (q2.Any())
                {
                    User u1 = q2.First();
                    RemoteUser ru = new RemoteUser()
                    {
                        UserName = userName,
                        NickName = u.NickName,
                        Server = "localhost:5018"
                    };

                    Conversation conv1 = new Conversation() { Messages = new List<Message>(), User = u1, RemoteUser = ru };
                    _context.Add(conv1);
                    ru.Conversation = conv1;
                    ru.ConversationId = conv1.Id;
                    _context.RemoteUsers.Add(ru);
                } else
                {
                    // this is not our user. we need to invite his server
                    string URL = "http://" + remoteUser.Server + "/api/invitations";

                    JsonInvitations inv = new JsonInvitations()
                    {
                        Server = "localhost:5018",
                        From = userName,
                        To = remoteUser.UserName,
                    };

                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HtmlResult = wc.UploadString(URL, inv.ToString());
                    }
                }
                await _context.SaveChangesAsync();
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
                var q = from remote in _context.RemoteUsers
                        where remote.UserName == contact && remote.Server == ru.Server
                        select remote;

                if (q.Any())
                {
                    // each one of our remote users with this new id (which are the same person) need to be changed
                    foreach (var r in q)
                    {
                        r.Server = ru.Server;
                        r.NickName = ru.NickName;
                        r.UserName = ru.UserName;
                    }
                    return NoContent();            //204
                }
            }
            return BadRequest();
        }


        // DELETE: /contacts/id
        [HttpDelete("contacts/{id}")]
        public async Task<IActionResult> Delete(string contact)
        {
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            string userName = UserNameFromJWT(authHeader, _configuration);

            // WHAT CAN CHANGE ?!?!?! WHAT STAYES THE SAME ?!?!?!
            if (ModelState.IsValid)
            {
                var q = from conversations in _context.Conversations
                        where conversations.RemoteUser.UserName == contact && conversations.User.UserName == userName
                        select conversations.RemoteUser;

                if (q.Any())
                {
                    _context.Remove(q.First());
                    _context.SaveChanges();
                    return NoContent();
                }
            }
            return BadRequest();
        }

        private Message getLastMessage(RemoteUser ru, string name)
        {
            var q = from conv in _context.Conversations.Include(m => m.Messages)
                    where conv.User.UserName == name && conv.RemoteUser == ru
                    select conv;

            if (q.Any())
            {
                Conversation c = q.First();
                if ((c != null) && (c.Messages.Count != 0))
                    return c.Messages.OrderByDescending(m => m.Id).FirstOrDefault();
            }
            return null;
        }
    }
}