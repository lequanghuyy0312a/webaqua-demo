using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace web_Aqua.Context
{
    public partial class db_aquaponicsContext : DbContext
    {
        public db_aquaponicsContext()
        {
        }

        public db_aquaponicsContext(DbContextOptions<db_aquaponicsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<ContactAndPolicy> ContactAndPolicies { get; set; } = null!;
        public virtual DbSet<ImportProduct> ImportProducts { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=db_aquaponics;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.Property(e => e.BlogID).HasColumnName("BlogID");

                entity.Property(e => e.CategoryID).HasColumnName("CategoryID");
        

                entity.Property(e => e.CreateOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(300);

                entity.Property(e => e.Thumbnail).HasMaxLength(100);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Blog_Category");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Blog_User");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.Property(e => e.BrandID).HasColumnName("BrandID");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.Nation).HasMaxLength(100);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<ContactAndPolicy>(entity =>
            {
                entity.ToTable("ContactAndPolicy");
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Content).HasColumnName("Content_");
            });

            modelBuilder.Entity<ImportProduct>(entity =>
            {
                entity.ToTable("ImportProduct");

                entity.Property(e => e.ImportProductId).HasColumnName("ImportProductID");

                entity.Property(e => e.Avatar).HasMaxLength(100);

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreateOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.ImportProducts)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ImportProduct_Brand");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ImportProducts)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ImportProduct_Category");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetail_Orderrrr");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetail_Product");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Price).HasMaxLength(100);

                entity.Property(e => e.ShortDescription).HasMaxLength(100);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Product_Brand");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Product_Category");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Avatar).HasMaxLength(100);

                entity.Property(e => e.CreateOnUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(12);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
