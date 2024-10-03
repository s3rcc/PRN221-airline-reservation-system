using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class User : IdentityUser
    {
        public string Gender { get; set; }
        public DateTime DoB { get; set; }
        public string CCCD { get; set; }

        public int TierId { get; set; }
        public int RoleId { get; set; }

        public Tier Tier { get; set; }
        public Role Role { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
