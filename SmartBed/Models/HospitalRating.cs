using System.ComponentModel.DataAnnotations;

namespace SmartBed.Models
{
    public class HospitalRating
    {
        [Key]
        public int RatingId { get; set; }

        public int HospitalId { get; set; }

        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime RatingDate { get; set; }
    }
}
