using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Diagnostics.CodeAnalysis;

namespace WypozyczeniaAPI.Models
{
    public class Pojazd
    {
        public int Id { get; set; }
        public string Rejestracja { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        [ValidateNever]
        public string Hash { get; set; }
        public float NS { get; set; }
        public float WE { get; set; }

        // odwołanie, inna tabla odwołuje sie do tej 
        [ValidateNever]
        public virtual List<Wypozyczenie> Wypozyczenie { get; set; }
        [ValidateNever]
        public virtual List<Rezerwacja> Rezerwacja { get; set; }
        //public virtual List<EmergencyPozycja> EmergencyPozycja { get; set; }
        [ValidateNever]
        public virtual List<Serwis> Serwis { get; set; }
    }
}
