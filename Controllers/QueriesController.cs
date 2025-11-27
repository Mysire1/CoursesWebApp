using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursesWebApp.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class QueriesController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ILanguageService _languageService;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;
        private readonly IQueryService _queryService;

        public QueriesController(
            IGroupService groupService,
            ILanguageService languageService,
            ITeacherService teacherService,
            IStudentService studentService,
            IQueryService queryService)
        {
            _groupService = groupService;
            _languageService = languageService;
            _teacherService = teacherService;
            _studentService = studentService;
            _queryService = queryService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchGroups(int? languageId, int? teacherId)
        {
            var groups = await _groupService.SearchGroupsAsync(languageId, teacherId);
            return PartialView("_GroupsPartial", groups);
        }
        
        [HttpPost]
        public async Task<IActionResult> CalculateCost(string calculationType, int? languageId)
        {
            try
            {
                object result = calculationType switch
                {
                    "all" => await _languageService.CalculateAllLanguagesCostAsync(),
                    "language" when languageId.HasValue => await _languageService.CalculateLanguageCostAsync(languageId.Value),
                    "byLevel" when languageId.HasValue => await _languageService.CalculateCostByLevelAsync(languageId.Value),
                    "monthly" when languageId.HasValue => await _languageService.CalculateMonthlyLanguageCostAsync(languageId.Value),
                    _ => new { Error = "Невірний тип розрахунку" }
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetFailedExams()
        {
            var failedResults = await _queryService.GetFailedExamResultsAsync();
            var levelsWithFailures = await _queryService.GetLevelsWithFailuresAsync();
            var failedCount = await _queryService.GetFailedExamsCountAsync();

            var result = new
            {
                FailedResults = failedResults,
                LevelsWithFailures = levelsWithFailures,
                TotalCount = failedCount
            };

            return Json(new { success = true, data = result });
        }
        
        [HttpPost]
        public async Task<IActionResult> GetTeachersByLanguageCount()
        {
            var teachersByCount = await _teacherService.GetTeachersByLanguageCountAsync();
            return Json(new { success = true, data = teachersByCount });
        }
        
        [HttpPost]
        public async Task<IActionResult> GetPaymentStatus(string statusType)
        {
            var students = statusType switch
            {
                "fullyPaid" => await _queryService.GetFullyPaidStudentsAsync(),
                "notFullyPaid" => await _queryService.GetNotFullyPaidStudentsAsync(),
                "debtLess50" => await _queryService.GetStudentsWithDebtLessThan50Async(),
                "withDeferrals" => await _queryService.GetStudentsWithDeferralsAsync(),
                "withDiscounts" => await _studentService.GetStudentsWithDiscountAsync(),
                _ => new List<CoursesWebApp.Models.Student>()
            };

            return PartialView("_StudentsPartial", students);
        }
        
        [HttpPost]
        public async Task<IActionResult> GetStudentsByLanguage()
        {
            var germanStudents = await _studentService.GetStudentsLearningLanguageAsync("Німецька");
            var multiLanguageStudents = await _studentService.GetStudentsLearningMultipleLanguagesAsync();

            var result = new
            {
                GermanStudents = germanStudents,
                MultiLanguageStudents = multiLanguageStudents
            };

            return Json(new { success = true, data = result });
        }
        
        [HttpPost]
        public async Task<IActionResult> ApplySmallGroupSurcharge()
        {
            try
            {
                var smallGroups = await _groupService.GetSmallGroupsAsync();
                var affectedStudents = await _groupService.ApplySmallGroupSurchargeAsync();
                
                return Json(new { 
                    success = true, 
                    message = $"Надбавку застосовано до {affectedStudents} студентів",
                    groups = smallGroups
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> ApplyLargeGroupDiscount()
        {
            try
            {
                var largeGroups = await _groupService.GetLargeGroupsAsync();
                var affectedStudents = await _groupService.ApplyLargeGroupDiscountAsync();
                
                return Json(new { 
                    success = true, 
                    message = $"Знижку застосовано до {affectedStudents} студентів",
                    groups = largeGroups
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> GetSchedule(int? groupId, int? teacherId)
        {
            var schedule = groupId.HasValue
                ? await _queryService.GetScheduleByGroupAsync(groupId.Value)
                : teacherId.HasValue
                    ? await _queryService.GetScheduleByTeacherAsync(teacherId.Value)
                    : new List<CoursesWebApp.Models.Schedule>();

            return PartialView("_SchedulePartial", schedule);
        }
    }
}