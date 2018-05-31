using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ScheduledDownTime
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string OvmId { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime Eind { get; set; }
    }
}
