using coremvcempty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.ViewModels
{
    public class EditViewModel: EmployeeVM
    {
        public int Id { get; set; }
        public string ExistingPhotoName { get; set; }
    }
}
