using BCrypt.Net;
using InterviewManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InterviewManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly InterviewManagementContext _context;

        public IndexModel(ILogger<IndexModel> logger, InterviewManagementContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public LoginModel loginModel { get; set; } = default!;

        public class LoginModel
        {
            [Required(ErrorMessage ="UserName is required")]
            public string? Username { get; set; }

            [Required(ErrorMessage ="Password is required")]
            [DataType(DataType.Password)]
            public string? Password { get; set; }
        }

        public void OnGet()
        {

        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid Account.");
        //        return Page();
        //    }
        //    else
        //    {
        //        var user = await _context.Employee.Include(r => r.Role).FirstOrDefaultAsync(a => a.UserName == loginModel.Username);

        //        if (user != null && BCrypt.Net.BCrypt.Verify(loginModel.Password,user.Password))
        //        {
        //            var claims = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.Name, user.Id.ToString()),
        //                new Claim(ClaimTypes.Role, user.Role.RoleName)
        //            };

        //            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //            var authProperties = new AuthenticationProperties
        //            {
        //                IsPersistent = true,
        //                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
        //            };

        //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        //            return RedirectToPage("/ims.recruitment.com/HomePage");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            return Page();
        //        }
        //    }          
        //}
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid Account.");
                return Page();
            }

            var user = await _context.Employee.Include(r => r.Role).FirstOrDefaultAsync(a => a.UserName == loginModel.Username);

            if (user != null)
            {
                _logger.LogInformation("Stored hash for user {Username}: {Password}", user.UserName, user.Password);
                var EncodedPassword = BCrypt.Net.BCrypt.HashPassword("123456789");
                _logger.LogInformation(" Mật khẩu đã mã hóa: {HashedPassword}", EncodedPassword);
                // Verify the password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password);

                if (isPasswordValid)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role.RoleName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToPage("/ims.recruitment.com/HomePage");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/password. Please try again");
                    return Page();
                }
            }          
        }
    }
}
