using System.ComponentModel.DataAnnotations;

namespace BussinessObjects
{
    public class AirPlane
    {
        [Key]
        public int PlaneId { get; set; }

        [Required(ErrorMessage = "Plane name is required!")]
        public string PlaneName { get; set; }

        [Required(ErrorMessage = "VIP seat number is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "VIP seat number must be a positive integer!")]
        public int VipSeatNumber { get; set; }

        [Required(ErrorMessage = "Normal seat number is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Normal seat number must be a positive integer!")]
        public int NormalSeatNumber { get; set; }

        // Navigation property
        public ICollection<Flight>? Flights { get; set; }
    }

}
