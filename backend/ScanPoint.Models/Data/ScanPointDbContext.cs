using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Models;
using ScanPoint.Models.Models;

namespace ScanPoint.Models.Data
{
    public class ScanPointDbContext : DbContext
    {
        public ScanPointDbContext(DbContextOptions<ScanPointDbContext> options)  : base(options)
        {
        }

        // ===============================
        // DBSets
        // ===============================
        public DbSet<User> Users { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        ///===================================================== kjo krijon tabelen( e detyryshme per cdo entitet)
        public DbSet<Shkolla> Shkollat { get; set; }
        public DbSet<Nxenesi> Nxenesit { get; set; }


        public DbSet<Punetori> Punetoret { get; set; }

        public DbSet<Fabrika> Fabrikat { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================
            // GLOBAL QUERY FILTERS (SOFT DELETE)
            // ===============================
            modelBuilder.Entity<Shop>()
                .HasQueryFilter(s => !s.IsDeleted);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            // ===============================
            // TPT INHERITANCE
            // ===============================
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Manager>().ToTable("Managers");
            modelBuilder.Entity<Cashier>().ToTable("Cashiers");

            // ===============================
            // UNIQUE / INDEXES
            // ===============================
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Shop>()
                .HasIndex(s => new { s.Name, s.AdminId })
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.Barcode, p.ShopId })
                .IsUnique();

            // ===============================
            // RELATIONS - SHOP
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

            // ===============================
            // MANAGER - CASHIER
            // ===============================
            modelBuilder.Entity<Manager>()
                .HasMany(m => m.ManagedCashiers)
                .WithOne(c => c.Manager)
                .HasForeignKey(c => c.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CASHIER - INVOICE
            // ===============================
            modelBuilder.Entity<Cashier>()
                .HasMany(c => c.Invoices)
                .WithOne(i => i.Cashier)
                .HasForeignKey(i => i.CashierId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // INVOICE ITEM RELATIONS
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
            // USER - SHOP (TPT SAFE)
            // ===============================
            modelBuilder.Entity<User>()
                .HasOne(u => u.Shop)
                .WithMany()
                .HasForeignKey(u => u.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // PRECISION FIELDS
            // ===============================
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            // ===============================
            // PERFORMANCE INDEXES
            // ===============================
            modelBuilder.Entity<Cashier>()
                .HasIndex(c => c.ManagerId);

            modelBuilder.Entity<Shop>()
                .HasIndex(s => s.AdminId);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ShopId);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.ShopId);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.CashierId);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.CreatedAt);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => new { i.ShopId, i.CreatedAt });

            modelBuilder.Entity<InvoiceItem>()
                .HasIndex(ii => ii.InvoiceId);

            modelBuilder.Entity<InvoiceItem>()
                .HasIndex(ii => ii.ProductId);
            //==============================================================================
            // ===============================
            // SHKOLLA - NXENESI RELATION
            // ===============================
            modelBuilder.Entity<Shkolla>()
                .HasMany(s => s.Nxenesit)  //nje shkolle ka shume nxenes    s per shkollen dhe n per nxenesin
                .WithOne(n => n.Shkolla)    // Çdo nxënës ka vetëm NJË shkollë.
                .HasForeignKey(n => n.ID_Shkolla)   //Foreign Key është ID_Shkolla
                .OnDelete(DeleteBehavior.Cascade);  ///Nëse fshihet shkolla, fshihen automatikisht edhe nxënësit e saj.

            modelBuilder.Entity<Fabrika>()
               .HasMany(f => f.Punetoret)  //nje shkolle ka shume nxenes    s per shkollen dhe n per nxenesin
               .WithOne(p => p.Fabrika)    // Çdo nxënës ka vetëm NJË shkollë.
               .HasForeignKey(p => p.ID_Fabrika)   //Foreign Key është ID_Shkolla
               .OnDelete(DeleteBehavior.Cascade);  


            /*
             
             * kodi per 1me1
             modelBuilder.Entity<Personi>()
            .HasOne(p => p.Pasaporta)
            .WithOne(pa => pa.Personi)
            .HasForeignKey<Pasaporta>(pa => pa.PersoniID)
            .OnDelete(DeleteBehavior.Cascade);


            * kodi per NmeN
            modelBuilder.Entity<Studenti>()
            .HasMany(s => s.Lendet)
            .WithMany(l => l.Studentet);



            ============nese per NmeN e shtojme tabelen ndermjetesuese ne menyre manuale behet keshtu:

            Tabela ndërmjetëse manuale
                public class StudentiLenda
                {
                    public int StudentiID { get; set; }
                    public Studenti Studenti { get; set; } = null!;

                    public int LendaID { get; set; }
                    public Lenda Lenda { get; set; } = null!;
                }


            Atëherë Fluent API bëhet:
                modelBuilder.Entity<StudentiLenda>()
                    .HasKey(sl => new { sl.StudentiID, sl.LendaID });
             */
        }
    }
}