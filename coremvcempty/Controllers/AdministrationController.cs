using coremvcempty.Models;
using coremvcempty.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace coremvcempty.Controllers
{
    [Authorize(Roles ="Admin")]
    //[Authorize]
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
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if(user==null)
            {
                ViewBag.ErrorMessage = $"User {id} Not Found";
                return View("NotFoundPage");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            var model = new EditUserVM
            {
                Id=user.Id,
                UserName = user.UserName,
                City = user.City,
                Email = user.Email,
                Roles = userRoles,
                Claims = userClaims.Select(c => c.Value).ToList()
            };

            
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User {id} Not Found";
                return View("NotFoundPage");
            }

            else
            {
                var result = await userManager.DeleteAsync(user);

                if(result.Succeeded)
                {
                    return RedirectToAction("GetUsersList");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Not Valid", error.Description);
                }
                return View("GetUsersList");
            }
                 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role {id} Not Found";
                return View("NotFoundPage");
            }

            else
            {
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetRolesList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Not Valid", error.Description);
                }
                return View("GetRolesList");
            }

        }


        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User {model.Id} Not Found";
                return View("NotFoundPage");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    return RedirectToAction("GetUsersList");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User {Id} Not Found";
                return View("NotFoundPage");
            }

            var model = new List<ManageUserRolesVM>();

            foreach(var role in await roleManager.Roles.ToListAsync())
            {
                var manageUserRolesVM = new ManageUserRolesVM
                {
                    RoleId = role.Id,
                    RoleName = role.Name             
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    manageUserRolesVM.isSelected = true;
                }
                else
                    manageUserRolesVM.isSelected = false;

                model.Add(manageUserRolesVM);

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<ManageUserRolesVM> model, string Id)
        {
            var user =await  userManager.FindByIdAsync(Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User {Id} Not Found";
                return View("NotFoundPage");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(x => x.isSelected==true).Select(y => y.RoleName));
          

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add  roles");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = Id });
        }



        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id{Id} not found";
                return View("NotFoundPage");
            }

            var existingClaims = await userManager.GetClaimsAsync(user);

            var model = new ManageUserClaimsVM()
            {
                UserId = Id
            };


            foreach(var claim in ClaimStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim()
                {
                    ClaimType = claim.Type,
                };

                if (existingClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return View(model);       

        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(ManageUserClaimsVM model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User {model.UserId} Not Found";
                return View("NotFoundPage");
            }

            var claims = await userManager.GetClaimsAsync(user);

            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user,model.Claims.Where(c=>c.IsSelected==true).Select(c=>new Claim(c.ClaimType,c.ClaimType)));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add  claims");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });
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
