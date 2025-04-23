using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int UserTypeId { get; set; }

        // Navigation Properties
        public UserType UserType { get; set; }
    }
}
