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

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<ILeTanRepository, LeTanRepository>();
builder.Services.AddScoped<IBuoiKhamRepository, BuoiKhamRepository>();
builder.Services.AddScoped<IBacSiRepository, BacSiRepository>();
builder.Services.AddScoped<IPhongKhamRepository, PhongKhamRepository>();
builder.Services.AddScoped<IChuyenKhoaRepository, ChuyenKhoaRepository>();
builder.Services.AddScoped<IBenhNhanRepository, BenhNhanRepository>();
builder.Services.AddScoped<ILichTrucRepository, LichTrucRepository>();
builder.Services.AddScoped<IPhongKhamRepository, PhongKhamRepository>();
builder.Services.AddScoped<IKetQuaKhamRepository, KetQuaKhamRepository>();
builder.Services.AddScoped<ITieuSuBenhAnRepository, TieuSuBenhAnRepository>();

builder.Services.AddScoped<IPhongKhamService, PhongKhamService>();
builder.Services.AddScoped<ILichTrucService, LichTrucService>();
builder.Services.AddScoped<IKetQuaKhamRepository, KetQuaKhamRepository>();
builder.Services.AddScoped<IBacSiService, BacSiService>();
builder.Services.AddScoped<IBenhNhanService, BenhNhanService>();
builder.Services.AddScoped<ILeTanService, LeTanService>();
builder.Services.AddScoped<IBuoiKhamService, BuoiKhamService>();
builder.Services.AddScoped<IChuyenKhoaService, ChuyenKhoaService>();
builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<AccountValidationService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

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
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Có lỗi khi auto-migrate: {ex.Message}");
    }
}
app.Run();