using coremvcempty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.ViewModels
{
    public class EditRoleVM
    {
        public string Id { get; set; }

        public string RoleName { get; set; }

        public List<string> UsersList { get; set; } = new List<string>();
    }
}
