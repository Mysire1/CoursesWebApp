using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Index()
        {
            ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
            return View();
        }
    }
}