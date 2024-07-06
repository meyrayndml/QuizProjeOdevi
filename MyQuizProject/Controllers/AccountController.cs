using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MyQuizProject.Models;

namespace MyQuizProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuizDbContext _context;

        public AccountController(QuizDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] User userData)
        {
            if (userData == null)
            {
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View(userData);
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userData.Username && x.Password == userData.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(userData);
            }

            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, userData.Username),
            new Claim("OturumAçmaZamanı", DateTime.Now.ToString("dd MM yyyy hh:mm:ss"))
            };

            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);
            await HttpContext.SignInAsync(claimPrincipal);

            // Username'i session'a kaydet
            HttpContext.Session.SetString("Username", userData.Username);

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear(); // Session'ı temizle
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([FromForm] User newUser)
        {
            if (newUser == null)
            {
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            var existingUser = _context.Users.FirstOrDefault(x => x.Username == newUser.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(newUser);
            }

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
    }
}
