using CoursesWebApp.Services;
using CoursesWebApp.Models;
using CoursesWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursesWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSchedule(AddScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Rotate modal with errors (keep selects)
                ViewBag.Groups = await _groupService.GetAllGroupsAsync();
                ViewBag.Classrooms = await _db.Classrooms.OrderBy(c => c.RoomNumber).ToListAsync();
                return PartialView("_AddScheduleModal", model);
            }

            var group = await _db.Groups.Include(g=>g.Teacher).FirstOrDefaultAsync(g=>g.GroupId == model.GroupId);
            var classroom = await _db.Classrooms.FindAsync(model.ClassroomId);
            if (group == null || classroom == null)
            {
                return BadRequest("Не знайдено групу або аудиторію.");
            }
            // Parse time
            var times = model.TimeRange.Split('-');
            if (times.Length != 2 || !TimeSpan.TryParse(times[0].Trim(), out var start) || !TimeSpan.TryParse(times[1].Trim(), out var end))
            {
                ModelState.AddModelError("TimeRange", "Введіть час у форматі 17:00 - 18:00");
                ViewBag.Groups = await _groupService.GetAllGroupsAsync();
                ViewBag.Classrooms = await _db.Classrooms.OrderBy(c => c.RoomNumber).ToListAsync();
                return PartialView("_AddScheduleModal", model);
            }
            // Save to database
            var sched = new Schedule
            {
                GroupId = group.GroupId,
                ClassroomId = classroom.ClassroomId,
                DayOfWeek = model.DayOfWeek,
                StartTime = start,
                EndTime = end,
                TeacherId = group.TeacherId,
                Date = group.StartDate.Date,
                Room = classroom.RoomNumber
            };
            _db.Schedules.Add(sched);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
