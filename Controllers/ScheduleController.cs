using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursesWebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly IQueryService _queryService;
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;
        private readonly ApplicationDbContext _db;
        public ScheduleController(IQueryService queryService, IGroupService groupService, ITeacherService teacherService, ApplicationDbContext db)
        {
            _queryService = queryService;
            _groupService = groupService;
            _teacherService = teacherService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            ViewBag.Classrooms = await _db.Classrooms.OrderBy(c=>c.RoomNumber).ToListAsync();
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
