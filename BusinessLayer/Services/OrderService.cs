using DataAccessLayer;
using EntityLayer;
using EntityLayer.DTOs;
using EntityLayer.DTOs.Order;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;

        // Constructor'ı public yap
        public OrderService(ApplicationDbContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        // Order oluştur ve stok düşür
        public async Task<OrderResponseDto?> CreateOrderAsync(OrderCreateDto orderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Stok kontrolü ve düşürme
                foreach (var item in orderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                        throw new Exception($"{item.ProductId} ID'li ürün bulunamadı!");

                    if (product.Amount < item.Quantity)
                        throw new Exception($"{product.ProductName} ürünü için yeterli stok yok! Mevcut stok: {product.Amount}, İstenen: {item.Quantity}");

                    // Stok düşür
                    product.Amount -= item.Quantity;
                }

                // Order oluştur
                var order = new Order
                {
                    CustomerId = orderDto.CustomerId,
                    OrderDate = orderDto.OrderDate,
                    Status = 1, // Draft
                    OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetOrderByIdAsync(order.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Sipariş oluşturulurken hata: {ex.Message}");
            }
        }

        // Order durumunu güncelle (Onayla/İptal Et)
        public async Task<bool> UpdateOrderStatusAsync(int orderId, int newStatus, byte[] rowVersion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null) return false;

                // Concurrency kontrol
                _context.Entry(order).Property(o => o.RowVersion).OriginalValue = rowVersion;

                var oldStatus = order.Status;
                order.Status = newStatus;

                // Eğer sipariş onaylanıyorsa ve önceden onaylanmamışsa
                if (newStatus == 2 && oldStatus != 2) // Approved
                {
                    // Stok zaten CreateOrder'da düşürüldü, burada ek işlem yapmaya gerek yok
                }
                // Eğer sipariş iptal ediliyorsa ve önceden onaylanmışsa
                else if (newStatus == 3 && oldStatus == 2) // Cancelled from Approved
                {
                    // Stok iadesi yap
                    foreach (var item in order.OrderItems)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            product.Amount += item.Quantity;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new Exception("Sipariş başka bir kullanıcı tarafından güncellenmiş. Lütfen verileri yenileyin.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Sipariş durumu güncellenirken hata: {ex.Message}");
            }
        }

        // Tüm order'ları getir
        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name ?? "Unknown",
                OrderDate = o.OrderDate,
                Status = o.Status,
                StatusText = StatusHelper.GetOrderStatusText(o.Status),
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
                RowVersion = o.RowVersion
            }).ToList();
        }

        // Order'ı ID ile getir
        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name ?? "Unknown",
                OrderDate = order.OrderDate,
                Status = order.Status,
                StatusText = StatusHelper.GetOrderStatusText(order.Status),
                TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
                RowVersion = order.RowVersion
            };
        }
    }
}