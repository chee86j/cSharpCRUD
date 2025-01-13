using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Models;
using Microsoft.AspNetCore.Cors;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<User> userManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                _logger.LogInformation("Starting registration for email: {Email}", model.Email);

                if (model == null)
                {
                    _logger.LogWarning("Registration failed: Model is null");
                    return BadRequest("Model is null");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                    return BadRequest("Email and password are required");

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return BadRequest("User with this email already exists");

                var user = new User { UserName = model.Email, Email = model.Email };
                
                _logger.LogInformation("Attempting to create user");
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created successfully");
                    try
                    {
                        var token = GenerateJwtToken(user);
                        return Ok(new { 
                            message = "Registration successful",
                            email = user.Email,
                            token = token
                        });
                    }
                    catch (Exception tokenEx)
                    {
                        _logger.LogError(tokenEx, "Token generation failed for user {Email}", user.Email);
                        return StatusCode(500, new { 
                            message = "Registration successful but token generation failed",
                            error = "Authentication error"
                        });
                    }
                }

                _logger.LogWarning("User creation failed: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(new { message = "Registration failed", errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed with error");
                return StatusCode(500, new { 
                    message = "Internal server error",
                    error = _configuration["DetailedErrors"] == "true" ? ex.Message : "An unexpected error occurred"
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return Unauthorized("Invalid email or password");

            var token = GenerateJwtToken(user);
            return Ok(new { 
                token = token,
                email = user.Email
            });
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"];
                var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? _configuration["Jwt:Issuer"];

                if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
                {
                    throw new InvalidOperationException("JWT key is not properly configured. Key must be at least 32 characters long.");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                
                // Validate key size
                if (key.KeySize < 256)
                {
                    throw new InvalidOperationException($"JWT key size is insufficient. Current: {key.KeySize} bits, Required: 256 bits");
                }

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: null,
                    claims: new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    },
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                throw new InvalidOperationException("Error generating authentication token", ex);
            }
        }
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }
    }
}
