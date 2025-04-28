using Application.Configuration;
using Application.Services.AuthServices;
using Application.Services.CategoryServices;
using Application.Services.JwtServices;
using Application.Services.PasswordServices;
using Application.Services.UserTypeServices;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using PresentationApi.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext for SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register custom services
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
//builder.Services.AddScoped<IPasswordService, PasswordService>();
//builder.Services.AddScoped<IJwtService, JwtService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Register the custom filter globally
builder.Services.AddScoped<ValidateModelAttribute>();
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ValidateModelAttribute>();  // Add custom filter here
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
