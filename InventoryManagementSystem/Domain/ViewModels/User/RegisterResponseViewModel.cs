using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.User
{
    public class RegisterResponseViewModel
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid UserTypeId { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
    }

    public class CustomerRegisterResponseViewModel : RegisterResponseViewModel
    {
        public Guid CustomerId { get; set; }
    }

    public class SupplierRegisterResponseViewModel : RegisterResponseViewModel
    {
        public Guid SupplierId { get; set; }
    }
}
