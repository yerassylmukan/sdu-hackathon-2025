using Microsoft.AspNetCore.Identity;
using WebApplication1.Common;
using WebApplication1.Common.Interfaces;
using WebApplication1.Identity;
using WebApplication1.Models;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserProfileModel>> GetProfileAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return Result<UserProfileModel>.Failure("User not found.");

        var dto = new UserProfileModel
        {
            UserName = user.UserName!,
            Email = user.Email!,
            DisplayName = user.UserName!
        };

        return Result<UserProfileModel>.Success(dto);
    }

    public async Task<Result<IEnumerable<UserProfileModel>>> GetProfilesAsync()
    {
        var users = _userManager.Users.ToList();

        var profileList = users.Select(user => new UserProfileModel
        {
            UserName = user.UserName!,
            Email = user.Email!,
            DisplayName = user.UserName!
        });

        return Result<IEnumerable<UserProfileModel>>.Success(profileList);
    }
}