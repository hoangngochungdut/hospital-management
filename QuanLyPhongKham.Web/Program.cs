using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services;
using QuanLyPhongKham.Services.BussinessValidationServices;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddControllersWithViews();

// =======================================================
// BƯỚC 1: THÊM DỊCH VỤ SESSION VÀO BUILDER 
// =======================================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Giữ đăng nhập trong 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<TaiKhoanRepository>();
builder.Services.AddScoped<AccountValidationService>();
builder.Services.AddScoped<TaiKhoanService>();
builder.Services.AddScoped<IBuoiKhamRepository, BuoiKhamRepository>();
builder.Services.AddScoped<IBuoiKhamService, BuoiKhamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// =======================================================
// BƯỚC 2: KHÍCH HOẠT SESSION (Vị trí bắt buộc phải ở đây)
// =======================================================
app.UseSession();

app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();