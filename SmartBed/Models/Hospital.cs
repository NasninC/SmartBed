using System.ComponentModel.DataAnnotations;

namespace SmartBed.Models
{
    public class Hospital
    {
        [Key]
        public int HospitalId { get; set; }

        [Required]
        public string HospitalName { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public int ICUBeds { get; set; }

        public int EmergencyBeds { get; set; }

        public int GeneralBeds { get; set; }

        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        // Hospital verification by DMO/Admin
        public string VerificationStatus { get; set; } = "Pending";
    }
}
