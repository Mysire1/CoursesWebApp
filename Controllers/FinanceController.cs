using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursesWebApp.Controllers
{
    [Authorize]
    public class FinanceController : Controller
    {
        private readonly IQueryService _queryService;
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        public FinanceController(IQueryService queryService, IStudentService studentService, IGroupService groupService)
        {
            _queryService = queryService;
            _studentService = studentService;
            _groupService = groupService;
        }

        public IActionResult Index()
        {
            // для майбутньої аналітики можна використати ViewBag, поки фіктивно
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PaymentStatus(string statusType)
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
        public async Task<IActionResult> ApplySmallGroupSurcharge()
        {
            var smallGroups = await _groupService.GetSmallGroupsAsync();
            var affectedStudents = await _groupService.ApplySmallGroupSurchargeAsync();
            return Json(new { success = true, message = $"Надбавку застосовано до {affectedStudents} студентів", groups = smallGroups });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyLargeGroupDiscount()
        {
            var largeGroups = await _groupService.GetLargeGroupsAsync();
            var affectedStudents = await _groupService.ApplyLargeGroupDiscountAsync();
            return Json(new { success = true, message = $"Знижку застосовано до {affectedStudents} студентів", groups = largeGroups });
        }
    }
}