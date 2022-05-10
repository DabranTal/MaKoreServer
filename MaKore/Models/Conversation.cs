using System.ComponentModel.DataAnnotations;

namespace MaKore.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public List<Message> Messages { get; set; }
        public User User { get; set; }

        public int RemoteUserId { get; set; }
        public RemoteUser RemoteUser { get; set; }


        public int getNextId()
        {
            return Messages.Max(m => m.Id) + 1;
        }
    }
}
