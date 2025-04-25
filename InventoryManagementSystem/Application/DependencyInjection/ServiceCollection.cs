using Application.Services.AuthServices;
using Application.Services.CategoryServices;
using Application.Services.JwtServices;
using Application.Services.PasswordServices;
using Application.Services.UserTypeServices;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DependencyInjection
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserTypeService, UserTypeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<IItemService, ItemService>();
            //services.AddScoped<ICustomerService, CustomerService>();
            //services.AddScoped<ISupplierService, SupplierService>();
            //services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            //services.AddScoped<ISalesOrderService, SalesOrderService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IAdminService, AdminService>();

            return services;
        }
    }
}
