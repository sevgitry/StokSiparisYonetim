using DataAccessLayer;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace YourProjectName.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Giriş sayfası
        public IActionResult Login()
        {
            // Eğer kullanıcı zaten giriş yapmışsa, rolüne göre yönlendir
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "1")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Giriş işlemi
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Email ve şifre gereklidir.";
                    return View();
                }

                var hashedPassword = HashPassword(password);
                var user = await _context.Users
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == email &&
                                            u.Password == hashedPassword &&
                                            u.Status == 1);

                if (user == null)
                {
                    ViewBag.Error = "Email veya şifre hatalı!";
                    return View();
                }

                // Session'a kullanıcı bilgilerini kaydet
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.RolId.ToString());

                // Rol ID'ye göre yönlendirme
                if (user.RolId == 1) // Admin
                {
                    return RedirectToAction("Index", "Admin");
                }
                else // Kullanıcı
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Giriş sırasında bir hata oluştu: " + ex.Message;
                return View();
            }
        }

        // GET: Kayıt sayfası
        public IActionResult Register()
        {
            return View();
        }

        // POST: Kayıt işlemi
        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Tüm alanlar gereklidir.";
                    return View();
                }

                // Email kontrolü
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    ViewBag.Error = "Bu email adresi zaten kullanılıyor.";
                    return View();
                }

                // Yeni kullanıcı oluştur - RolId otomatik 2 (Kullanıcı)
                var newUser = new User
                {
                    Name = name.Trim(),
                    Email = email.Trim(),
                    Password = HashPassword(password),
                    RolId = 2, // Default olarak 2 (kullanıcı) rolü
                    Status = 1, // Aktif kullanıcı
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                ViewBag.Success = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Kayıt sırasında bir hata oluştu: " + ex.Message;
                return View();
            }
        }

        // Çıkış işlemi - Login sayfasına yönlendir
public IActionResult Logout()
{
    try
    {
        // Session'ı temizle
        HttpContext.Session.Clear();
        
        // TempData ile çıkış mesajı (isteğe bağlı)
        TempData["Success"] = "Başarıyla çıkış yaptınız.";
        
        // Login sayfasına yönlendir
        return RedirectToAction("Login", "Auth");
    }
    catch (Exception ex)
    {
        TempData["Error"] = "Çıkış yapılırken bir hata oluştu: " + ex.Message;
        return RedirectToAction("Login", "Auth");
    }
}

        // Şifre hashleme metodu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}