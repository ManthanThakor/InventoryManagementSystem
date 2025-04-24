using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.UserTypes
{
    public class UserTypeViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class CreateUserTypeViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must be between {2} and {1} characters", MinimumLength = 2)]
        public string Name { get; set; }
    }

    public class UpdateUserTypeViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must be between {2} and {1} characters", MinimumLength = 2)]
        public string Name { get; set; }
    }
}
