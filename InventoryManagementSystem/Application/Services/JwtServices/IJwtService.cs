using Domain.ViewModels.JwtUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.JwtServices
{
    public interface IJwtService
    {
        (string token, DateTime expiration) GenerateToken(JwtUserViewModel user);
    }

}
