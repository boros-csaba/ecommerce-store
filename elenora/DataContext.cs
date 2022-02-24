using elenora.BusinessModels;
using elenora.Features.ProductFeeds;
using elenora.Features.ProductPricing;
using elenora.Features.StringBraceletDesigner;
using elenora.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace elenora
{
    public class DataContext : IdentityDbContext<User, Role, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {


        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Customer>()
                .HasOne(c => c.Cart)
                .WithOne(c => c.Customer)
                .HasForeignKey<Cart>(c => c.CustomerId);

            builder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<User>(c => c.CustomerId);

            builder.Entity<CustomerPopupStats>()
                .ToTable("Customers");

            builder.Entity<Customer>()
                .ToTable("Customers")
                .HasOne(c => c.CustomerPopupStats)
                .WithOne()
                .HasForeignKey<CustomerPopupStats>(cp => cp.Id);

            builder.Entity<OrderPopupStats>()
                .ToTable("Orders");

            builder.Entity<Order>()
                .ToTable("Orders")
                .HasOne(c => c.OrderPopupStats)
                .WithOne()
                .HasForeignKey<OrderPopupStats>(cp => cp.Id);
        }

        public DbSet<Bracelet> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<BraceletCartItem> BraceletCartItems { get; set; }
        public DbSet<CustomBraceletCartItem> CustomBraceletCartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<BraceletOrderItem> BraceletOrderItems { get; set; }
        public DbSet<CustomBraceletOrderItem> CustomBraceletOrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ProductComponent> ProductComponents { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentFamily> ComponentFamilies { get; set; }
        public DbSet<Chakra> Chakras { get; set; }
        public DbSet<ComponentFamilyChakra> ComponentFamilyChakras { get; set; }
        public DbSet<Horoscope> Horoscopes { get; set; }
        public DbSet<ComponentFamilyHoroscope> ComponentFamilyHoroscopes { get; set; }
        public DbSet<CustomBraceletComponent> CustomBraceletComponents { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Popup> Popups { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<ComplementaryProduct> ComplementaryProducts { get; set; }
        public DbSet<BeadComplementaryProduct> BeadComplementaryProducts { get; set; }
        public DbSet<CartItemComplementaryProduct> CartItemComplementaryProducts { get; set; }
        public DbSet<OrderItemComplementaryProduct> OrderItemComplementaryProducts { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<BraceletPreviewRequest> BraceletPreviewRequests { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<CustomTextBraceletCartItem> CustomTextBraceletCartItems { get; set; }
        public DbSet<CustomTextBraceletOrderItem> CustomTextOrderItems { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailHistory> EmailHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<ComponentSupplier> ComponentSuppliers { get; set; }
        public DbSet<ComponentImage> ComponentImages { get; set; }
        public DbSet<ProductStatistics> ProductStatistics { get; set; }
        public DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public DbSet<StringBraceletCartItem> StringBraceletCartItems { get; set; }
        public DbSet<StringBraceletOrderItem> StringBraceletOrderItems { get; set; }
        public DbSet<GiftBraceletOrderItem> GiftBraceletOrderItems { get; set; }
        public DbSet<ProductFeedVisibility> ProductFeedVisibilities { get; set; }
    }
}
