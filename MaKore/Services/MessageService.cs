using MaKore.JsonClasses;
using MaKore.Models;
using MaKore.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaKore.Services
{
    public class MessageService : IMessageService
    {
        private readonly MaKoreContext _context;


        public MessageService(MaKoreContext context)
        {
            _context = context;
        }

        public bool Create(string currUser, string id, JsonMessage message)
        {
            var q = from conv in _context.Conversations
                    where conv.User.UserName == currUser && conv.RemoteUser.UserName == id
                    select conv;

            string newContent;
            newContent = currUser + "æ" + message.Content;

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
                             where conv.User.UserName == id && conv.RemoteUser.UserName == currUser
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
                return true;
            }
            return false;
        }


        public bool Delete(int id)
        {
            var q = from mess in _context.Messages
                    where mess.Id == id
                    select mess;

            if (q.Any())
            {
                Message m = q.First();
                _context.Remove(m);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public bool Edit(int id, JsonMessage message)
        {

            var q = from mess in _context.Messages
                    where mess.Id == id
                    select mess;

            if (q.Any())
            {
                Message m = q.First();
                string sender = m.getSender();
                m.Content = sender + "æ" + message.Content;
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public JsonMessage Get(string currUser, int id)
        {
            var q = from message in _context.Messages
                    where message.Id == id
                    select message;
            if (q.Any())
            {
                Message mess = q.First();

                bool sent;

                if (mess.getSender() == currUser) { sent = true; } else { sent = false; }

                return new JsonMessage() { Content = mess.getContent(), Created = Message.getTime(), Id = mess.Id, Sent = sent };
            }
            return null;
        }


        public List<JsonMessage> GetAll(string currUser, string id)
        {

            var qu = from conversations in _context.Conversations
                     where conversations.User.UserName == currUser && conversations.RemoteUser.UserName == id
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

                    bool sent = false;
                    if (sender == currUser) { sent = true; }

                    messagesList.Add(new JsonMessage() { Content = content, Id = Id, Sent = sent, Created = time });
                }
                return messagesList;
            }
            return null;
        }
    }
}
