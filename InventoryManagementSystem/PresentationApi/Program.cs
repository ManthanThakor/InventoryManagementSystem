using Application.Services.AdminService;
using Application.Services.AdminServices;
using Application.Services.AuthServices;
using Application.Services.CategoryServices;
using Application.Services.CustomerServices;
using Application.Services.ItemServices;
using Application.Services.JwtServices;
using Application.Services.PasswordServices;
using Application.Services.PurchaseOrderServices;
using Application.Services.SalesOrderServices;
using Application.Services.SupplierServices;
using Application.Services.SupportServices;
using Application.Services.UserTypeServices;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PresentationApi.Extensions;
using PresentationApi.Hubs;
using PresentationApi.Seed;
using PresentationApi.SignalRServices;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

AnsiConsoleTheme customConsoleTheme = new AnsiConsoleTheme(
    new Dictionary<ConsoleThemeStyle, string>
    {
        [ConsoleThemeStyle.Text] = "\x1b[38;5;253m",
        [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;246m",
        [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;242m",
        [ConsoleThemeStyle.Invalid] = "\x1b[38;5;196m",
        [ConsoleThemeStyle.Null] = "\x1b[38;5;42m",
        [ConsoleThemeStyle.Name] = "\x1b[38;5;15m",
        [ConsoleThemeStyle.String] = "\x1b[38;5;217m",
        [ConsoleThemeStyle.Number] = "\x1b[38;5;151m",
        [ConsoleThemeStyle.Boolean] = "\x1b[38;5;219m",
        [ConsoleThemeStyle.Scalar] = "\x1b[38;5;217m",
        [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;5;137m",
        [ConsoleThemeStyle.LevelDebug] = "\x1b[38;5;111m",
        [ConsoleThemeStyle.LevelInformation] = "\x1b[38;5;77m",
        [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;220m",
        [ConsoleThemeStyle.LevelError] = "\x1b[38;5;196m",
        [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;199m",
    });

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "InventoryAPI")
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 102400;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
builder.Services.AddScoped<ICustomerSupportService, CustomerSupportService>();

builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory Management API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Content-Disposition");
    });

    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true);
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("AdminOrSupplier", policy =>
        policy.RequireRole("Admin", "Supplier"));

    options.AddPolicy("AdminOrCustomer", policy =>
        policy.RequireRole("Admin", "Customer"));
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}



app.UseCors("AllowAll");
app.UseCors("SignalRPolicy");

app.UseExceptionHandler("/error");

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CustomerSupportHub>("/hubs/customerSupport");

await app.SeedDatabaseAsync();


try
{
    Log.Information("Starting Inventory Management API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}