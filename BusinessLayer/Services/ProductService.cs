using DataAccessLayer;
using EntityLayer;
using EntityLayer.DTOs;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Where(p => p.Status == 1)
                .ToListAsync();

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                SellPrice = p.SellPrice,
                PurchasePrice = p.PurchasePrice,
                Amount = p.Amount,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "Unknown",
                SellerId = p.SellerId,
                SellerName = p.Seller?.Name ?? "Unknown",
                CreatedDate = p.CreatedDate,
                Status = p.Status,
                StatusText = StatusHelper.GetCustomerStatusText(p.Status)
            }).ToList();
        }

        public async Task<bool> CreateProductAsync(ProductCreateDto productDto)
        {
            try
            {
                var product = new Product
                {
                    ProductName = productDto.ProductName,
                    SellPrice = productDto.SellPrice,
                    PurchasePrice = productDto.PurchasePrice,
                    Amount = productDto.Amount,
                    CategoryId = productDto.CategoryId,
                    SellerId = productDto.SellerId,
                    Status = productDto.Status
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                SellPrice = product.SellPrice,
                PurchasePrice = product.PurchasePrice,
                Amount = product.Amount,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "Unknown",
                SellerId = product.SellerId,
                SellerName = product.Seller?.Name ?? "Unknown",
                CreatedDate = product.CreatedDate,
                Status = product.Status,
                StatusText = StatusHelper.GetCustomerStatusText(product.Status)
            };
        }

        public async Task<bool> UpdateProductAsync(ProductUpdateDto productDto)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productDto.Id);
                if (product == null) return false;

                product.ProductName = productDto.ProductName;
                product.SellPrice = productDto.SellPrice;
                product.PurchasePrice = productDto.PurchasePrice;
                product.Amount = productDto.Amount;
                product.CategoryId = productDto.CategoryId;
                product.SellerId = productDto.SellerId;
                product.Status = productDto.Status;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return false;

                product.Status = 0; // Passive
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        // Product için hızlı sipariş oluşturma
        public async Task<bool> CreateOrderForProductAsync(int productId, int quantity, int customerId)
        {
            try
            {
                // Product'ı getir ve stok kontrolü yap
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    throw new Exception("Ürün bulunamadı!");

                if (product.Amount < quantity)
                    throw new Exception($"{product.ProductName} ürünü için yeterli stok yok! Mevcut stok: {product.Amount}");

                // Order oluştur
                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    Status = 1, // Draft
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = productId,
                            Quantity = quantity,
                            UnitPrice = (decimal)product.SellPrice
                        }
                    }
                };

                _context.Orders.Add(order);

                // Stok güncelle (opsiyonel - onaylandığında düşürmek daha iyi)
                // product.Amount -= quantity;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Sipariş oluşturulurken hata: {ex.Message}");
            }
        }
        // Stok güncelleme metodu
        public async Task<bool> UpdateProductStockAsync(int productId, int newAmount)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return false;

                product.Amount = newAmount;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Stok düşürme metodu
        public async Task<bool> DecreaseProductStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null || product.Amount < quantity)
                    return false;

                product.Amount -= quantity;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Stok artırma metodu (sipariş iptalinde kullanılır)
        public async Task<bool> IncreaseProductStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return false;

                product.Amount += quantity;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

