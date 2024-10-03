using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        // Navigation
        public Booking Booking { get; set; }
        public User User { get; set; }
    }
}
