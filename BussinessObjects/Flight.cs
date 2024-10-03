using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }
        public int PlaneId { get; set; }
        public int PilotId { get; set; }
        public string FlightNumber { get; set; }
        public int OriginID { get; set; }
        public int DestinationID { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BasePrice { get; set; }
        public string Status { get; set; }

        // Navigation
        public AirPlane Plane { get; set; }
        public Pilot Pilot { get; set; }
        public Location Origin { get; set; }
        public Location Destination { get; set; }
    }
}
