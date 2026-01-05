using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceManagementApi.DTOs;
using ServiceManagementApi.Models;

namespace ServiceManagementApi.Services;

public interface IAdminService
{
    Task<List<object>> GetAllUsersWithRolesAsync();
    Task<IdentityResult> UpdateUserRoleAsync(string userId, string newRole);
}


public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;


    // Change IdentityUser to ApplicationUser here:
    public AdminService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<object>> GetAllUsersWithRolesAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? "Customer"
            });
        }
        return userList;
    }

    public async Task<IdentityResult> UpdateUserRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

       
        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!removeResult.Succeeded) return removeResult;

        
        return await _userManager.AddToRoleAsync(user, newRole);
    }
}