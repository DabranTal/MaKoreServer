using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaKore.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public User User { get; set; }

        public List<Message> Messages { get; set; }

        public int RemoteUserId { get; set; }

        public RemoteUser RemoteUser { get; set; }
    }
}
