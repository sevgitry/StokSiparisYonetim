using Microsoft.EntityFrameworkCore;
using System;
using EntityLayer;

namespace DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-EQOD4HE;Database=StokYonetim;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        // DbSet'ler
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Order ilişkileri
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // User entity'si için Rol ilişkisi
            modelBuilder.Entity<User>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product entity'si için ilişkiler
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany()
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // RowVersion konfigürasyonu
            modelBuilder.Entity<Order>()
                .Property(o => o.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Customer>()
                .Property(c => c.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Product>()
                .Property(p => p.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<User>()
                .Property(u => u.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Category>()
                .Property(c => c.RowVersion)
                .IsRowVersion();

            // Rol seed data - RowVersion gerekmez çünkü timestamp değil
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Name = "Admin" },
                new Rol { Id = 2, Name = "User" }
            );

            // Admin user seed data - RowVersion boş array olarak verildi
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "admin@admin.com",
                    Password = "admin123",
                    RolId = 1,
                    Status = 1,
                    CreatedDate = new DateTime(2024, 1, 24),
                    RowVersion = new byte[] { 0 } // RowVersion eklendi
                }
            );

            // Customer Status default değeri
            modelBuilder.Entity<Customer>()
                .Property(c => c.Status)
                .HasDefaultValue(1); // Active

            // Order Status default değeri
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue(1); // Draft

            // User Status default değeri
            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasDefaultValue(1); // Active

            // Category Status default değeri
            modelBuilder.Entity<Category>()
                .Property(c => c.Status)
                .HasDefaultValue(1); // Active
        }
    }
}