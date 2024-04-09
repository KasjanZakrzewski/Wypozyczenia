using System.Security.Cryptography.X509Certificates;

namespace WypozyczeniaAPI.Models
{
    public class UzytkownikDto
    {
        public string? Id { get; set; }
        public string? Imie { get; set; }
        public string? Nazwisko { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
