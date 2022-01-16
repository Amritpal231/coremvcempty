using coremvcempty.Models;
using coremvcempty.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.Controllers
{
    //[Authorize(Roles ="Admin")]
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleVM model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole { Name = model.RoleName };
                var result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetRolesList");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult GetRolesList()
        {
            var roles = roleManager.Roles;

            return View(roles);

        }



        [HttpGet]
        public IActionResult GetUsersList()
        {
            var users=userManager.Users;

            return View(users);

        }


        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role {id} Not Found";
                return View("NotFoundPage");
            }

            var model = new EditRoleVM
            {
                Id = role.Id,
                RoleName = role.Name
            };

            //foreach (var user in userManager.Users)
            //{
            //    if (await userManager.IsInRoleAsync(user,role.Name))
            //    {
            //        model.UsersList.Add(user.UserName);
            //    }
            //}

            foreach (var users in await userManager.GetUsersInRoleAsync(role.Name))
            {
                model.UsersList.Add(users.UserName);
            }


            return View(model);

        }



        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleVM model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role {model.Id} Not Found";
                return View("NotFoundPage");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetRolesList");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUsersInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                var model = new List<AddOrRemoveUsersinRoleVM>();

                foreach (var user in await userManager.Users.ToListAsync())
                {
                    AddOrRemoveUsersinRoleVM addOrRemoveUsersinRoleVM = new AddOrRemoveUsersinRoleVM
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };

                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        addOrRemoveUsersinRoleVM.IsSelected = true;
                    }
                    else
                    {
                        addOrRemoveUsersinRoleVM.IsSelected = false;
                    }

                    model.Add(addOrRemoveUsersinRoleVM);

                }
                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = $"Role {roleId} Not Found";
                return View("NotFoundPage");
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsersInRole(List<AddOrRemoveUsersinRoleVM> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role {roleId} Not Found";
                return View("NotFoundPage");
            }
            else
            {
                foreach (var users in model)
                {
                    var user = await userManager.FindByIdAsync(users.UserId);

                    IdentityResult result=null;

                    if (users.IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                    {
                         result = await userManager.AddToRoleAsync(user, role.Name);
                    }
                    else if (!users.IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                    {
                         result = await userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else { continue; }
                    
                    if(result!= null && !result.Succeeded)
                    {
                        ModelState.AddModelError("","");
                        return View(model);                    
                    }
                   
                }
                return RedirectToAction("EditRole",new { Id = roleId });
            }

           
        }
    }
}
