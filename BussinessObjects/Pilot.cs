using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects
{
    public class Pilot
    {
        [Key]
        public int PilotId { get; set; }
        public string PilotName { get; set; }
        public string Status { get; set; }
    }
}
