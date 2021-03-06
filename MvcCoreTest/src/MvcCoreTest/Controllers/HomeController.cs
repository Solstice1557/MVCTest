﻿namespace MvcCoreTest.Controllers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MvcCoreTest.Auth;
    using MvcCoreTest.Model;
    using MvcCoreTest.Utils;

    using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

    [Authorize]
    public class HomeController : Controller
    {
        private const string CaptchaSessionVarName = "CaptchaImageStr";

        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ICaptchaGenerator captchaGenerator;

        public HomeController(
            SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager, 
            ICaptchaGenerator captchaGenerator)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.captchaGenerator = captchaGenerator;
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

            byte[] captchaStringBytes;
            if (!this.HttpContext.Session.TryGetValue(CaptchaSessionVarName, out captchaStringBytes) 
                || captchaStringBytes == null
                || captchaStringBytes.Length == 0)
            {
                model.Captcha = string.Empty;
                this.ModelState.AddModelError("Captcha", "Wrong captcha.");
                return this.View(model);
            }

            var savedCaptchaString = Encoding.UTF8.GetString(captchaStringBytes);
            if (string.IsNullOrEmpty(savedCaptchaString) 
                || !string.Equals(savedCaptchaString, model.Captcha, StringComparison.CurrentCultureIgnoreCase))
            {
                model.Captcha = string.Empty;
                this.ModelState.AddModelError("Captcha", "Wrong captcha.");
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Captcha()
        {
            if (!this.HttpContext.Session.IsAvailable)
            {
                throw new InvalidOperationException();
            }

            var captcha = this.captchaGenerator.Generate();
            this.HttpContext.Session.Set(CaptchaSessionVarName, Encoding.UTF8.GetBytes(captcha.Item2));

            return this.File(captcha.Item1, "image/jpeg");
        }

        [HttpPost]
        public IActionResult Crypt(string textToCrypt)
        {
            var rnd = new Random();
            var key = Enumerable.Range(0, 32).Select(i => (byte)rnd.Next(255)).ToArray();

            var model = new EncryptModel();
            byte[] encrypteContent;
            byte[] iv;
            using (var aes = Aes.Create())
            {
                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(textToCrypt);
                            }

                            iv = aes.IV;

                            encrypteContent = msEncrypt.ToArray();
                            model.EncryptedText = Convert.ToBase64String(encrypteContent);
                        }
                    }
                }
            }
            
            byte[] encryptedKey;
            RSAParameters parameters;
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 2048;
                parameters = rsa.ExportParameters(true);
                encryptedKey = rsa.Encrypt(key, RSAEncryptionPadding.Pkcs1);
                model.EncryptedKey = Convert.ToBase64String(encryptedKey);
            }

            byte[] decryptedKey;
            
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 2048;
                rsa.ImportParameters(parameters);
                decryptedKey = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.Pkcs1);
                model.DecryptedKey = Convert.ToBase64String(decryptedKey);
            }

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(decryptedKey, iv))
                {
                    using (var msDecrypt = new MemoryStream(encrypteContent))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                model.DecryptedText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return this.View(model);
        }

        private async Task<AppUser> GetCurrentUser()
        {
            return await this.userManager.GetUserAsync(this.User);
        }
    }
}
