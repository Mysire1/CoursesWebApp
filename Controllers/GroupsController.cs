[HttpGet]
public async Task<IActionResult> GetGroupStudents(int groupId)
{
    var students = await _studentService.GetAllStudentsAsync();
    var groupStudents = students
        .Where(s => s.GroupId == groupId)
        .Select(s => new {
            s.StudentId,
            s.FirstName,
            s.LastName,
            s.Phone
        }).ToList();
    return Json(groupStudents);
}