using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WypozyczeniaAPI.Models
{
    public class Opis
    {
        public int Id { get; set; }

        public DateTime Data { get; set; 
        }
        public string Typ { get; set; }
        
        public string Tytul { get; set; }

        public string? Treść { get; set; }

        // klucz obcy pojazdu 
        public int SerwisId { get; set; }
        [ValidateNever]
        public virtual Serwis Serwis { get; set; }
    }
}
