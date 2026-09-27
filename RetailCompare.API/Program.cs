using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RetailCompare.API.Data;
using RetailCompare.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Register DbContext with SQLite connection string
builder.Services.AddDbContext<RetailCompareDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=retailcompare.db"));

// 2. Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 3. Register Custom Services
builder.Services.AddScoped<ITokenService, TokenService>();

// 4. Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKey_ForRetailCompareApp_2026_KeyMustBeLongEnough!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "RetailCompareAPI",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "RetailCompareApp",
            ValidateLifetime = true
        };
    });

// Add Controllers, Swagger, etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS Middleware (Must be before UseAuthentication / UseAuthorization)
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Enable Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();