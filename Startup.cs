using System;
using AutoMapper;
using Hangfire;
using Hangfire.Common;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ItsyBits.Data;
using ItsyBits.Models;
using ItsyBits.Services;
using ItsyBits.Helpers;
using ItsyBits.ViewModels;
using Newtonsoft.Json;

namespace ItsyBits {
    public class Startup {

        public Startup(IHostingEnvironment env) {

            // Add config file(s)
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment()) {
                builder.AddUserSecrets<Startup>();
            }

            // Add environment variables and build config
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(o => o.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(o => {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
                o.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Use lower case urls in routing
            services.AddRouting(o => o.LowercaseUrls = true);

            // Ignore reference loops
            services.AddMvc().AddJsonOptions(o => { o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });

            // Sessions
            services.AddDistributedMemoryCache();
            services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromSeconds(10);
                o.CookieHttpOnly = true;
            });

            // Add hangfire
            services.AddHangfire(o => o.UseStorage(new MySqlStorage(Configuration.GetConnectionString("Hangfire"))));
            JobHelper.SetSerializerSettings(new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            services.AddTransient<ScheduledTasks>();

            // Add email sender
            services.AddTransient<IEmailSender, AuthMessageSender>();

            // Add config
            services.AddSingleton<IConfiguration>(Configuration);

            // Add automapper
            MapperConfiguration config = new MapperConfiguration(o => {
                // Inputs
                o.CreateMap<StoreAnimalViewModel, Animal>();
                o.CreateMap<StoreBuildingViewModel, Building>();
                o.CreateMap<StoreAnimalUpgradeViewModel, AnimalUpgrade>()
                    .ForMember(d => d.Upgrade, opt => opt.Ignore());
                o.CreateMap<StoreBuildingUpgradeViewModel, BuildingUpgrade>()
                    .ForMember(d => d.Upgrade, opt => opt.Ignore());
                o.CreateMap<Animal, AnimalManageViewModel>()
                    .ForMember(d => d.Refund, opt => opt.MapFrom(a => a.Type.Price / 4))
                    .ForMember(d => d.Description, opt => opt.MapFrom(a => a.Type.Description))
                    .ForMember(d => d.SpritePath, opt => opt.MapFrom(a => a.Type.SpritePath))
                    .ReverseMap();
                o.CreateMap<Building, BuildingManageViewModel>()
                    .ForMember(d => d.Refund, opt => opt.MapFrom(a => a.Type.Price / 4))
                    .ForMember(d => d.Description, opt => opt.MapFrom(a => a.Type.Description))
                    .ForMember(d => d.SpritePath, opt => opt.MapFrom(a => a.Type.SpritePath))
                    .ReverseMap();
                o.CreateMap<ApplicationUser, UserManageViewModel>()
                    .ReverseMap();
                // Outputs
                o.CreateMap<Upgrade, UpgradeViewModel>().MaxDepth(1);
                o.CreateMap<AnimalType, AnimalTypeViewModel>().MaxDepth(1);
                o.CreateMap<Animal, AnimalViewModel>()
                    .ForMember(d => d.Type, opt => opt.MapFrom(a => a.Type.Name))
                    .ForMember(d => d.SpritePath, opt => opt.MapFrom(a => a.Type.SpritePath))
                    .ForMember(d => d.Description, opt => opt.MapFrom(a => a.Type.Description))
                    .ForMember(d => d.Age, opt => opt.MapFrom(a => a.Created.ReadableAge()))
                    .MaxDepth(2);
                o.CreateMap<BuildingType, BuildingTypeViewModel>();
                o.CreateMap<Building, BuildingViewModel>()
                    .ForMember(d => d.Type, opt => opt.MapFrom(b => b.Type.Name))
                    .ForMember(d => d.SpritePath, opt => opt.MapFrom(b => b.Type.SpritePath))
                    .ForMember(d => d.Description, opt => opt.MapFrom(b => b.Type.Description))
                    .MaxDepth(2); ;
                o.CreateMap<Notification, NotificationViewModel>().MaxDepth(2);
                o.CreateMap<ApplicationUser, UserViewModel>().MaxDepth(2);
                o.CreateMap<Plot, PlotViewModel>();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Determine what kind of errors to show
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");

            app.UseStaticFiles();
            app.UseIdentity();

            // Hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new [] { new RoleAuthorizationFilter() } });
            app.UseHangfireServer();
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.AwardUserCoins(), Cron.Daily(0));
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.CheckAnimalHealth(), Cron.Hourly(0));

            // Routes
            app.UseSession();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "area",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
