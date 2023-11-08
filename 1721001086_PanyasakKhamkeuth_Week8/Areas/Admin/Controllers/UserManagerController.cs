using _1721001086_PanyasakKhamkeuth_Week8.Data;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagerController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _db.Users.ToList();
            var userViewModels = new List<UserViewModel>(); // Use the UserViewModel

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userViewModel = new UserViewModel // Use the UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Address = user.Address,
                    Roles = userRoles
                };
                userViewModels.Add(userViewModel);
            }

            return View(userViewModels);
        }
        public IActionResult Create()
        {
            var userViewModel = new UserViewModel
            {
                RolesList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "User", Text = "User" }
                // Thêm các vai trò khác vào danh sách
            }
            };

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                Address = model.Address
            };

            // Tạo người dùng
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Lưu các vai trò được chọn cho người dùng
                var rolesToAdd = model.Roles.Where(role => role != null).ToList();
                foreach (var role in rolesToAdd)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Nếu có lỗi, cần tạo danh sách các vai trò để truyền đến view
            model.RolesList = new List<SelectListItem>();
            var roles = _roleManager.Roles.ToList();
            foreach (var role in roles)
            {
                model.RolesList.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name
                });
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            // Lấy danh sách tất cả các vai trò từ RoleManager
            var allRoles = _roleManager.Roles.ToList();

            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Address = user.Address,
                Roles = userRoles,
                RolesList = allRoles.Select(role => new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name,
                    Selected = userRoles.Contains(role.Name)
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Name = model.Name;
            user.Address = model.Address;

            // Cập nhật quyền của người dùng dựa trên dữ liệu từ model.Roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(model.Roles).ToList();
            var rolesToAdd = model.Roles.Except(currentRoles).ToList();

            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            // Kiểm tra và cập nhật trạng thái khóa/mở khóa
            if (model.IsLockedOut)
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, false);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // Nếu có lỗi, cần tạo danh sách các vai trò để truyền đến view
            var roles = _roleManager.Roles.ToList();
            model.RolesList = roles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Name
            }).ToList();

            return View(model);
        }
    


        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Address = user.Address,
                Roles = userRoles
            };

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); // Lock vĩnh viễn

            if (result.Succeeded)
            {
                user.IsLockedOut = true; // Cập nhật trạng thái khóa tài khoản
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý lỗi
                return RedirectToAction("Index"); // Hoặc trả về view thông báo lỗi
            }
        }

        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null); // Mở khóa

            if (result.Succeeded)
            {
                user.IsLockedOut = false; // Cập nhật trạng thái mở khóa tài khoản
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý lỗi
                return RedirectToAction("Index"); // Hoặc trả về view thông báo lỗi
            }
        }



        public async Task<IActionResult> LockUnlock(string id, bool lockAccount)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (lockAccount)
            {
                // Khóa tài khoản
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Mở khóa tài khoản
                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            // Xử lý trường hợp lỗi
            return RedirectToAction("Index");
        }


    }
}
