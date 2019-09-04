using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserSystemWb.DAL;
using UserSystemWb.Models;
using UserSystemWb.ViewModels;
using UserSystemWb.Utilies;

namespace UserSystemWb.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _context;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Register(RegisterViewModel RegisterViewModel)
        {
            if (!ModelState.IsValid) return View(RegisterViewModel);

            AppUser newUser = new AppUser()
            {
                Name = RegisterViewModel.Name,
                Surname = RegisterViewModel.Surname,
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Username,
            };

            IdentityResult identityResult = await _userManager.CreateAsync(newUser,RegisterViewModel.Password);

            if(!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(RegisterViewModel);
            }

             await _userManager.AddToRoleAsync(newUser, Utility.Roles.Member.ToString());

            await _signInManager.SignInAsync(newUser, true);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();

            return RedirectToAction("Index","Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);
            AppUser user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if(user == null)
            {
                ModelState.AddModelError("", "Email or password wrong");
                return View(loginViewModel);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync
                (user,loginViewModel.Password,loginViewModel.RememberMe,true);

            if(!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or password wrong");
                return View(loginViewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task RoleSeeder()
        {
           
            if (!await _roleManager.RoleExistsAsync(Utility.Roles.Admin.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(Utility.Roles.Admin.ToString()));
            }
            if (!await _roleManager.RoleExistsAsync(Utility.Roles.Member.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(Utility.Roles.Member.ToString()));
            }
        }

    }
}