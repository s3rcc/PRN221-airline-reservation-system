using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class Tier
    {
        [Key]
        public int TierId { get; set; }
        public string TierName { get; set; }
        public int PriorityLevel { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
