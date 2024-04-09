using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Diagnostics.CodeAnalysis;
using WypozyczeniaAPI.Areas.Identity.Data;

namespace WypozyczeniaAPI.Models
{
    public class Wypozyczenie
    {
        public int Id { get; set; }
        public DateTime DataRozpoczęcia { get; set; }
        public DateTime? DataZakończenia { get; set; }

        public float Dystans { get; set; }
        public float Oplata { get; set; }

        // klucz obcy pojazdu 
        public int PojazdId { get; set; }
        [ValidateNever]
        public virtual Pojazd Pojazd { get; set; }

        // klucz obcy Użytkownika
        public String UzytkownikId { get; set; }
        [ValidateNever]
        public virtual User Uzytkownik { get; set; }
    }
}
