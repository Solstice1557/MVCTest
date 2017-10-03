namespace MvcCoreTest.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MvcCoreTest.Auth;
    using MvcCoreTest.Model;

    using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public HomeController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result =
                await
                this.signInManager.PasswordSignInAsync(
                    model.Login,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);
            if (result == SignInResult.Success)
            {
                return this.LocalRedirect(returnUrl ?? "/");
            }

            if (result == SignInResult.LockedOut)
            {
                return this.View("Lockout");
            }

            this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return this.RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await this.GetCurrentUser();
            return this.View(user);
        }

        [HttpGet]
        [Authorize(Roles = "master")]
        public IActionResult Manager()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return this.View();
        }

        private async Task<AppUser> GetCurrentUser()
        {
            return await this.userManager.GetUserAsync(this.User);
        }
    }
}
