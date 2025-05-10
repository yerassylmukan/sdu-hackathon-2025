using WebApplication1.Models;

namespace WebApplication1.Common.Interfaces;

public interface IProfileService
{
    Task<Result<UserProfileModel>> GetProfileAsync(string userName);
    Task<Result<IEnumerable<UserProfileModel>>> GetProfilesAsync();
}