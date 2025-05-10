using Microsoft.AspNetCore.Identity;
using WebApplication1.Common;
using WebApplication1.Common.Interfaces;
using WebApplication1.Identity;

namespace WebApplication1.Services;

public class AuthService : IAuthService
{
    private readonly ITokenClaimService _tokenClaimService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager, ITokenClaimService tokenClaimService)
    {
        _userManager = userManager;
        _tokenClaimService = tokenClaimService;
    }

    public async Task<Result<string>> RegisterAsync(string userName, string password)
    {
        if (await _userManager.FindByNameAsync(userName) != null)
            return Result<string>.Failure("UserName already exists");

        var user = new ApplicationUser { UserName = userName, Email = userName };
        await _userManager.CreateAsync(user, password);
        await _userManager.AddToRoleAsync(user, "User");
        return await _tokenClaimService.GetTokenAsync(userName);
    }

    public async Task<Result<string>> LoginAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null) return Result<string>.Failure("UserName does not exist.");

        if (!await _userManager.CheckPasswordAsync(user, password)) return Result<string>.Failure("Invalid password.");

        return await _tokenClaimService.GetTokenAsync(user.UserName);
    }

    public async Task<Result<bool>> GiveAdminRoleAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return Result<bool>.Failure("User not found.");

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (isAdmin) return Result<bool>.Failure("User is already an admin.");

        var addToAdminResult = await _userManager.AddToRoleAsync(user, "Admin");
        if (!addToAdminResult.Succeeded) return Result<bool>.Failure("Failed to assign admin role.");

        var isUser = await _userManager.IsInRoleAsync(user, "User");
        if (isUser)
        {
            var removeUserResult = await _userManager.RemoveFromRoleAsync(user, "User");
            if (!removeUserResult.Succeeded)
                return Result<bool>.Failure("Admin role assigned, but failed to remove user role.");
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ChangePasswordAsync(string userName, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return Result<bool>.Failure("User not found.");

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changePasswordResult.Succeeded)
            return Result<bool>.Failure("Failed to change password: " +
                                        string.Join("; ", changePasswordResult.Errors.Select(e => e.Description)));

        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> ChangeEmailAsync(string userName, string newEmail)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return Result<string>.Failure("User not found.");

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var changeEmailResult = await _userManager.ChangeEmailAsync(user, newEmail, token);

        if (!changeEmailResult.Succeeded)
            return Result<string>.Failure("Failed to change email: " +
                                          string.Join("; ", changeEmailResult.Errors.Select(e => e.Description)));

        user.UserName = newEmail;
        var updateUsernameResult = await _userManager.UpdateAsync(user);

        if (!updateUsernameResult.Succeeded)
            return Result<string>.Failure("Email updated, but failed to update username: " +
                                          string.Join("; ", updateUsernameResult.Errors.Select(e => e.Description)));

        var jwtToken = await _tokenClaimService.GetTokenAsync(newEmail);

        return jwtToken.IsFailure
            ? Result<string>.Failure("Failed to change password.")
            : Result<string>.Success(jwtToken.Value!);
    }
}