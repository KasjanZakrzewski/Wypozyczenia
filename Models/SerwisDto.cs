using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WypozyczeniaAPI.Areas.Identity.Data;

namespace WypozyczeniaAPI.Models
{
    public class SerwisDto
    {
        
        public int Id { get; set; }
        public string Status { get; set; }

        public DateTime DataRozpoczecia { get; set; }

        public DateTime? DataZakonczenia { get; set; }

        // klucz obcy pojazdu 
        public string Rejestracja { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }

        // klucz obcy pojazdu 
        public string Admin { get; set; }


        // klucz obcy pojazdu 
        public string Pracownik { get; set; }
    }
}
