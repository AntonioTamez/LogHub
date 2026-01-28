using LogHub.API.DTOs;
using LogHub.API.Services;
using LogHub.Core.Entities;
using LogHub.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LogHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
        {
            return BadRequest(new { error = "Email already exists" });
        }

        var user = new User
        {
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        var expiryHours = int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24");

        _logger.LogInformation("User registered: {Email}", user.Email);

        return CreatedAtAction(nameof(Register), new AuthResponse(
            token,
            user.Email,
            user.Name,
            DateTime.UtcNow.AddHours(expiryHours)
        ));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { error = "Invalid email or password" });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { error = "Account is inactive" });
        }

        var token = _jwtService.GenerateToken(user);
        var expiryHours = int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24");

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return Ok(new AuthResponse(
            token,
            user.Email,
            user.Name,
            DateTime.UtcNow.AddHours(expiryHours)
        ));
    }
}
