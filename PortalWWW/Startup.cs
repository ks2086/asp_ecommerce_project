using Data.Services;

namespace PortalWWW
{
    public class Startup
    {
        public IConfiguration configRoot
        {
            get;
        }
        public Startup(IConfiguration configuration)
        {
            configRoot = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ContentService>();
            services.AddScoped<ContentTypeService>();
            services.AddScoped<ProductCategoryService>();
            services.AddScoped<ProductService>();
            services.AddScoped<ProductChaptersService>();
            services.AddScoped<ProductImagesService>();
            services.AddScoped<OrderService>();
            services.AddScoped<UserService>();
            services.AddScoped<PromotionService>();

            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache(); // Dodaje pamięć podręczną dla sesji
            services.AddSession(options =>
            {
                options.Cookie.Name = "CartSession"; // Nazwa pliku cookie sesji
                options.Cookie.MaxAge = TimeSpan.FromMinutes(60); // Czas wygaśnięcia sesji
                options.IdleTimeout = TimeSpan.FromMinutes(60); // Czas bezczynności, po którym sesja wygaśnie
                options.Cookie.HttpOnly = true; // Ustawienia zabezpieczeń cookie
                options.Cookie.IsEssential = true; // Wymaga wyraźnej zgody użytkownika na przechowywanie ciasteczka
            });
            services.AddControllers();
        }
    }
}
