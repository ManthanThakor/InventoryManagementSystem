using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<SupplierItem> SupplierItems { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<CustomerItem> CustomerItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("UserTypeId");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("UserId");

                entity.HasOne(u => u.UserType)
                      .WithMany(ut => ut.Users)
                      .HasForeignKey(u => u.UserTypeId)
                      .HasConstraintName("FK_User_UserTypeID");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("CategoryId");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ItemId");

                entity.HasOne(i => i.Category)
                      .WithMany(c => c.Items)
                      .HasForeignKey(i => i.CategoryId)
                      .HasConstraintName("FK_Item_CategoryID");

                entity.Property(i => i.GSTPercent).HasPrecision(18, 2);
                entity.Property(i => i.PurchasePrice).HasPrecision(18, 2);
                entity.Property(i => i.SellingPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("SupplierId");

                entity.HasOne(s => s.User)
                      .WithOne(u => u.Supplier)
                      .HasForeignKey<Supplier>(s => s.UserId)
                      .HasConstraintName("FK_Supplier_UserID");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("CustomerId");

                entity.HasOne(c => c.User)
                      .WithOne(u => u.Customer)
                      .HasForeignKey<Customer>(c => c.UserId)
                      .HasConstraintName("FK_Customer_UserID");
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("PurchaseOrderId");

                entity.HasOne(po => po.Supplier)
                      .WithMany(s => s.PurchaseOrders)
                      .HasForeignKey(po => po.SupplierId)
                      .HasConstraintName("FK_PurchaseOrder_SupplierID");

                entity.Property(po => po.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<SupplierItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("SupplierItemId");

                entity.HasOne(si => si.PurchaseOrder)
                      .WithMany(po => po.SupplierItems)
                      .HasForeignKey(si => si.PurchaseOrderId)
                      .HasConstraintName("FK_SupplierItem_PurchaseOrderID");

                entity.HasOne(si => si.Item)
                      .WithOne(i => i.SupplierItem)
                      .HasForeignKey<SupplierItem>(si => si.ItemId)
                      .HasConstraintName("FK_SupplierItem_ItemID");

                entity.HasOne(si => si.Supplier)
                      .WithMany(s => s.SupplierItems)
                      .HasForeignKey(si => si.SupplierId)
                      .HasConstraintName("FK_SupplierItem_SupplierID");

                entity.Property(si => si.GSTAmount).HasPrecision(18, 2);
                entity.Property(si => si.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("SalesOrderId");

                entity.HasOne(so => so.Customer)
                      .WithMany(c => c.SalesOrders)
                      .HasForeignKey(so => so.CustomerId)
                      .HasConstraintName("FK_SalesOrder_CustomerID");

                entity.Property(so => so.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<CustomerItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("CustomerItemId");

                entity.HasOne(ci => ci.SalesOrder)
                      .WithMany(so => so.CustomerItems)
                      .HasForeignKey(ci => ci.SalesOrderId)
                      .HasConstraintName("FK_CustomerItem_SalesOrderID")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ci => ci.Item)
                      .WithOne(i => i.CustomerItem)
                      .HasForeignKey<CustomerItem>(ci => ci.ItemId)
                      .HasConstraintName("FK_CustomerItem_ItemID")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ci => ci.Customer)
                      .WithMany(c => c.CustomerItems)
                      .HasForeignKey(ci => ci.CustomerId)
                      .HasConstraintName("FK_CustomerItem_CustomerID")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ci => ci.GSTAmount).HasPrecision(18, 2);
                entity.Property(ci => ci.TotalAmount).HasPrecision(18, 2);
            });
        }
    }
}
