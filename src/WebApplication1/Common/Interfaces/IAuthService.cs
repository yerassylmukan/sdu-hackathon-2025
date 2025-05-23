﻿namespace WebApplication1.Common.Interfaces;

public interface IAuthService
{
    Task<Result<string>> RegisterAsync(string userName, string password);
    Task<Result<string>> LoginAsync(string userName, string password);
    Task<Result<bool>> GiveAdminRoleAsync(string userName);
    Task<Result<bool>> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    Task<Result<string>> ChangeEmailAsync(string username, string newEmail);
}