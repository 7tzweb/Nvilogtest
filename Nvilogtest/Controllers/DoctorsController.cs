using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nvilogtest.Models; // שים לב: אם השם של הפרויקט שלך שונה - תעדכן
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Nvilogtest.Controllers
{
    public class DoctorsController : Controller
    {
        public IActionResult Index()
        {
            // טוען את רשימת הרופאים מהקובץ
            var doctors = LoadDoctors()
           .OrderByDescending(d =>
               (double.Parse($"{(d.Reviews?.AverageRating ?? 0)}{(d.Reviews?.TotalRatings ?? 0)}") - (d.PromotionLevel))
           )
           .ToList();

                return View(doctors);
        }
        private Dictionary<string, string> LoadLanguages()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "language.json");

            if (!System.IO.File.Exists(filePath))
            {
                return new Dictionary<string, string>();
            }

            var json = System.IO.File.ReadAllText(filePath);
            var languagesWrapper = JsonConvert.DeserializeObject<LanguageWrapper>(json);

            return languagesWrapper?.Language ?? new Dictionary<string, string>();
        }

        private List<Article> LoadArticles()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "articles.json");

            if (!System.IO.File.Exists(filePath))
            {
                return new List<Article>();
            }

            var json = System.IO.File.ReadAllText(filePath);
            var articles = JsonConvert.DeserializeObject<List<Article>>(json);

            return articles ?? new List<Article>();
        }

        // מחלקת עזר
        private class LanguageWrapper
        {
            public Dictionary<string, string> Language { get; set; } = new Dictionary<string, string>();
        }

        public IActionResult SaveLead([FromBody] LeadModel lead)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Leads.json");

            List<LeadModel> leads = new List<LeadModel>();

            // אם הקובץ קיים - נטען את התוכן הקיים
            if (System.IO.File.Exists(filePath))
            {
                var existingJson = System.IO.File.ReadAllText(filePath);
                leads = JsonConvert.DeserializeObject<List<LeadModel>>(existingJson) ?? new List<LeadModel>();
            }

            // נוסיף את הליד החדש
            leads.Add(lead);

            // נשמור את הכל מחדש
            var updatedJson = JsonConvert.SerializeObject(leads, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, updatedJson);

            return Ok();
        }

        private List<Doctor> LoadDoctors()
        {
            var doctorsPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "doctors.json");
            var doctorsJson = System.IO.File.ReadAllText(doctorsPath);
            var doctors = JsonConvert.DeserializeObject<List<Doctor>>(doctorsJson) ?? new List<Doctor>();

            var languagesMap = LoadLanguages();

            foreach (var doctor in doctors)
            {
                if (doctor.LanguageIds != null)
                {
                    doctor.LanguageNames = string.Join(", ", doctor.LanguageIds
                        .Where(id => languagesMap.ContainsKey(id))
                        .Select(id => languagesMap[id]));
                }
                else
                {
                    doctor.LanguageNames = string.Empty;
                }
            }

            var articles = LoadArticles(); // נטען את המאמרים

            foreach (var doctor in doctors)
            {
                doctor.HasArticle = articles.Any(article =>
                    article.Sponsorships != null &&
                    article.Sponsorships.Any(s => s.SponsorshipId == doctor.Id)
                );
            }

            // תיקון טלפונים 
            foreach (var doctor in doctors)
            {
                if (doctor.Phones != null)
                {
                    foreach (var phone in doctor.Phones)
                    {
                        if (!string.IsNullOrEmpty(phone.Number) && !phone.Number.Contains("-"))
                        {
                            if (phone.Number.Length >= 3)
                            {
                                phone.Number = phone.Number.Substring(0, 2) + "-" + phone.Number.Substring(2);
                            }
                        }
                    }
                }
            }

            // מיון OverallRating
            return doctors.OrderByDescending(d =>
                (double.Parse($"{(d.Reviews?.AverageRating ?? 0)}{(d.Reviews?.TotalRatings ?? 0)}") - (d.PromotionLevel))
            ).ToList();
        }
    }
}
