using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Implementations;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.BussinessValidationServices;
using QuanLyPhongKham.Services.Implementations;
using QuanLyPhongKham.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(conn))
{
    throw new Exception("Connection string is NULL");
}

Console.WriteLine("CONN = " + conn);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("QuanLyPhongKham.Data")
    ));
//Console.WriteLine("CONNECTION = " + conn);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Dòng này giúp bỏ qua các vòng lặp dữ liệu, ae pull về dùng API sẽ không bị lỗi
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// =======================================================
// BƯỚC 1: THÊM DỊCH VỤ SESSION VÀO BUILDER 
// =======================================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Giữ đăng nhập trong 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IBacSiRepository, BacSiRepository>();
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<ILeTanRepository, LeTanRepository>();
builder.Services.AddScoped<IBuoiKhamRepository, BuoiKhamRepository>();
builder.Services.AddScoped<IBuoiKhamRepository, BuoiKhamRepository>();
builder.Services.AddScoped<IBacSiRepository, BacSiRepository>();
builder.Services.AddScoped<IPhongKhamRepository, PhongKhamRepository>();
builder.Services.AddScoped<IChuyenKhoaRepository, ChuyenKhoaRepository>();
builder.Services.AddScoped<IBenhNhanRepository, BenhNhanRepository>();
builder.Services.AddScoped<IBacSiService, BacSiService>();
builder.Services.AddScoped<IBenhNhanService, BenhNhanService>();
builder.Services.AddScoped<ILeTanService, LeTanService>();
builder.Services.AddScoped<IBuoiKhamService, BuoiKhamService>();

builder.Services.AddScoped<TaiKhoanService>();
builder.Services.AddScoped<AccountValidationService>();
//builder.Services.AddScoped<BacSiService>();



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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Lệnh này sẽ tự động kiểm tra thư mục Migrations và chạy cập nhật vào cơ sở dữ liệu
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Có lỗi khi auto-migrate: {ex.Message}");
    }
}
app.Run();