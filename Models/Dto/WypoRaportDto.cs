namespace WypozyczeniaAPI.Models.Dto
{
    public class WypoRaportDto
    {
        public int Id { get; set; }
        public DateTime DataRozpoczęcia { get; set; }
        public DateTime? DataZakończenia { get; set; }
        public float Dystans { get; set; }
        public float Oplata { get; set; }
        public string Rejestracja { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Email { get; set; }
    }
}
