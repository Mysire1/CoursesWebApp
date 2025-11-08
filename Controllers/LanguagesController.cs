using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoursesWebApp.Controllers
{
    [Authorize]
    public class LanguagesController : Controller
    {
        private readonly ILanguageService _languageService;
        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            var languages = await _languageService.GetAllLanguagesAsync();
            if (!string.IsNullOrEmpty(filter))
                languages = languages.Where(l => l.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.Filter = filter;
            ViewBag.Languages = languages;
            return View();
        }
    }
}