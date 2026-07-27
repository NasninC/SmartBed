using System.ComponentModel.DataAnnotations;

namespace SmartBed.Models
{
    public class Hospital
    {
        [Key]
        public int HospitalId { get; set; }

        [Required]
        public string HospitalName { get; set; }

        [Required]
        public string Location { get; set; }

        public int ICUBeds { get; set; }

        public int EmergencyBeds { get; set; }

        public int GeneralBeds { get; set; }

        public string ContactNumber { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}