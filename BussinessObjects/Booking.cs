using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public int FlightId { get; set; }
        public int? ReturnFlightId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public bool Status { get; set; }
        public int AdultNum { get; set; }
        public int ChildNum { get; set; }
        public int BabyNum { get; set; }
        public string ClassType { get; set; }
        public string ReturnClassType { get; set; }
        // Navigation
        public User? User { get; set; }
        public Flight? Flight { get; set; }
        public Flight? ReturnFlight { get; set; }
        public Payment? Payment { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
    }
}
