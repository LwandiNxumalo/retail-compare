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
    private readonly RetailCompareDbContext _context = context;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and Password are required.");
        }

        if (await _context.Users.AnyAsync(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest("User with this email already exists.");
        }

        var user = new User
        {
            Email = request.Email.Trim().ToLower(),
            FullName = request.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);

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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id.ToString(),
            Email = user.Email,
            FullName = user.FullName
        });
    }
}