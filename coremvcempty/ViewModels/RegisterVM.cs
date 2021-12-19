using coremvcempty.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [EmailAddress]
        [Remote("IsEmailInUse", "Account")]
        [CustomEmailValidation(allowedeDomain:"gmail.com",ErrorMessage ="This domain name is not allowed")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="Passwords must match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string City { get; set; }


    }
}
