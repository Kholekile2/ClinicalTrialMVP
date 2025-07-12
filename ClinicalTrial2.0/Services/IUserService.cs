using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Models.ViewModels;

namespace ClinicalTrial2._0.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterViewModel model);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> IsEmailTakenAsync(string email);
    }
}
