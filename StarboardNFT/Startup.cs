using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarboardNFT.Areas.Identity;
using StarboardNFT.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Radzen;
using StarboardNFT.Models;
using Nethereum.Metamask;
using Nethereum.Metamask.Blazor;
using Nethereum.UI;
using FluentValidation;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using StarboardNFT.Services;
using Blazored.LocalStorage;
using StarboardNFT.Hubs;
using StarboardNFT.Interface;

namespace StarboardNFT
{
    public class Startup
    {

        public static Dictionary<string, decimal> CoinPriceDict = new Dictionary<string, decimal>();
        public static Dictionary<string, decimal> Coin1hChangeDict = new Dictionary<string, decimal>();
        public static Dictionary<string, decimal> Coin24hChangeDict = new Dictionary<string, decimal>();
        public static Dictionary<string, decimal> Coin7dChangeDict = new Dictionary<string, decimal>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredLocalStorage();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddSingleton<WeatherForecastService>();
            services.AddTransient<ApplicationDbContext>(); //register DbContext as Transient

            services.AddSignalR();
            services.AddTransient<ProfileService>();
            services.AddTransient<NFTService>();
            services.AddTransient<NotificationsService>();
            services.AddTransient<AuctionService>();
            services.AddTransient<ActivityService>();
            //services.AddScoped<UserConnectionManager>();
            services.AddScoped<IUserConnectionManager, UserConnectionManager>();

            //// Nethereum Services

            services.AddScoped<IMetamaskInterop, MetamaskBlazorInterop>();
            services.AddScoped<MetamaskInterceptor>();
            services.AddScoped<MetamaskHostProvider>();
            services.AddScoped<IEthereumHostProvider>(serviceProvider =>
            {
                return serviceProvider.GetService<MetamaskHostProvider>();
            });
            services.AddScoped<IEthereumHostProvider, MetamaskHostProvider>();
            services.AddScoped<NethereumAuthenticator>();
            // services.AddValidatorsFromAssemblyContaining<Nethereum.Erc20.Blazor.Erc20Transfer>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;

                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            //Radzen Services
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<Radzen.NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            services.AddBlazoredSessionStorage();
            services.AddScoped<ClipboardService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Perform DB Migration
            Initialize(app.ApplicationServices);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapHub(NotificationHub);
            //});
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                // auto migration
                context.Database.Migrate();

                // Seed the database.
                InitializeSeedData(context);

                CoinPriceDict.Add("ETH", 0.00M);
                Coin1hChangeDict.Add("ETH", 0.00M);
                Coin24hChangeDict.Add("ETH", 0.00M);
                Coin7dChangeDict.Add("ETH", 0.00M);
            }
        }

        private static void InitializeSeedData(ApplicationDbContext context)
        {

        }
    }
}
