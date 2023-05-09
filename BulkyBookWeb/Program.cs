using BulkyBookWeb.Data;
using Microsoft.EntityFrameworkCore;
using Typesense;
using Typesense.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

////Typesense Start
//var provider = new ServiceCollection()
//    .AddTypesenseClient(config =>
//    {
//        config.ApiKey = "xyz";
//        config.Nodes = new List<Node>
//        {
//            new Node("localhost", "8108", "http")
//        };
//    }).BuildServiceProvider();
//var typesenseClient = provider.GetService<ITypesenseClient>();
////Typesense End

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
