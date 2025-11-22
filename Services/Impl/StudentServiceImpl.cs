public async Task<Student> UpdateStudentAsync(Student student)
{
    var dbStudent = await _context.Students.FindAsync(student.StudentId);
    if (dbStudent == null) throw new Exception("Студент не знайдений");

    dbStudent.FirstName = student.FirstName;
    dbStudent.LastName = student.LastName;
    dbStudent.Email = student.Email;
    dbStudent.Phone = student.Phone;
    dbStudent.HasDiscount = student.HasDiscount;
    dbStudent.DiscountPercentage = student.HasDiscount ? Math.Clamp(student.DiscountPercentage, 0, 100) : 0;
    dbStudent.GroupId = student.GroupId;
    dbStudent.DateOfBirth = student.DateOfBirth;
    dbStudent.Status = student.Status;
    dbStudent.IsActive = student.IsActive;
    dbStudent.RegistrationDate = student.RegistrationDate;
    dbStudent.CreatedAt = student.CreatedAt;
    dbStudent.LastLoginAt = student.LastLoginAt;

    await _context.SaveChangesAsync();
    return dbStudent;
}
