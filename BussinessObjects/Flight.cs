using System.ComponentModel.DataAnnotations;

namespace BussinessObjects
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Flight
    {
        [Key]
        public int FlightId { get; set; }

        [Required(ErrorMessage = "Plane ID is required!")]
        public int PlaneId { get; set; }

        [Required(ErrorMessage = "Pilot ID is required!")]
        public int PilotId { get; set; }

        [Required(ErrorMessage = "Flight number is required!")]
        [StringLength(10, ErrorMessage = "Flight number cannot exceed 10 characters!")]
        public string FlightNumber { get; set; }

        [Required(ErrorMessage = "Origin ID is required!")]
        public int OriginID { get; set; }

        [Required(ErrorMessage = "Destination ID is required!")]
        public int DestinationID { get; set; }

        [Required(ErrorMessage = "Departure date and time is required!")]
        public DateTime DepartureDateTime { get; set; }

        [Required(ErrorMessage = "Arrival date and time is required!")]
        public DateTime ArrivalDateTime { get; set; }

        [Required(ErrorMessage = "Base price is required!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than zero!")]
        public decimal BasePrice { get; set; }

        public bool Status { get; set; }

        // Navigation properties
        public virtual AirPlane? Plane { get; set; }
        public virtual Pilot? Pilot { get; set; }
        public virtual Location? Origin { get; set; }
        public virtual Location? Destination { get; set; }
    }
}
