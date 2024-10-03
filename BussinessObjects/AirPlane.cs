using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class AirPlane
    {
        [Key]
        public int PlaneId { get; set; }
        public string PlaneName { get; set; }
        public int VipSeatNumber { get; set; }
        public int NormalSeatNumber { get; set; }

        // Navigation property
        public ICollection<Flight> Flights { get; set; }
    }
}
