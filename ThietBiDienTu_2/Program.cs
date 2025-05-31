using Microsoft.EntityFrameworkCore;
using ThietBiDienTu_2.Areas.Admin.InterfaceRepositories;
using ThietBiDienTu_2.Areas.Admin.Repositories;
using ThietBiDienTu_2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15); // Session timeout duration
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

builder.Services.AddDbContext<ToolDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Tool"));
});

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register repositories
builder.Services.AddScoped<ICoSoAdmin, CoSoAdminRepo>();
builder.Services.AddScoped<IPhieuMuonAdmin, PhieuMuonAdminRepo>();
builder.Services.AddScoped<IPhongAdmin, PhongAdminRepo>();
builder.Services.AddScoped<INhanVien, NhanvienAdminRepo>();
builder.Services.AddScoped<IDongThietBiAdmin, DongThietBiAdminRepo>();
builder.Services.AddScoped<IThietBiAdmin, ThietBiAdminRepo>();
builder.Services.AddScoped<IKhoa, KhoaAdminRepo>();
builder.Services.AddScoped<ISinhvienAdmin, SinhvienAdminRepo>();
builder.Services.AddScoped<IPhieuSuaAdmin, PhieuSuaAdminRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Ensure session middleware is used after routing

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "admin/{controller=Khoa}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
