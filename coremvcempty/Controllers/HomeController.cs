using coremvcempty.Models;
using coremvcempty.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace coremvcempty.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepo _employeerepo;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ILogger<HomeController> logger { get; }

        public HomeController(IEmployeeRepo employeeRepo, 
                              IWebHostEnvironment hostingEnvironment,
                              ILogger<HomeController> logger)
        {
            _employeerepo = employeeRepo;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }


        public ViewResult Index()
        {
            ViewBag.PageTitle = "Employee List ";
            return View(_employeerepo.GetAllEmployees());


        }

        public ViewResult Details(int? Id)
        {
            Employee emp = _employeerepo.GetEmployee(Id);
            if(emp==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", Id.Value);
            }
            HomeDetailsVM homeDetailsVM = new HomeDetailsVM
            {
                Employee = _employeerepo.GetEmployee(Id),
                PageTitle = "Employee Details"
            };
            return View(homeDetailsVM);
        }

        [Authorize]
        public ViewResult Edit(int Id)
        {
            //throw new Exception();
            Employee emp = _employeerepo.GetEmployee(Id);
            EditViewModel model = new EditViewModel
            {   
                Id=emp.Id,          
                Name = emp.Name,
                Email = emp.Email,
                Department = emp.Department,
                ExistingPhotoName=emp.PhotoPath,               
            };
            return View(model);
        }

       
        [HttpPost]
        public IActionResult Edit(EditViewModel modal)
        {
            if (ModelState.IsValid)
            {
                Employee emp = _employeerepo.GetEmployee(modal.Id);
                emp.Department = modal.Department;
                emp.Email = modal.Email;
                emp.Name = modal.Name;
              
                if (modal.Photo != null)
                {
                    if(modal.ExistingPhotoName!=null)
                    {
                        string filepath = Path.Combine(hostingEnvironment.WebRootPath, "Images",modal.ExistingPhotoName);

                        System.IO.File.Delete(filepath);
                    }
                    emp.PhotoPath = ProcessUploadedFile(modal);
                    
                }
                else
                {
                    emp.PhotoPath = modal.ExistingPhotoName;
                }

                _employeerepo.Update(emp);



                return RedirectToAction("Index");
            }
           

            return View(modal);
        }


        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        
        public ActionResult Create(EmployeeVM modal)
        {
            if (ModelState.IsValid)
            {
                string filename = ProcessUploadedFile(modal);
                Employee e = new Employee
                {
                    Name = modal.Name,
                    Email = modal.Email,
                    Department = modal.Department,
                    PhotoPath = filename
                };

                _employeerepo.CreateEmployee(e);


                return RedirectToAction("Details", new { Id = e.Id });
            }
            return View();
        }

        private string ProcessUploadedFile(EmployeeVM modal)
        {
            string filename = null;
            if (modal.Photo != null)
            {
                string uploads = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                filename = Guid.NewGuid() + "_" + modal.Photo.FileName;
                string filePath = Path.Combine(uploads, filename);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    modal.Photo.CopyTo(filestream);
                }
            }       

            return filename;
        }
    }
}
