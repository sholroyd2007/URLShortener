namespace URLShortener.Models
{
    public class Url
    {
        public int Id { get; set; }
        public string ShortCode { get; set; }
        public string Destination { get; set; }
        public int Clicks { get; set; } = 0;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Delete { get; set; } = DateTime.UtcNow.AddDays(14);

    }
}
