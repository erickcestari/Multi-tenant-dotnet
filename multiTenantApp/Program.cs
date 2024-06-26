using Microsoft.EntityFrameworkCore;
using multiTenantApp.Extensions;
using multiTenantApp.Middleware;
using multiTenantApp.Models;
using multiTenantApp.Services;
using multiTenantApp.Services.ProductService;
using multiTenantApp.Services.TenantService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Current tenant service with scoped lifetime (created per each request)
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();

// adding a database service with configuration -- connection string read from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<TenantDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAndMigrateTenantDatabases(builder.Configuration);

// Product CRUD service with transient lifetime
builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IProductService, ProductService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<TenantResolver>();
app.MapControllers();

app.Run();
