using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.Models
{
    public class MockEmployeeRepo : IEmployeeRepo
    {
        private  List<Employee> emplist;

        public MockEmployeeRepo()
        {
            emplist = new List<Employee>() {
            new Employee {Id=1,Name="Mike",Email="mike@gmail.com", Department=Dept.IT},
            new Employee { Id = 2, Name = "John", Email = "john@gmail.com", Department = Dept.HR },
            new Employee { Id = 3, Name = "Harry", Email = "harry@gmail.com", Department = Dept.Admin},
            new Employee { Id = 5, Name = "Nick", Email = "nick@gmail.com", Department = Dept.IT }
        };
        }

        public Employee CreateEmployee(Employee e)
        {
            e.Id= emplist.Max(e => e.Id) + 1;
            emplist.Add(e);
            return e;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return emplist;
        }

        public Employee GetEmployee(int? Id)
        {
            return emplist.FirstOrDefault(emp=>emp.Id==Id);
        }

        Employee IEmployeeRepo.Delete(int Id)
        {
            throw new NotImplementedException();
        }

        Employee IEmployeeRepo.Update(Employee emp)
        {
            throw new NotImplementedException();
        }
    }
}
