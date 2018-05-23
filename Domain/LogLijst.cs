using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class LogLijst
    {
        public int LogLijstId { get; set; }
        public String Naam { get; set; }
        public int GebruikerId { get; set; }
        public string OvmId { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
