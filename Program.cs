var builder = WebApplication.CreateBuilder(args);

// เปิดใช้งาน Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map Route เข้ากับหน้าเว็บ
app.MapRazorPages();

app.Run();