using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaKore.Models
{
    public class Conversation
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 1)]
        public User User { get; set; }

        public List<Message> Messages { get; set; }

        public int RemoteUserId { get; set; }

        public RemoteUser RemoteUser { get; set; }
    }
}
