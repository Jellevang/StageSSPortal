using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Klant
    {
        public int KlantId { get; set; }
        [Required]
        [MinLength(3)]
        public string Naam { get; set; }
        [Required]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Geen emailadres gegeven")]
       // [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public bool IsGeblokkeerd { get; set; }
        public bool IsKlantAccount { get; set; }
        public Klant HoofdKlant { get; set; }
    }
}
