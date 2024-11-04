using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }
        public string SeatNumber { get; set; }
        public string TicketType { get; set; }
        public string? CustomerName { get; set; }
        public DateTime IssuedDate { get; set; }
        public decimal Carryluggage { get; set; }
        public decimal Baggage { get; set; }
        public string ClassType { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}
