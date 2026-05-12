using System;
using EcomApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // store UserRole as string in database - readable and safe
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();

        // product -> category relationship
        modelBuilder
            .Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order -> User relationship
        modelBuilder
            .Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem -> Order relationship
        modelBuilder
            .Entity<OrderItem>()
            .HasOne(io => io.Order)
            .WithMany(u => u.OrderItems)
            .HasForeignKey(io => io.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem and Product relationship
        modelBuilder
            .Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Category>()
            .HasData(
                new Category
                {
                    Id = 1,
                    Name = "Electronics",
                    Description = "Electronic devices",
                },
                new Category
                {
                    Id = 2,
                    Name = "Furniture",
                    Description = "Home and office furniture",
                },
                new Category
                {
                    Id = 3,
                    Name = "Stationery",
                    Description = "Office supplies",
                },
                new Category
                {
                    Id = 4,
                    Name = "Clothing",
                    Description = "Apparel and accessories",
                }
            );

        modelBuilder
            .Entity<Product>()
            .HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "High performance laptop",
                    Price = 75000,
                    Stock = 10,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 2,
                    Name = "Phone",
                    Description = "Latest smartphone",
                    Price = 25000,
                    Stock = 25,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 3,
                    Name = "Headphones",
                    Description = "Noise cancelling",
                    Price = 3000,
                    Stock = 50,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 4,
                    Name = "Desk",
                    Description = "Wooden office desk",
                    Price = 12000,
                    Stock = 8,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 5,
                    Name = "Chair",
                    Description = "Ergonomic office chair",
                    Price = 8000,
                    Stock = 15,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 6,
                    Name = "Notebook",
                    Description = "A5 ruled notebook",
                    Price = 200,
                    Stock = 100,
                    CategoryId = 3,
                },
                new Product
                {
                    Id = 7,
                    Name = "Pen",
                    Description = "Ball point pen",
                    Price = 50,
                    Stock = 500,
                    CategoryId = 3,
                }
            );
    }
}
