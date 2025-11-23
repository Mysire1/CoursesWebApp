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
            // 1. Перевірка: чи вибрані дні тижня?
            if (model.DaysOfWeek == null || !model.DaysOfWeek.Any())
            {
                ModelState.AddModelError("DaysOfWeek", "Виберіть хоча б один день тижня!");
            }

            // 2. Перевірка валідації моделі
            if (!ModelState.IsValid)
            {
                // Виводимо помилки в консоль (для дебагу)
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                ViewBag.Groups = await _groupService.GetAllGroupsAsync();
                ViewBag.Classrooms = await _db.Classrooms.OrderBy(c => c.RoomNumber).ToListAsync();
                return PartialView("_AddScheduleModal", model);
            }

            var group = await _db.Groups
                .Include(g => g.Teacher)
                .FirstOrDefaultAsync(g => g.GroupId == model.GroupId);
            
            var classroom = await _db.Classrooms.FindAsync(model.ClassroomId);
            
            if (group == null || classroom == null)
            {
                return BadRequest("Не знайдено групу або аудиторію.");
            }

            if (group.TeacherId == 0) // Переконайтеся, що TeacherId nullable int? або перевіряйте на null
            {
                return BadRequest("У групи не призначено викладача.");
            }

            var times = model.TimeRange.Split('-');
            if (times.Length != 2 || !TimeSpan.TryParse(times[0].Trim(), out var start) || !TimeSpan.TryParse(times[1].Trim(), out var end))
            {
                ModelState.AddModelError("TimeRange", "Введіть час у форматі 17:00 - 18:00");
                ViewBag.Groups = await _groupService.GetAllGroupsAsync();
                ViewBag.Classrooms = await _db.Classrooms.OrderBy(c => c.RoomNumber).ToListAsync();
                return PartialView("_AddScheduleModal", model);
            }

            try 
            {
                // Лічильник для перевірки
                int addedCount = 0;

                foreach (var day in model.DaysOfWeek)
                {
                    var sched = new Schedule
                    {
                        GroupId = group.GroupId,
                        ClassroomId = classroom.ClassroomId,
                        DayOfWeek = day,
                        StartTime = start,
                        EndTime = end,
                        TeacherId = group.TeacherId,
                        // УВАГА: Тут логічна пастка (див. пункт Логічна помилка нижче)
                        Date = group.StartDate.Date, 
                        Room = classroom.RoomNumber
                    };

                    _db.Schedules.Add(sched);
                    addedCount++;
                }

                if (addedCount > 0)
                {
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Якщо база даних викине помилку (наприклад, конфлікт ключів)
                // Поставте тут breakpoint, щоб побачити ex.Message
                ModelState.AddModelError("", $"Помилка збереження в БД: {ex.Message}");
                ViewBag.Groups = await _groupService.GetAllGroupsAsync();
                ViewBag.Classrooms = await _db.Classrooms.OrderBy(c => c.RoomNumber).ToListAsync();
                return PartialView("_AddScheduleModal", model);
            }

            return RedirectToAction("Index");
        }
    }
}
