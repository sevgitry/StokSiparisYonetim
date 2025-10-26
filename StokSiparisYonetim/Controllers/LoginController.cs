using BusinessLayer.Services;
using EntityLayer.DTOs;
using EntityLayer.DTOs.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StokSiparisYonetim.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public LoginController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.Login(loginDto.Email, loginDto.Password);
                if (result)
                {
                    var user = await _authService.GetUserByEmailAsync(loginDto.Email);
                    if (user != null)
                    {
                        // Claims (kullanıcı bilgileri) oluştur
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim("UserRole", user.RolId.ToString()),
                            new Claim("UserName", user.Name)
                        };

                        // ClaimsIdentity oluştur
                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Authentication properties
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true, // "Beni hatırla" özelliği
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7), // 7 gün
                            RedirectUri = "/Home/Index"
                        };

                        // Sign in (cookie oluştur)
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        // Session'da da tutabilirsiniz (isteğe bağlı)
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("UserName", user.Name);
                        HttpContext.Session.SetString("UserRole", user.RolId.ToString());

                        // Rol'e göre yönlendirme
                        if (user.RolId == 1) // Admin
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else // Müşteri
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ModelState.AddModelError("", "Geçersiz email veya şifre");
            }

            return View("Index", loginDto);
        }

        // GET: Register sayfası
        public IActionResult Register()
        {
            // Eğer kullanıcı zaten giriş yapmışsa ana sayfaya yönlendir
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Register işlemi
        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            try
            {
                // Validasyon
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Tüm alanlar gereklidir.";
                    return View();
                }

                if (password.Length < 6)
                {
                    ViewBag.Error = "Şifre en az 6 karakter olmalıdır.";
                    return View();
                }

                // Email kontrolü
                var existingUser = await _authService.GetUserByEmailAsync(email);
                if (existingUser != null)
                {
                    ViewBag.Error = "Bu email adresi zaten kullanılıyor.";
                    return View();
                }

                // Yeni kullanıcı DTO'su oluştur
                var userDto = new UserCreateDto
                {
                    Name = name.Trim(),
                    Email = email.Trim().ToLower(),
                    Password = password,
                    RolId = 2, // Müşteri rolü
                    Status = 1 // Aktif
                };

                // Kullanıcıyı oluştur
                var result = await _userService.CreateUserAsync(userDto);

                if (result != null)
                {
                    TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Kayıt işlemi sırasında bir hata oluştu.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Kayıt işlemi sırasında bir hata oluştu: " + ex.Message;
                return View();
            }
        }

        // Çıkış işlemi - Cookie temizleme
        public async Task<IActionResult> Logout()
        {
            // Cookie'yi temizle
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Session'ı temizle
            HttpContext.Session.Clear();

            TempData["Success"] = "Başarıyla çıkış yaptınız.";
            return RedirectToAction("Index");
        }

        // Erişim reddedildi sayfası
        public IActionResult AccessDenied()
        {
            // Kullanıcı bilgilerini ViewBag'e aktar
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.UserRole = User.FindFirst("UserRole")?.Value;
                ViewBag.UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            }

            return View();

        }
    }
}
