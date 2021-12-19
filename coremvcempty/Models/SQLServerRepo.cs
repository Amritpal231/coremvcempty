using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.Models
{
    public class SQLServerRepo : IEmployeeRepo
    {       
        private readonly AppDbContext _context;
        public SQLServerRepo(AppDbContext context)
        {
            _context = context;
        }

        public Employee CreateEmployee(Employee e)
        {
            _context.Employees.Add(e);
            _context.SaveChanges();
            return e;
        }

        public Employee Delete(int Id)
        {
            Employee e = _context.Employees.Find(Id);
            if(e!=null)
            {
                _context.Employees.Remove(e);
                _context.SaveChanges();
            }
            return e;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _context.Employees;
        }

        public Employee GetEmployee(int? Id)
        {
            return _context.Employees.Find(Id);
            
        }

        public Employee Update(Employee emp)
        {
            Employee empnew = _context.Employees.Find(emp.Id);

            _context.Employees.Update(empnew);
            _context.SaveChanges();
            return empnew;
        }
    }
}
