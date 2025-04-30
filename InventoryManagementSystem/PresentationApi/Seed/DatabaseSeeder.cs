using Application.Services.PasswordServices;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository;

namespace PresentationApi.Seed
{
    public class DatabaseSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            IServiceProvider serviceProvider,
            ILogger<DatabaseSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                using IServiceScope scope = _serviceProvider.CreateScope();

                ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await dbContext.Database.EnsureCreatedAsync();

                await SeedUserTypesAsync(scope);

                await SeedAdminUserAsync(scope);

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private async Task SeedUserTypesAsync(IServiceScope scope)
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRepository<UserType>>();
            var userTypes = new[] { "Admin", "Customer", "Supplier" };

            foreach (var typeName in userTypes)
            {
                try
                {
                    var existingTypes = await repository.FindAll(ut => ut.Name == typeName);
                    var existingType = existingTypes.FirstOrDefault();

                    if (existingType == null)
                    {
                        var userType = new UserType
                        {
                            Name = typeName,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        };

                        await repository.Add(userType);
                        await repository.SaveChangesMethod();
                        _logger.LogInformation($"Created user type: {typeName}");
                    }
                    else
                    {
                        _logger.LogInformation($"User type already exists: {typeName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating user type {typeName}");
                    throw;
                }
            }
        }

        private async Task SeedAdminUserAsync(IServiceScope scope)
        {
            try
            {
                var userTypeRepository = scope.ServiceProvider.GetRequiredService<IRepository<UserType>>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

                // Fix: Get all Admin user types and await it
                var adminUserTypes = await userTypeRepository.FindAll(ut => ut.Name == "Admin");
                var adminUserType = adminUserTypes.FirstOrDefault();

                if (adminUserType == null)
                {
                    _logger.LogError("Admin user type not found after seeding");
                    throw new InvalidOperationException("Admin user type not found");
                }

                var adminUsers = await userRepository.FindAll(u => u.Username == "admin");
                var adminUser = adminUsers.FirstOrDefault();

                if (adminUser == null)
                {
                    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

                    adminUser = new User
                    {
                        FullName = "System Administrator",
                        Username = "admin",
                        PasswordHash = passwordService.HashPassword("admin@123"),
                        UserTypeId = adminUserType.Id,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };

                    await userRepository.Add(adminUser);
                    await userRepository.SaveChangesMethod();
                    _logger.LogInformation("Created admin user");
                }
                else
                {
                    _logger.LogInformation("Admin user already exists");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin user");
                throw;
            }
        }
    }
}
