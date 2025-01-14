using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv; // Add at the very top of the file

DotNetEnv.Env.Load(); // Add at the very top of the file

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL DB
var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity Auth
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Auth - updated section
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? 
    throw new InvalidOperationException("JWT_KEY is not configured");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? 
    throw new InvalidOperationException("JWT_ISSUER is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),
                NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
            };
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add detailed logging configuration
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable detailed error messages in development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Add middleware to log all requests in development
    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
        
        // Log request body for debug purposes
        if (context.Request.ContentType?.Contains("application/json") == true)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            logger.LogDebug($"Request Body: {body}");
            context.Request.Body.Position = 0;
        }
        
        await next();
    });
}

// Comment out HTTPS redirection
// app.UseHttpsRedirection();

// Configure the HTTP request pipeline
app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});

app.UseCors("AllowAll"); // Make sure this is before Authentication
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        throw;
    }
});

app.Run();
