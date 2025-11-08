using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursesWebApp.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly IQueryService _queryService;
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;
        public ScheduleController(IQueryService queryService, IGroupService groupService, ITeacherService teacherService)
        {
            _queryService = queryService;
            _groupService = groupService;
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            return View();
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