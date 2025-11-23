using CoursesWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoursesWebApp.Services
{
    public interface ILevelService
    {
        Task<IEnumerable<Level>> GetAllLevelsAsync();
        Task<Level?> GetLevelByIdAsync(int id);
    }
}
