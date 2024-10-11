using System.ComponentModel.DataAnnotations;

namespace BussinessObjects
{
    public class Location
    {
        [Key]
        public int LocationID { get; set; }
        [Required(ErrorMessage = "Location name is required!")]
        public string LocationName { get; set; }

        // Navigation property
        public ICollection<Flight>? OriginFlights { get; set; }
        public ICollection<Flight>? DestinationFlights { get; set; }
    }
}
