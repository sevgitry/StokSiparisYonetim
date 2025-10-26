using BusinessLayer.Services;
using EntityLayer.DTOs.Order;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace StokSiparisYonetim.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Siparişler yüklenirken hata: " + ex.Message;
                return View(new System.Collections.Generic.List<OrderResponseDto>());
            }
        }

        // Sipariş detayı
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Sipariş bulunamadı!";
                    return RedirectToAction("Index");
                }
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sipariş yüklenirken hata: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Sipariş iptal et
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id, byte[] rowVersion)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, 3, rowVersion); // 3 = Cancelled

                if (result)
                {
                    TempData["Success"] = "Sipariş başarıyla iptal edildi! Stok iade edildi.";
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

            return RedirectToAction("Index");
        }

        // Sipariş onayla
        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int id, byte[] rowVersion)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, 2, rowVersion); // 2 = Approved

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

            return RedirectToAction("Index");
        }

        private bool IsUserLoggedIn()
        {
            return User.Identity.IsAuthenticated;
        }
    }
}