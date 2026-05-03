using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Models;

namespace ScanPoint.Models.Data
{
    public class ScanPointDbContext : DbContext
    {
        public ScanPointDbContext(DbContextOptions<ScanPointDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================
            // SOFT DELETE — Global Query Filter
            // Automatikisht filtron IsDeleted=true nga të gjitha queries
            // ✅ GetByIdAsync, GetAllAsync, GetByAdminIdAsync — asnjëra nuk do kthejë shop të fshirë
            // ⚠️  Nëse dëshiron të shohësh edhe të fshirat: _context.Shops.IgnoreQueryFilters()
            // ===============================
            modelBuilder.Entity<Shop>().HasQueryFilter(s => !s.IsDeleted);

            // Soft delete filter për Users (Manager / Cashier)
            // ⚠️  Admin nuk filtrohet këtu — Admin ka IsDeleted=false gjithmonë
            // Përdor IgnoreQueryFilters() kur duhet të shohësh të fshirat
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

            // ===============================
            // TPT – User inheritance
            // ===============================
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Manager>().ToTable("Managers");
            modelBuilder.Entity<Cashier>().ToTable("Cashiers");

         

           

            modelBuilder.Entity<Product>()
       .HasIndex(p => new { p.Barcode, p.ShopId })
       .IsUnique();
            // ===============================
            // Shop relations
            // ===============================
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Managers)
                .WithOne(m => m.Shop)
                .HasForeignKey(m => m.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Cashiers)
                .WithOne(c => c.Shop)
                .HasForeignKey(c => c.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Shop)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Invoices)
                .WithOne(i => i.Shop)
                .HasForeignKey(i => i.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Shop>()
                .HasOne(s => s.Admin)
                .WithMany(u => u.Shops)
                .HasForeignKey(s => s.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shop>()
    .HasIndex(s => new { s.Name, s.AdminId })
    .IsUnique();

            // ===============================
            // Manager → Cashiers
            // ===============================
            modelBuilder.Entity<Manager>()
                .HasMany(m => m.ManagedCashiers)
                .WithOne(c => c.Manager)
                .HasForeignKey(c => c.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // Cashier → Invoices
            // ===============================
            modelBuilder.Entity<Cashier>()
                .HasMany(c => c.Invoices)
                .WithOne(i => i.Cashier)
                .HasForeignKey(i => i.CashierId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // InvoiceItem relations
            // ===============================
            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Product)
                .WithMany(p => p.InvoiceItems)
                .HasForeignKey(ii => ii.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // User → Shop (TPT safe)
            // ===============================
            modelBuilder.Entity<User>()
                .HasOne(u => u.Shop)
                .WithMany()
                .HasForeignKey(u => u.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            

           
            modelBuilder.Entity<Product>()
                .Property(p => p.Price).HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount).HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.Price).HasPrecision(18, 2);


            // ===============================
            // INDEXES (PERFORMANCE BOOST 🚀)
            // ===============================

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.ShopId);

            // Cashier
            modelBuilder.Entity<Cashier>()
                .HasIndex(c => c.ManagerId);

            // Shop
            modelBuilder.Entity<Shop>()
                .HasIndex(s => s.AdminId);

            // Product
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ShopId);

            // Invoice
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.ShopId);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.CashierId);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.CreatedAt);

            // Composite (raporte)
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => new { i.ShopId, i.CreatedAt });

            // InvoiceItem
            modelBuilder.Entity<InvoiceItem>()
                .HasIndex(ii => ii.InvoiceId);

            modelBuilder.Entity<InvoiceItem>()
                .HasIndex(ii => ii.ProductId);
        }
    }
}