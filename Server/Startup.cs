using Server.Services;
using System.Text.Json.Serialization;

namespace Server
{
    public class Startup
    {
        IConfigurationRoot configurationRoot;

        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment configuration)
        {
            configurationRoot = new ConfigurationBuilder().SetBasePath(configuration.ContentRootPath).AddJsonFile("appsettings.json").Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            //Использование сессии
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //Использование кук
            services.ConfigureApplicationCookie(configure => configure.Cookie.Expiration = TimeSpan.FromDays(14));
            
            //Подключение к бд
            //DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            //services.AddDbContext<AutoDataContext>(options => options.UseLazyLoadingProxies().UseMySql(configurationRoot.GetConnectionString("DefaultConnection"),
                //ServerVersion.AutoDetect(configurationRoot.GetConnectionString("DefaultConnection"))));
            
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            //Добавление manager'ов
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
