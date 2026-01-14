using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.Services;


namespace mymusic_app.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ------------------- WEB REGISTER ----------------
        [HttpGet("Register")]
        public IActionResult Register() => View();

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] string email, [FromForm] string password,
            [FromForm] string firstName, [FromForm] string lastName,
            [FromForm] DateTime dob, [FromForm] string gender)
        {
            try
            {
                // Register as regular user (isAdmin = false) and provide default Bio
                var user = await _authService.RegisterAsync(email, password, firstName, lastName, dob, gender, false);

                // Sign in user with cookie
                await _authService.SignInAsync(user, HttpContext);

                // Redirect based on role
                if (user.IsAdmin)
                    return RedirectToAction("Index", "Admin"); // Admin dashboard

                return RedirectToAction("Dashboard", "Account"); // regular users
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // ------------------- WEB LOGIN ----------------
        [HttpGet("Login")]
        public IActionResult Login() => View();

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            var user = await _authService.LoginAsync(email, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }

            await _authService.SignInAsync(user, HttpContext);

            // Redirect based on role
            if (user.IsAdmin)
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Dashboard", "Account");
        }

        // ------------------- WEB LOGOUT ----------------
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync(HttpContext);
            return RedirectToAction("Login");
        }

        // ------------------- WEB ACCESS DENIED ----------------
        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied() => View();

        // ------------------- API REGISTER (JSON) ----------------
        [HttpPost("ApiRegister")]
        public async Task<IActionResult> ApiRegister([FromBody] RegisterRequest request)
        {
            try
            {
                // Default Bio to empty string
                var user = await _authService.RegisterAsync(
                    request.Email, request.Password,
                    request.FirstName, request.LastName,
                    request.DateOfBirth, request.Gender,
                    isAdmin: false
                );

                var token = _authService.GenerateJwtToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ------------------- API LOGIN (JSON) ----------------
        [HttpPost("ApiLogin")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginRequest request)
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        // ------------------- DTOs ----------------
        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
