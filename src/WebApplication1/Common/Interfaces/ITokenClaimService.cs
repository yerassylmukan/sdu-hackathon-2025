namespace WebApplication1.Common.Interfaces;

public interface ITokenClaimService
{
    Task<Result<string>> GetTokenAsync(string userName);
}