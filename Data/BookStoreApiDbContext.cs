using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookStoreApi.Data
{
    public class BookStoreApiDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }

        public BookStoreApiDbContext(DbContextOptions<BookStoreApiDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(bc => bc.BookId);

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(bc => bc.CategoryId);

            base.OnModelCreating(modelBuilder);

            // تنظیمات مدل Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id); // تعیین کلید اصلی

                entity.Property(b => b.Title)
                    .IsRequired() // عنوان اجباری است
                    .HasMaxLength(100); // حداکثر طول 100 کاراکتر

                entity.Property(b => b.Author)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.Description)
                    .HasMaxLength(500);

                entity.Property(b => b.Price)
                    .HasColumnType("decimal(18,2)"); // نوع داده در دیتابیس
            });

            // تنظیمات مدل Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id); // کلید اصلی

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50); // حداکثر طول 50

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // رابطه یک به چند با Order
                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // حذف آبشاری
            });

            // تنظیمات مدل Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.OrderDate)
                    .IsRequired();

                entity.Property(o => o.TotalPrice)
                    .HasColumnType("decimal(18,2)");

                // رابطه یک به چند با OrderItem
                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // تنظیمات مدل OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.Quantity)
                    .IsRequired();

                entity.Property(oi => oi.Price)
                    .HasColumnType("decimal(18,2)");

                // رابطه چند به یک با Book
                entity.HasOne(oi => oi.Book)
                    .WithMany()
                    .HasForeignKey(oi => oi.BookId)
                    .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف اگر کتابی در سفارش وجود دارد
            });

            #region SeedData
            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Fantasy" },
                new Category() { Id = 2, Name = "Romance" },
                new Category() { Id = 3, Name = "Art" },
                new Category() { Id = 4, Name = "Horror" },
                new Category() { Id = 5, Name = "Thriller" }
                );

            modelBuilder.Entity<Book>().HasData(
                new Book() { Id = 1, Title = "1984", Author = "george orwell", Price = 12000, Description = "Here we have a test description for our book" });

            modelBuilder.Entity<BookCategory>().HasData(new BookCategory() { BookId = 1, CategoryId = 3 }, new BookCategory() { BookId = 1, CategoryId = 5 });
            #endregion
        }
    }
}
