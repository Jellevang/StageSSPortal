using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Klant
    {
        public int KlantId { get; set; }
        public string Naam { get; set; }
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Geen emailadres gegeven")]
        public string Email { get; set; }
        //public string Status { get; set; }
        public bool IsGeblokkeerd { get; set; }
    }
}
