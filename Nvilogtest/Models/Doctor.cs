namespace Nvilogtest.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int PromotionLevel { get; set; }
        public List<Phone> Phones { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public Reviews Reviews { get; set; }
        public List<string> LanguageIds { get; set; } = new List<string>();

        public string LanguageNames { get; set; } = string.Empty;
        public bool HasArticle { get; set; }
    }

    public class Phone
    {
        public string Number { get; set; }
        public int PhoneType { get; set; }
    }
   

    public class Reviews
    {
        public double ProfessionalismRate { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public double WaitingTimeRate { get; set; }
        public double ServiceRate { get; set; }
    }

     
}
