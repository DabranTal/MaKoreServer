using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaKore.Models
{
    public class RemoteUser
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string Server { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
