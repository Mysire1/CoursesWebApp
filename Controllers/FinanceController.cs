[HttpPost]
public async Task<IActionResult> PaymentStatus(string statusType, int? groupId)
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
    // Фільтруємо по групі, якщо вибрана
    if (groupId.HasValue)
    {
        students = students.Where(s => s.GroupId == groupId.Value).ToList();
    }
    return PartialView("_StudentsPartial", students);
}
