using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.ViewModels
{
    public class EditUserVM
    {
        public EditUserVM()
        {
            Roles = new List<string>();
            Claims = new List<string>();
        }
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required][EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }

        public IList<string> Roles { get; set; } 

        public IList<string> Claims { get; set; }
    }
}
