using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailCompare.API.Data;
using RetailCompare.API.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(RetailCompareDbContext context, ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and Password are required.");
        }

        var normalizedEmail = request.Email.Trim().ToLower();

        if (await context.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            return BadRequest("User with this email already exists.");
        }

        var user = new User
        {
            Email = normalizedEmail,
            FullName = request.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var token = tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id.ToString(),
            Email = user.Email,
            FullName = user.FullName
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id.ToString(),
            Email = user.Email,
            FullName = user.FullName
        });
    }


}