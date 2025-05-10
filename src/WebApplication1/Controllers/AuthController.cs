using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Register([FromBody] UserModel userModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(userModel.UserName, userModel.Password);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] UserModel userModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginAsync(userModel.UserName, userModel.Password);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<bool>> GiveAdminRole([FromBody] RoleRequest request)
    {
        var result = await _authService.GiveAdminRoleAsync(request.UserName);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<string>> ChangePassword([FromBody] ChangePasswordRequestModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) return Unauthorized();

        var result = await _authService.ChangePasswordAsync(userName, request.CurrentPassword, request.NewPassword);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<bool>> ChangeEmail([FromBody] ChangeEmailRequestModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) return Unauthorized();

        var result = await _authService.ChangeEmailAsync(userName, request.NewEmail);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{token}")]
    public IActionResult GetPayload(string token)
    {
        if (string.IsNullOrEmpty(token)) return BadRequest("Token is required.");

        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token)) return BadRequest("The token is not in a valid JWT format.");

            var jwtToken = handler.ReadJwtToken(token);

            var payload = jwtToken.Claims
                .ToDictionary(claim => claim.Type, claim => (object)claim.Value);

            return Ok(payload);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}