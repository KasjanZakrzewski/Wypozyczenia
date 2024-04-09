namespace WypozyczeniaAPI.Models.Dto
{
    public class WypoRezDto2
    {
        public int Id { get; set; }
        public DateTime DataRozpoczęcia { get; set; }
        public DateTime? DataZakończenia { get; set; }
        public string Rejestracja { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Email { get; set; }
    }
}
