using System;
using System.Net;
using BarionClientLibrary;
using elenora.Features.Analytics;
using elenora.Features.Cart;
using elenora.Features.Inventory;
using elenora.Features.ProductFeeds;
using elenora.Features.ProductList;
using elenora.Features.ProductPricing;
using elenora.Features.QualityAuditing;
using elenora.Features.RelatedProducts;
using elenora.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Wkhtmltopdf.NetCore;

namespace elenora
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if E2E
            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase("TestDb")
            );
#else
            //services.AddDbContext<DataContext>(options =>
            //    options.UseNpgsql(Configuration.GetConnectionString("Website"))
            //);
#endif
            // todo
            //services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<ICartService, CartService>();
            //services.AddScoped<IWishlistService, WishlistService>();
            //services.AddScoped<IOrderService, OrderService>();
            //services.AddScoped<IActionLogService, ActionLogService>();
            //services.AddScoped<ICustomerService, CustomerService>();
            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IPromotionService, PromotionService>();
            //services.AddScoped<IArticleService, ArticleService>();
            //services.AddScoped<IPdfService, PdfService>();
            //services.AddScoped<IFaqService, FaqService>();
            //services.AddScoped<IQuizService, QuizService>();
            //services.AddScoped<ITestimonialService, TestimonialService>();
            //services.AddScoped<IProductImageService, ProductImageService>();
            //services.AddScoped<ISearchService, SearchService>();
            //services.AddScoped<ICartRepository, CartRepository>();
            //services.AddScoped<IRelatedProductsService, RelatedProductsService>();
            //services.AddScoped<IProductFeedService, ProductFeedService>();
            //services.AddScoped<IInventoryService, InventoryService>();
            //services.AddScoped<IProductPricingService, ProductPricingService>();
            //services.AddScoped<IProductListService, ProductListService>();
            //services.AddScoped<IQualityAuditingService, QualityAuditingService>();
            //services.AddScoped<IAnalyticsService, AnalyticsService>();
            //services.AddWkhtmltopdf();

            var barionSettings = new BarionSettings
            {
                //BaseUrl = new Uri(Configuration.GetValue<string>("Settings:BarionUrl")),
                //POSKey = Guid.Parse(Configuration.GetValue<string>("Settings:BarionKey")), 
                Payee = "info@elenora.hu",
            };
            services.AddSingleton(barionSettings);
            services.AddTransient<BarionClient>();
            services.AddHttpClient<BarionClient>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(cookieOptions => {
                cookieOptions.LoginPath = "/admin/login";
            });
            services.AddControllersWithViews();
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
            services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().Build()
                ));
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.RequireHeaderSymmetry = false;
                options.ForwardLimit = null;
                options.KnownProxies.Add(IPAddress.Parse("192.168.0.2"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false,
                ForwardLimit = null,
                KnownProxies = { IPAddress.Parse("192.168.0.2") }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/hiba");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + 31536000;
                }
            });
            app.UseCors();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

#if E2E
            using var serviceScope = app.ApplicationServices.CreateScope();
            TestData.GenerateTestData(serviceScope.ServiceProvider.GetService<DataContext>());
#endif
        }
    }
}
