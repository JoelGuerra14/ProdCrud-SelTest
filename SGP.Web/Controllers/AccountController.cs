using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SGP.Application.Interfaces;
using SGP.Domain.Entities;
using System.Security.Claims;

namespace SGP.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Products");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            var result = await _userRepository.ValidateUserAsync(usuario, password);

            if (result.IsSuccess)
            {
                var user = (User)result.Data;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("Id", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = false });

                return RedirectToAction("Index", "Products");
            }

            ViewBag.Error = result.Message;
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.RegisterAsync(user);
                if (result.IsSuccess)
                {
                    return RedirectToAction("Login");
                }
                ViewBag.Error = result.Message;
            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}