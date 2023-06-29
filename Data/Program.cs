using Data.Models;
using Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<ProjectDatabaseSettings>(builder.Configuration.GetSection("ProjectDatabase"));

builder.Services.AddSingleton<ContentService>();
builder.Services.AddSingleton<ContentTypeService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<ProductCategoryService>();
builder.Services.AddSingleton<ProductChaptersService>();
builder.Services.AddSingleton<ProductImagesService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<PromotionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
