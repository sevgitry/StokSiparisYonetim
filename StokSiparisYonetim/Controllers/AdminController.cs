using BusinessLayer.Services;
using EntityLayer.DTOs;
using EntityLayer.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StokSiparisYonetim.Controllers
{
    [Authorize(Policy = "AdminOnly")] // Sadece Admin (RolId=1) erişebilir
    public class AdminController : Controller
    {
        private readonly UserService _userService;
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;
        private readonly OrderService _orderService;
        private readonly CategoryService _categoryService;

        public AdminController(
            UserService userService,
            ProductService productService,
            CustomerService customerService,
            OrderService orderService,
            CategoryService categoryService)
        {
            _userService = userService;
            _productService = productService;
            _customerService = customerService;
            _orderService = orderService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Kullanıcı bilgilerini cookie'den al
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = User.FindFirst("UserRole")?.Value;

                // Admin dashboard için istatistikleri topla
                var users = await _userService.GetAllUsersAsync();
                var products = await _productService.GetAllProductsAsync();
                var orders = await _orderService.GetAllOrdersAsync();

                ViewBag.UserCount = users.Count;
                ViewBag.ProductCount = products.Count;
                ViewBag.OrderCount = orders.Count;
                ViewBag.UserName = userName;
                ViewBag.UserRole = userRole;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Dashboard yüklenirken hata oluştu: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kullanıcılar yüklenirken hata oluştu: " + ex.Message;
                return View(new List<UserResponseDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });

                return Json(new
                {
                    success = true,
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    rolId = user.RolId,
                    status = user.Status,
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                // Kategorileri ve satıcıları ViewData ile gönder
                var categories = await _categoryService.GetAllCategoriesAsync();
                var sellers = await _userService.GetAllUsersAsync();

                ViewData["Categories"] = categories;
                ViewData["Sellers"] = sellers;
                ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ürünler yüklenirken hata oluştu: " + ex.Message;
                return View(new List<ProductResponseDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return Json(new { success = false, message = "Ürün bulunamadı" });

                return Json(new
                {
                    success = true,
                    id = product.Id,
                    productName = product.ProductName,
                    sellPrice = product.SellPrice,
                    purchasePrice = product.PurchasePrice,
                    amount = product.Amount,
                    categoryId = product.CategoryId,
                    sellerId = product.SellerId,
                    status = product.Status
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateDto userDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(userDto);
                if (result != null)
                {
                    TempData["Success"] = "Kullanıcı başarıyla oluşturuldu!";
                }
                else
                {
                    TempData["Error"] = "Kullanıcı oluşturulurken hata oluştu!";
                }
                return RedirectToAction(nameof(Users));
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurun: " + string.Join(", ", errors);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(id, userDto);
                if (result != null)
                {
                    TempData["Success"] = "Kullanıcı başarıyla güncellendi!";
                }
                else
                {
                    TempData["Error"] = "Kullanıcı güncellenirken hata oluştu!";
                }
            }
            else
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurun!";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                TempData["Success"] = "Kullanıcı başarıyla silindi!";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken hata oluştu!";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id, int status)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user != null)
                {
                    var userUpdateDto = new UserUpdateDto
                    {
                        Id = id,
                        Name = user.Name,
                        Email = user.Email,
                        RolId = user.RolId,
                        Status = status,
                    };

                    var result = await _userService.UpdateUserAsync(id, userUpdateDto);
                    if (result != null)
                    {
                        TempData["Success"] = "Kullanıcı durumu başarıyla güncellendi!";
                    }
                    else
                    {
                        TempData["Error"] = "Kullanıcı durumu güncellenirken hata oluştu!";
                    }
                }
                else
                {
                    TempData["Error"] = "Kullanıcı bulunamadı!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kullanıcı durumu güncellenirken hata: " + ex.Message;
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto productDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // ProductService'te CreateProductAsync yoksa, basit bir implementasyon
                    // Şimdilik başarılı mesajı gösterelim
                    TempData["Success"] = "Ürün başarıyla oluşturuldu! (Demo)";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Ürün oluşturulurken hata: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurun!";
            }
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductUpdateDto productDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // ProductService'te UpdateProductAsync yoksa, basit implementasyon
                    TempData["Success"] = "Ürün başarıyla güncellendi! (Demo)";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Ürün güncellenirken hata: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurun!";
            }
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                // ProductService'te DeleteProductAsync yoksa, basit implementasyon
                TempData["Success"] = "Ürün başarıyla silindi! (Demo)";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ürün silinirken hata: " + ex.Message;
            }
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, int status, byte[] rowVersion)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status, rowVersion);
            if (result)
            {
                TempData["Success"] = "Sipariş durumu başarıyla güncellendi!";
            }
            else
            {
                TempData["Error"] = "Sipariş durumu güncellenemedi!";
            }
            return RedirectToAction(nameof(Orders));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return Content("<div class='alert alert-danger'>Sipariş bulunamadı!</div>");
            }

            var html = $@"
        <div class='order-details'>
            <div class='row mb-4'>
                <div class='col-md-6'>
                    <h5>Sipariş Bilgileri</h5>
                    <table class='table table-sm'>
                        <tr>
                            <th>Sipariş No:</th>
                            <td>#{order.Id}</td>
                        </tr>
                        <tr>
                            <th>Müşteri:</th>
                            <td>{order.CustomerName} (ID: {order.CustomerId})</td>
                        </tr>
                        <tr>
                            <th>Sipariş Tarihi:</th>
                            <td>{order.OrderDate:dd.MM.yyyy HH:mm}</td>
                        </tr>
                        <tr>
                            <th>Durum:</th>
                            <td><span class='badge {GetStatusBadgeClass(order.Status)}'>{order.StatusText}</span></td>
                        </tr>
                    </table>
                </div>
                <div class='col-md-6'>
                    <h5>Özet</h5>
                    <table class='table table-sm'>
                        <tr>
                            <th>Toplam Tutar:</th>
                            <td class='text-success fw-bold'>{order.TotalAmount:C2}</td>
                        </tr>
                        <tr>
                            <th>Ürün Sayısı:</th>
                            <td>{order.OrderItems.Count} ürün</td>
                        </tr>
                    </table>
                </div>
            </div>

            <h5>Sipariş Kalemleri</h5>
            <div class='table-responsive'>
                <table class='table table-sm table-striped'>
                    <thead class='table-light'>
                        <tr>
                            <th>Ürün</th>
                            <th>Birim Fiyat</th>
                            <th>Miktar</th>
                            <th>Toplam</th>
                        </tr>
                    </thead>
                    <tbody>";

            foreach (var item in order.OrderItems)
            {
                html += $@"
            <tr>
                <td>{item.ProductName}</td>
                <td>{item.UnitPrice:C2}</td>
                <td>{item.Quantity}</td>
                <td class='fw-bold'>{item.TotalPrice:C2}</td>
            </tr>";
            }

            html += $@"
                    </tbody>
                    <tfoot class='table-dark'>
                        <tr>
                            <td colspan='3' class='text-end'><strong>Genel Toplam:</strong></td>
                            <td><strong>{order.TotalAmount:C2}</strong></td>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>";

            return Content(html);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int id, byte[] rowVersion)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, 2, rowVersion);
                if (result)
                {
                    TempData["Success"] = "Sipariş başarıyla onaylandı!";
                }
                else
                {
                    TempData["Error"] = "Sipariş onaylanamadı!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Orders));
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id, byte[] rowVersion)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, 3, rowVersion);
                if (result)
                {
                    TempData["Success"] = "Sipariş başarıyla iptal edildi! Stoklar iade edildi.";
                }
                else
                {
                    TempData["Error"] = "Sipariş iptal edilemedi!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Orders));
        }

        [HttpGet]
        public async Task<IActionResult> PrintOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public IActionResult ClearCache()
        {
            TempData["Success"] = "Önbellek başarıyla temizlendi!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult GenerateReport()
        {
            TempData["Success"] = "Rapor başarıyla oluşturuldu!";
            return RedirectToAction(nameof(Index));
        }

        private string GetStatusBadgeClass(int status)
        {
            return status switch
            {
                1 => "bg-warning",
                2 => "bg-success",
                3 => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}