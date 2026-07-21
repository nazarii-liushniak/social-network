using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Extensions;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) :  ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] CreateUserRequest createUserRequest)
    {
        var result = await authService.RegisterAsync(createUserRequest);

        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokensResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await authService.LoginAsync(loginRequest);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshTokenString)
    {
        var result = await authService.LogoutAsync(refreshTokenString);

        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokensResponse>> RefreshTokens([FromBody] string refreshTokenString)
    {
        var result = await authService.RefreshTokensAsync(refreshTokenString);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var result = await authService.ChangePasswordAsync(
            changePasswordRequest.OldPassword,
            changePasswordRequest.NewPassword);
        
        return result.ToActionResult();
    }
}