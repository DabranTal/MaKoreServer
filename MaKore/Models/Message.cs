using System.ComponentModel.DataAnnotations;
namespace MaKore.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Time { get; set; }
    }
}
