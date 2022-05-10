using System.ComponentModel.DataAnnotations;

namespace MaKore.Models
{
    public class User
    {
        [Key]
        public string? Name { get; set; }
        public string? Nickname { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
