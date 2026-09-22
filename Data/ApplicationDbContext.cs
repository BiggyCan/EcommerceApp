using EcommerceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Data
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        // ==========================================
        // PRODUCTOS
        // ==========================================

        public DbSet<Product> Products { get; set; }


        // ==========================================
        // PEDIDOS
        // ==========================================

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }


        // ==========================================
        // SEGUIMIENTO DE CARRITOS
        // ==========================================

        public DbSet<TrackedCart> TrackedCarts { get; set; }

        public DbSet<TrackedCartItem> TrackedCartItems { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ======================================
            // PRODUCTOS
            // ======================================

            modelBuilder.Entity<Product>()
                .Property(
                    product =>
                        product.Price
                )
                .HasPrecision(
                    18,
                    2
                );


            // ======================================
            // PEDIDOS
            // ======================================

            modelBuilder.Entity<Order>()
                .Property(
                    order =>
                        order.TotalAmount
                )
                .HasPrecision(
                    18,
                    2
                );


            modelBuilder.Entity<OrderItem>()
                .Property(
                    item =>
                        item.UnitPrice
                )
                .HasPrecision(
                    18,
                    2
                );


            modelBuilder.Entity<Order>()
                .HasOne(
                    order =>
                        order.User
                )
                .WithMany()
                .HasForeignKey(
                    order =>
                        order.UserId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            modelBuilder.Entity<OrderItem>()
                .HasOne(
                    item =>
                        item.Order
                )
                .WithMany(
                    order =>
                        order.Items
                )
                .HasForeignKey(
                    item =>
                        item.OrderId
                )
                .OnDelete(
                    DeleteBehavior.Cascade
                );


            modelBuilder.Entity<OrderItem>()
                .HasOne(
                    item =>
                        item.Product
                )
                .WithMany()
                .HasForeignKey(
                    item =>
                        item.ProductId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            // ======================================
            // CARRITOS RASTREADOS
            // ======================================

            modelBuilder.Entity<TrackedCart>()
                .Property(
                    cart =>
                        cart.TotalAmount
                )
                .HasPrecision(
                    18,
                    2
                );


            modelBuilder.Entity<TrackedCartItem>()
                .Property(
                    item =>
                        item.UnitPrice
                )
                .HasPrecision(
                    18,
                    2
                );


            // ======================================
            // CARRITO -> USUARIO
            // ======================================

            modelBuilder.Entity<TrackedCart>()
                .HasOne(
                    cart =>
                        cart.User
                )
                .WithMany()
                .HasForeignKey(
                    cart =>
                        cart.UserId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            // ======================================
            // CARRITO -> ITEMS
            // ======================================

            modelBuilder.Entity<TrackedCartItem>()
                .HasOne(
                    item =>
                        item.TrackedCart
                )
                .WithMany(
                    cart =>
                        cart.Items
                )
                .HasForeignKey(
                    item =>
                        item.TrackedCartId
                )
                .OnDelete(
                    DeleteBehavior.Cascade
                );


            // ======================================
            // ITEM -> PRODUCTO
            // ======================================

            modelBuilder.Entity<TrackedCartItem>()
                .HasOne(
                    item =>
                        item.Product
                )
                .WithMany()
                .HasForeignKey(
                    item =>
                        item.ProductId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            // ======================================
            // CARRITO CONVERTIDO -> PEDIDO
            // ======================================

            modelBuilder.Entity<TrackedCart>()
                .HasOne(
                    cart =>
                        cart.ConvertedOrder
                )
                .WithMany()
                .HasForeignKey(
                    cart =>
                        cart.ConvertedOrderId
                )
                .OnDelete(
                    DeleteBehavior.SetNull
                );


            // ======================================
            // ÍNDICES
            // ======================================

            modelBuilder.Entity<TrackedCart>()
                .HasIndex(
                    cart =>
                        cart.UserId
                );


            modelBuilder.Entity<TrackedCart>()
                .HasIndex(
                    cart =>
                        cart.Status
                );


            modelBuilder.Entity<TrackedCart>()
                .HasIndex(
                    cart =>
                        cart.LastActivityAt
                );


            modelBuilder.Entity<TrackedCart>()
                .HasIndex(
                    cart =>
                        new
                        {
                            cart.UserId,
                            cart.Status
                        }
                );
        }
    }
}