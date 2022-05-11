using System.ComponentModel.DataAnnotations;

namespace MaKore.Models
{
    public class Rating
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1,5)]
        public int Grade { get; set; }

        public string Feedback { get; set; }

        public DateTime Date { get; set; }
    }
}
