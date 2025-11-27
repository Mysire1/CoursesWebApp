using CoursesWebApp.Models;

namespace CoursesWebApp.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<Group?> GetGroupByIdAsync(int id);
        Task<IEnumerable<Group>> SearchGroupsAsync(int? languageId = null, int? teacherId = null);
        Task<IEnumerable<Group>> GetSmallGroupsAsync(int maxStudents = 5);
        Task<IEnumerable<Group>> GetLargeGroupsAsync(int exactStudents = 20);
        Task<int> ApplySmallGroupSurchargeAsync(decimal surchargePercentage = 20);
        Task<int> ApplyLargeGroupDiscountAsync(decimal discountPercentage = 5);
        Task CreateGroupAsync(Group group);
        Task UpdateGroupAsync(Group group);
        Task DeleteGroupAsync(int id);
    }
}
