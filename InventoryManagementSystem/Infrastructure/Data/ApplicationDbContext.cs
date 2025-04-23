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
                      .HasConstraintName("FK_User_UserType");
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
                      .HasConstraintName("FK_Item_Category");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("SupplierId");

                entity.HasOne(s => s.User)
                      .WithOne(u => u.Supplier)
                      .HasForeignKey<Supplier>(s => s.UserId)
                      .HasConstraintName("FK_Supplier_User");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("CustomerId");

                entity.HasOne(c => c.User)
                      .WithOne(u => u.Customer)
                      .HasForeignKey<Customer>(c => c.UserId)
                      .HasConstraintName("FK_Customer_User");
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("PurchaseOrderId");

                entity.HasOne(po => po.Supplier)
                      .WithMany(s => s.PurchaseOrders)
                      .HasForeignKey(po => po.SupplierId)
                      .HasConstraintName("FK_PurchaseOrder_Supplier");
            });

            modelBuilder.Entity<SupplierItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("SupplierItemId");

                entity.HasOne(si => si.PurchaseOrder)
                      .WithMany(po => po.SupplierItems)
                      .HasForeignKey(si => si.PurchaseOrderId)
                      .HasConstraintName("FK_SupplierItem_PurchaseOrder");

                entity.HasOne(si => si.Item)
                      .WithOne(i => i.SupplierItem)
                      .HasForeignKey<SupplierItem>(si => si.ItemId)
                      .HasConstraintName("FK_SupplierItem_Item");

                entity.HasOne(si => si.Supplier)
                      .WithMany(s => s.SupplierItems)
                      .HasForeignKey(si => si.SupplierId)
                      .HasConstraintName("FK_SupplierItem_Supplier");
            });

            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("SalesOrderId");

                entity.HasOne(so => so.Customer)
                      .WithMany(c => c.SalesOrders)
                      .HasForeignKey(so => so.CustomerId)
                      .HasConstraintName("FK_SalesOrder_Customer");
            });

            modelBuilder.Entity<CustomerItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("CustomerItemId");

                entity.HasOne(ci => ci.SalesOrder)
                      .WithMany(so => so.CustomerItems)
                      .HasForeignKey(ci => ci.SalesOrderId)
                      .HasConstraintName("FK_CustomerItem_SalesOrder");

                entity.HasOne(ci => ci.Item)
                      .WithOne(i => i.CustomerItem)
                      .HasForeignKey<CustomerItem>(ci => ci.ItemId)
                      .HasConstraintName("FK_CustomerItem_Item");

                entity.HasOne(ci => ci.Customer)
                      .WithMany(c => c.CustomerItems)
                      .HasForeignKey(ci => ci.CustomerId)
                      .HasConstraintName("FK_CustomerItem_Customer");
            });
        }
    }
}
