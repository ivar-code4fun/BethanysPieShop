using BethanysPieShop.App;
using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BethanysPieShopDbContextConnection") ?? throw new InvalidOperationException("Connection string 'BethanysPieShopDbContextConnection' not found.");

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IShoppingCart, ShoppingCart>(sp => ShoppingCart.GetCart(sp));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
       });
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<BethanysPieShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:BethanysPieShopDbContextConnection"]);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BethanysPieShopDbContext>();
//builder.Services.AddControllers(); // api support


var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapDefaultControllerRoute();
//app.MapControllerRoute(
//name: "default",
//pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseAntiforgery();
app.MapRazorPages();
//app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

DbInitializer.Seed(app);
app.Run();
