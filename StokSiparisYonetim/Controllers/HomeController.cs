using BusinessLayer.Services;
using EntityLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StokSiparisYonetim.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private readonly AuthService _authService;

        public HomeController(ProductService productService, AuthService authService)
        {
            _productService = productService;
            _authService = authService;
        }
        // Giriþ kontrolü
        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }

        private IActionResult CheckAuthentication()
        {
            if (!IsAuthenticated())
            {
                TempData["Error"] = "Lütfen önce giriþ yapýn!";
                return RedirectToAction("Login", "Auth");
            }
            return null;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                // Kullanýcý bilgilerini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = User.FindFirst("UserRole")?.Value;

                // Eðer admin giriþ yapmýþsa admin paneline yönlendir
                if (userRole == "1") // Admin
                {
                    return RedirectToAction("Index", "Admin");
                }

                ViewBag.UserName = userName;
                ViewBag.UserRole = userRole;

                // Product verilerini getir
                var products = await _productService.GetAllProductsAsync();

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ana sayfa yüklenirken hata oluþtu: " + ex.Message;
                return View(new List<ProductResponseDto>());
            }
        }

        public IActionResult Privacy()
        {
            // Kullanýcý bilgilerini al
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = User.FindFirst("UserRole")?.Value;

            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            return View();
        }


        private bool IsUserLoggedIn()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return !string.IsNullOrEmpty(userId);
        }
    }
}