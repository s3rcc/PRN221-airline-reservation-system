using System.ComponentModel.DataAnnotations;

namespace BussinessObjects
{
    public class Pilot
    {
        [Key]
        public int PilotId { get; set; }
        [Required(ErrorMessage = "Pilot name is required!")]
        public string PilotName { get; set; }
        public bool Status { get; set; }

        // Navigation property
        public ICollection<Flight>? Flights { get; set; }
    }
}
