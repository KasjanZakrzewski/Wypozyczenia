namespace WypozyczeniaAPI.Models
{
    public class RezWypDto
    {
        public int Id { get; set; }
        public DateTime DataRozpoczęcia { get; set; }
        public DateTime? DataZakończenia { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }

    }
}
