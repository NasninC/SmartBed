using System.ComponentModel.DataAnnotations;

namespace SmartBed.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public int HospitalId { get; set; }

        [Required]
        public string PatientName { get; set; }

        [Required]
        public int PatientAge { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string BedType { get; set; }

        public DateTime BookingDate { get; set; }

        public string Status { get; set; }
    }
}
