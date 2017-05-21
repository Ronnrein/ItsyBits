using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace ItsyBits
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

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

            // Use lower case urls
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // Sessions
            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.CookieHttpOnly = true;
            });

            // Hangfire
            services.AddHangfire(o => o.UseStorage(new MySqlStorage(Configuration.GetConnectionString("Hangfire"))));
            JobHelper.SetSerializerSettings(new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            services.AddTransient<ScheduledTasks>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add config
            services.AddSingleton<IConfiguration>(Configuration);

            // Add automapper
            MapperConfiguration config = new MapperConfiguration(o => {
                // Inputs
                o.CreateMap<StoreAnimalViewModel, Animal>();
                o.CreateMap<StoreBuildingViewModel, Building>();
                o.CreateMap<StoreAnimalUpgradeViewModel, AnimalUpgrade>()
                    .ForMember(s => s.Upgrade, opts => opts.Ignore());
                o.CreateMap<StoreBuildingUpgradeViewModel, BuildingUpgrade>()
                    .ForMember(s => s.Upgrade, opts => opts.Ignore());
                // Outputs
                o.CreateMap<Upgrade, UpgradeViewModel>().MaxDepth(1);
                o.CreateMap<AnimalType, AnimalTypeViewModel>().MaxDepth(1);
                o.CreateMap<Animal, AnimalViewModel>()
                    .ForMember(d => d.Upgrades, opts => opts.MapFrom(a => a.Upgrades))
                    .ForMember(d => d.Building, opts => opts.MapFrom(a => a.Building))
                    .ForMember(d => d.Type, opts => opts.MapFrom(a => a.Type.Name))
                    .ForMember(d => d.SpritePath, opts => opts.MapFrom(a => a.Type.SpritePath))
                    .ForMember(d => d.Description, opts => opts.MapFrom(a => a.Type.Description))
                    .ForMember(d => d.StatusText, opts => opts.MapFrom(a => a.GetStatusText()))
                    .MaxDepth(2);
                o.CreateMap<BuildingType, BuildingTypeViewModel>();
                o.CreateMap<Building, BuildingViewModel>()
                    .ForMember(d => d.Upgrades, opts => opts.MapFrom(b => b.Upgrades))
                    .ForMember(d => d.Type, opts => opts.MapFrom(b => b.Type.Name))
                    .ForMember(d => d.SpritePath, opts => opts.MapFrom(b => b.Type.SpritePath))
                    .ForMember(d => d.Description, opts => opts.MapFrom(b => b.Type.Description))
                    .ForMember(d => d.StatusText, opts => opts.MapFrom(b => b.GetStatusText()))
                    .MaxDepth(2); ;
                o.CreateMap<Notification, NotificationViewModel>().MaxDepth(2);
                o.CreateMap<ApplicationUser, UserViewModel>().MaxDepth(2);
                o.CreateMap<Plot, PlotViewModel>().MaxDepth(1);
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new [] { new RoleAuthorizationFilter() } });
            app.UseHangfireServer();
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.AwardUserCoins(), Cron.Daily(0));
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.CheckAnimalHealth(), Cron.Hourly(0));
            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

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
