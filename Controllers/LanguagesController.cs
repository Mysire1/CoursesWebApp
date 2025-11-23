using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoursesWebApp.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class LanguagesController : Controller
    {
        private readonly ILanguageService _languageService;
        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            var allWithCount = await ((dynamic)_languageService).GetAllLanguagesWithGroupCountAsync();
            if (!string.IsNullOrEmpty(filter))
                allWithCount = allWithCount.Where(l => l.Language.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.Filter = filter;
            ViewBag.LanguagesWithCount = allWithCount;
            return View();
        }
    }
}
