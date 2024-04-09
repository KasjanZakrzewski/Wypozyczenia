using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WypozyczeniaAPI.Models
{
    public class PojazdMap
    {
        public int Id { get; set; }
        public string Rejestracja { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public float NS { get; set; }
        public float WE { get; set; }

        public int RezerwacjaId { get; set; }
        public int WypozyczenieId { get; set; }
    }
}
