using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using WypozyczeniaAPI.Areas.Identity.Data;

namespace WypozyczeniaAPI.Models
{
    public class Zgłoszenie
    {
        [Key]
        public int Id { get; set; }

        public DateTime Data { get; set; }
        public string? Treść { get; set; }

        // klucz obcy Użytkownika
        public String UzytkownikId { get; set; }
        [ValidateNever]
        public virtual User Uzytkownik { get; set; }

        // klucz obcy Admina
        public String? AdminId { get; set; }
        [ValidateNever]
        public virtual User Admin { get; set; }
    }
}
