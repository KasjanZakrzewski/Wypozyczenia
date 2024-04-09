using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WypozyczeniaAPI.Areas.Identity.Data;

namespace WypozyczeniaAPI.Models
{
    public class Serwis
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public DateTime DataRozpoczecia { get; set; }

        public DateTime? DataZakonczenia { get; set; }

        // klucz obcy pojazdu 
        public int PojazdId { get; set; }
        [ValidateNever]
        public virtual Pojazd Pojazd { get; set; }

        // klucz obcy pojazdu 
        public string AdminId { get; set; }
        [ValidateNever]
        public virtual User Admin { get; set; }

        // klucz obcy pojazdu 
        public string PracownikId { get; set; }
        [ValidateNever]
        public virtual User Pracownik { get; set; }

        [ValidateNever]
        public virtual List<Opis> Opis { get; set; }

    }
}
