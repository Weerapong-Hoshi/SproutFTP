var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(); // เปิดใช้งาน Razor Pages

var app = builder.Build();

app.UseStaticFiles(); // บอกให้ระบบวิ่งไปหาไฟล์ในโฟลเดอร์ wwwroot เช่น css, js, images เป็นต้น
app.UseRouting(); // บอกให้ระบบวิ่งไปหาไฟล์ในโฟลเดอร์ Pages และ Controllers เพื่อทำการ Routing ไปยังหน้าเว็บที่ต้องการ

app.MapRazorPages(); // บอกให้ระบบวิ่งไปหาไฟล์ในโฟลเดอร์ Pages

app.Run();