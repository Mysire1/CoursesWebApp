using CoursesWebApp.Models;

namespace CoursesWebApp.Services
{
    public interface IQueryService
    {
        Task<IEnumerable<ExamResult>> GetFailedExamResultsAsync();
        Task<IEnumerable<Level>> GetLevelsWithFailuresAsync();
        Task<int> GetFailedExamsCountAsync();
        
        Task<IEnumerable<Student>> GetFullyPaidStudentsAsync();
        Task<IEnumerable<Student>> GetNotFullyPaidStudentsAsync();
        Task<IEnumerable<Student>> GetStudentsWithDebtLessThan50Async();
        Task<IEnumerable<Student>> GetStudentsWithDeferralsAsync();
        
        Task<IEnumerable<Schedule>> GetScheduleByGroupAsync(int groupId);
        Task<IEnumerable<Schedule>> GetScheduleByTeacherAsync(int teacherId);
    }
}