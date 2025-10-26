using BusinessLayer.Services;
using EntityLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace StokSiparisYonetim.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            try
            {
                var cartItems = _cartService.GetCart();
                var cartTotal = _cartService.GetCartTotal();

                ViewBag.CartTotal = cartTotal;
                return View(cartItems);
            }
            catch (Exception ex)
            {
                ViewBag.CartTotal = 0;
                TempData["Error"] = "Sepet yüklenirken bir hata oluştu: " + ex.Message;
                return View(new List<CartItemDto>());
            }
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (request == null || request.ProductId <= 0 || request.Quantity <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Geçersiz ürün bilgisi"
                    });
                }

                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Lütfen önce giriş yapın"
                    });
                }

                var userRole = User.FindFirst("UserRole")?.Value;
                if (userRole != "2")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bu işlem için yetkiniz yok"
                    });
                }

                var cartItem = new CartItemDto
                {
                    ProductId = request.ProductId,
                    ProductName = request.ProductName,
                    UnitPrice = request.UnitPrice,
                    Quantity = request.Quantity,
                    MaxStock = request.MaxStock,
                    ImageUrl = request.ImageUrl ?? ""
                };

                _cartService.AddToCart(cartItem);

                var itemCount = _cartService.GetCartItemCount();
                var cartTotal = _cartService.GetCartTotal();

                return Json(new
                {
                    success = true,
                    itemCount = itemCount,
                    cartTotal = cartTotal,
                    message = $"{request.Quantity} adet {request.ProductName} sepete eklendi"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            try
            {
                _cartService.UpdateCart(productId, quantity);
                var itemCount = _cartService.GetCartItemCount();
                return Json(new { success = true, itemCount = itemCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                _cartService.RemoveFromCart(productId);
                var itemCount = _cartService.GetCartItemCount();
                return Json(new { success = true, itemCount = itemCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            try
            {
                var cartItems = _cartService.GetCart();
                if (!cartItems.Any())
                {
                    TempData["Error"] = "Sepetiniz boş!";
                    return RedirectToAction("Index");
                }

                _cartService.ClearCart();
                TempData["Success"] = "Siparişiniz başarıyla oluşturuldu!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sipariş oluşturulurken hata: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(0);
                }

                var userRole = User.FindFirst("UserRole")?.Value;
                if (userRole != "2")
                {
                    return Json(0);
                }

                var count = _cartService.GetCartItemCount();
                return Json(count);
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; }
        public string? ImageUrl { get; set; }
    }
}