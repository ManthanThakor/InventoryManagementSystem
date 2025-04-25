using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.JwtUser
{
    public class JwtUserViewModel
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }
}
