using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WypozyczeniaAPI.Models
{
    public class Pozycja
    {
        public int Id { get; set; }
        public float NS { get; set; }
        public float WE { get; set; }
        public DateTime Data { get; set; }

        // klucz obcy pojazdu 
        public int PojazdId { get; set; }
        [ValidateNever]
        public virtual Pojazd Pojazd { get; set; }
    }
}
