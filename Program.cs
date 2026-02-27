var builder = WebApplication.CreateBuilder(args);

// เปิดใช้งาน Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles(); // เปิดใช้งานการให้บริการไฟล์สถิต (เช่น CSS, JS) จาก wwwroot
app.UseRouting(); // เปิดใช้งานการกำหนดเส้นทาง (Routing) เพื่อให้สามารถเข้าถึงหน้าเว็บได้

// Map Route เข้ากับหน้าเว็บ
app.MapRazorPages();

app.Run();