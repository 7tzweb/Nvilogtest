namespace Nvilogtest.Models
{
  
    public class Article
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public List<Sponsorship> Sponsorships { get; set; } = new List<Sponsorship>();
    }

    public class Sponsorship
    {
        public string SponsorshipName { get; set; } = string.Empty;
        public int SponsorshipType { get; set; }
        public int SponsorshipId { get; set; }
        public string PromotionText { get; set; } = string.Empty;
    }

}
