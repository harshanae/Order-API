using OrderApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application
{
    public class OrderDbContext: DbContext
    {
        public OrderDbContext(DbContextOptions options): base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Order_Product> OrderDetails { get; set; }

        public DbSet<CustomerTotal> CustomerTotal { get; set; }
        public DbSet<EmployeeTotal> EmployeeTotal { get; set; }

        public DbSet<ProductOrderSummary> ProductOrderSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order_Product>().HasKey(sc => new { sc.OrderId, sc.ProductId });
            modelBuilder.Entity<CustomerTotal>().HasNoKey();
            modelBuilder.Entity<EmployeeTotal>().HasNoKey();
            modelBuilder.Entity<ProductOrderSummary>().HasNoKey();
            //modelBuilder.Ignore<CustomerTotal>();
            //modelBuilder.Ignore<EmployeeTotal>();
            //modelBuilder.Ignore<ProductOrderSummary>();
        }
    }
}
