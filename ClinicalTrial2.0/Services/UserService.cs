using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ClinicalTrial2._0.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    PhysicalAddress = model.PhysicalAddress,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.UtcNow,
                    EmailConfirmed = true // For demo purposes
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Ensure roles exist
                    await EnsureRolesExistAsync();

                    // Add user to role
                    await _userManager.AddToRoleAsync(user, model.UserType);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        private async Task EnsureRolesExistAsync()
        {
            string[] roleNames = { "Participant", "Recruiter" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
