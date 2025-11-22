[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Teacher")]
public async Task<IActionResult> Edit(int id, Student student)
{
    if (id != student.StudentId)
        return NotFound();
        
    if (ModelState.IsValid)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(student.Email))
            {
                ModelState.AddModelError("Email", "Email не може бути порожнім!");
                ViewBag.Groups = await _groupService.GetAllGroupsAsync();
                return View(student);
            }
            var another = await _studentService.FindByEmailAsync(student.Email);
            if (another != null && another.StudentId != student.StudentId)
            {
                ModelState.AddModelError("Email", "Email вже використовується іншим студентом!");
                ViewBag.Groups = await _groupService.GetAllGroupsAsync();
                return View(student);
            }
            var dbStudent = await _studentService.GetStudentByIdAsync(id);
            if (dbStudent == null) return NotFound();
            dbStudent.FirstName = student.FirstName;
            dbStudent.LastName = student.LastName;
            dbStudent.Email = student.Email;
            dbStudent.Phone = student.Phone;
            dbStudent.HasDiscount = student.HasDiscount;
            dbStudent.DiscountPercentage = student.HasDiscount ? Math.Clamp(student.DiscountPercentage, 0, 100) : 0;
            dbStudent.GroupId = student.GroupId;
            dbStudent.DateOfBirth = DateTime.SpecifyKind(student.DateOfBirth, DateTimeKind.Utc);
            await _studentService.UpdateStudentAsync(dbStudent);
            TempData["SuccessMessage"] = "Студента оновлено!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Помилка при оновленні: {ex.Message}");
        }
    }
    ViewBag.Groups = await _groupService.GetAllGroupsAsync();
    return View(student);
}
