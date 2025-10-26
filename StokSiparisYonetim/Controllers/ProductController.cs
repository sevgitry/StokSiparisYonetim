using BusinessLayer.Services;
using EntityLayer.DTOs;
using EntityLayer.DTOs.Order;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StokSiparisYonetim.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;
        private readonly OrderService _orderService;

        public ProductController(ProductService productService, CustomerService customerService, OrderService orderService)
        {
            _productService = productService;
            _customerService = customerService;
            _orderService = orderService;
        }

        // GET: Product/CreateOrder/5 - Sipariş oluşturma sayfası
        [HttpGet]
        public async Task<IActionResult> CreateOrder(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Ürün bulunamadı!";
                return RedirectToAction("Index", "Home");
            }

            var customers = await _customerService.GetAllCustomersAsync();
            ViewBag.Customers = customers;

            var model = new ProductOrderViewModel
            {
                Product = product,
                Quantity = 1,
                CustomerId = customers.FirstOrDefault()?.Id ?? 0
            };

            return View(model);
        }

        // POST: Product/CreateOrder - Sipariş oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateOrder(ProductOrderViewModel model)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Order DTO oluştur
                    var orderDto = new OrderCreateDto
                    {
                        CustomerId = model.CustomerId,
                        OrderDate = DateTime.Now,
                        OrderItems = new List<OrderItemCreateDto>
                        {
                            new OrderItemCreateDto
                            {
                                ProductId = model.Product.Id,
                                Quantity = model.Quantity,
                                UnitPrice = (decimal)model.Product.SellPrice
                            }
                        }
                    };

                    // Order oluştur (stok otomatik düşecek)
                    var order = await _orderService.CreateOrderAsync(orderDto);

                    if (order != null)
                    {
                        TempData["Success"] = $"{model.Quantity} adet {model.Product.ProductName} siparişi başarıyla oluşturuldu! Stok güncellendi.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Error"] = "Sipariş oluşturulamadı!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            // Hata durumunda view'i tekrar yükle
            var customers = await _customerService.GetAllCustomersAsync();
            ViewBag.Customers = customers;
            model.Product = await _productService.GetProductByIdAsync(model.Product.Id);

            return View(model);
        }

        // Hızlı sipariş (Ana sayfadaki formdan)
        [HttpPost]
        public async Task<IActionResult> QuickOrder(int productId, int quantity)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Varsayılan müşteri ID'si
                var customers = await _customerService.GetAllCustomersAsync();
                var defaultCustomerId = customers.FirstOrDefault()?.Id ?? 1;

                // Product'ı getir
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    TempData["Error"] = "Ürün bulunamadı!";
                    return RedirectToAction("Index", "Home");
                }

                // Order DTO oluştur
                var orderDto = new OrderCreateDto
                {
                    CustomerId = defaultCustomerId,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItemCreateDto>
            {
                new OrderItemCreateDto
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = (decimal)product.SellPrice // Explicit conversion
                }
            }
                };

                // Order oluştur (stok otomatik düşecek)
                var order = await _orderService.CreateOrderAsync(orderDto);

                if (order != null)
                {
                    TempData["Success"] = $"{quantity} adet {product.ProductName} siparişi oluşturuldu! Stok güncellendi.";
                }
                else
                {
                    TempData["Error"] = "Sipariş oluşturulamadı!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        private bool IsUserLoggedIn()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return !string.IsNullOrEmpty(userId);
        }
    }

    // ViewModel
    public class ProductOrderViewModel
    {
        public ProductResponseDto Product { get; set; }

        [Required(ErrorMessage = "Miktar gereklidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den büyük olmalıdır")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Müşteri seçimi gereklidir")]
        public int CustomerId { get; set; }
    }
}