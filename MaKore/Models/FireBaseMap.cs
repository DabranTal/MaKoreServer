using System.ComponentModel.DataAnnotations;

namespace MaKore.Models
{
    public class FireBaseMap
    {
        [Key]
        public string UserName { get; set; }

        public string Token { get; set; }

        public string Key { get; set; }

    }
}
