using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.Models
{
    public interface IEmployeeRepo
    {
        Employee GetEmployee(int? Id);

        IEnumerable<Employee> GetAllEmployees();

        Employee CreateEmployee(Employee e);

        Employee Delete(int Id);

        Employee Update(Employee emp);
    }
}
