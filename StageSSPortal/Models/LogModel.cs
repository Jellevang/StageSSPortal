using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageSSPortal.Models
{
    public class LogModel
    {
        public string Naam { get; set; }
        public DateTime ActionDate { get; set; }
        public string Gebruiker { get; set; }
        public string Ovm { get; set; }
    }
}