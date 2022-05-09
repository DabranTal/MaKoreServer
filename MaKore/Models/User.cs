using System.ComponentModel.DataAnnotations;

namespace MaKore.Models
{
    public class User
    {
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }    
        public string NickName { get; set; }
        public List<Conversation> ConversationList { get; set; }  
        
    }
}
