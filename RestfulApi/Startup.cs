using Data.Services;

namespace RestfulApi
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
        }
    }
}
