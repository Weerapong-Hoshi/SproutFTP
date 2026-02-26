# 🌱 SproutFTP - ระบบจัดการไฟล์ FTP

## 📝 ภาพรวมโครงงาน

**SproutFTP** คือเว็บแอปพลิเคชันสำหรับจัดการไฟล์บน FTP Server สร้างด้วย **ASP.NET Core Razor Pages** ช่วยให้ผู้ใช้สามารถจัดการไฟล์บนเซิร์ฟเวอร์ FTP ผ่านเว็บเบราว์เซอร์ได้อย่างสะดวก

### ความสามารถ
- ✅ **อัปโหลด** ไฟล์จากเครื่องคอมพิวเตอร์ขึ้น FTP Server
- ✅ **ดู** รายการไฟล์ที่เก็บบน FTP Server
- ✅ **ดาวน์โหลด** ไฟล์จาก FTP Server ลงเครื่อง
- ✅ **ลบ** ไฟล์ออกจาก FTP Server

---

## 🏗️ เทคโนโลยีที่ใช้

| ส่วนประกอบ | เทคโนโลยี |
|-----------|-----------|
| **Framework** | ASP.NET Core 10.0 |
| **ภาษาโปรแกรม** | C# |
| **หน้าเว็บ** | Razor Pages |
| **FTP Library** | FluentFTP v53.0.2 |
| **Frontend** | Bootstrap 5, jQuery |
| **IDE** | Visual Studio Code |

---

## 🔄 วิธีการทำงาน

### แผนภาพการทำงาน

```
┌─────────────┐      ┌──────────────────┐      ┌─────────────┐
│   ผู้ใช้     │ ───► │  ASP.NET Core    │ ───► │  FTP Server │
│  Browser    │ ◄─── │  Web App         │ ◄─── │  (Remote)   │
└─────────────┘      └──────────────────┘      └─────────────┘
```

### การทำงานของระบบ:

1. **ดูไฟล์** → โหลดหน้า → เชื่อมต่อ FTP → ดึงรายชื่อไฟล์ → แสดงผลในตาราง
2. **อัปโหลดไฟล์** → เลือกไฟล์ → ส่งไปยังเซิร์ฟเวอร์ → FTP บันทึกไฟล์ → รีเฟรชรายการ
3. **ดาวน์โหลดไฟล์** → คลิกดาวน์โหลด → ดึงจาก FTP → ส่งให้เบราว์เซอร์
4. **ลบไฟล์** → คลิกลบ → ลบออกจาก FTP → รีเฟรชรายการ

---

## 📁 โครงสร้างโปรเจกต์

```
SproutFTP/
├── Program.cs                    # จุดเริ่มต้นของแอปพลิเคชัน
├── appsettings.json             # การตั้งค่า (FTP settings)
├── SproutFTP.csproj            # dependencies ของโปรเจกต์
│
├── Pages/
│   ├── Index.cshtml             # ⭐ หน้าหลัก (ฟอร์มอัปโหลด, ตารางไฟล์)
│   ├── Index.cshtml.cs          # ⭐ ตรรกะ backend (การทำงาน FTP)
│   ├── Error.cshtml             # หน้าแสดงข้อผิดพลาด
│   ├── Privacy.cshtml           # นโยบายความเป็นส่วนตัว
│   └── Shared/
│       └── _Layout.cshtml       # แม่แบบเว็บไซต์ (header, footer)
│
└── wwwroot/
    ├── css/
    │   └── site.css             # สไตล์ที่กำหนดเอง
    └── lib/                     # ไลบรารีของบุคคลที่สาม
        ├── bootstrap/           # Bootstrap CSS
        └── jquery/              # jQuery
```

---

## 💻 คำอธิบายโค้ด

### 1. Program.cs (จุดเริ่มต้น)

**ที่อยู่:** `d:\งาน\อ.วิน\SproutFTP\Program.cs`

**หน้าที่:**
- สร้างและตั้งค่าเว็บแอปพลิเคชัน
- เปิดใช้งาน Razor Pages
- ตั้งค่า routing และไฟล์ static
- เริ่มเว็บเซิร์ฟเวอร์

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var app = builder.Build();
app.MapRazorPages();
app.Run();
```

---

### 2. Index.cshtml.cs (ตรรกะ Backend)

**ที่อยู่:** `d:\งาน\อ.วิน\SproutFTP\Pages\Index.cshtml.cs`

นี่คือ **สมอง** ของแอปพลิเคชัน:

| ฟังก์ชัน | หน้าที่ |
|----------|---------|
| `GetFtpClient()` | สร้างการเชื่อมต่อ FTP โดยใช้การตั้งค่า |
| `LoadFileList()` | ดึงรายชื่อไฟล์จาก FTP server |
| `OnGetAsync()` | จัดการการโหลดหน้า - แสดงรายชื่อไฟล์ |
| `OnPostUploadAsync()` | จัดการการอัปโหลดไฟล์ไปยัง FTP |
| `OnGetDownloadAsync()` | จัดการการดาวน์โหลดไฟล์จาก FTP |
| `OnPostDeleteAsync()` | จัดการการลบไฟล์จาก FTP |

#### การตั้งค่าการเชื่อมต่อ FTP:
```csharp
var config = new FtpConfig
{
    EncryptionMode = FtpEncryptionMode.None,    // ไม่ใช้ SSL
    InternetProtocolVersions = FtpIpVersion.IPv4, // IPv4 เท่านั้น
    ConnectTimeout = 10000,
};
```

---

### 3. Index.cshtml (อินเทอร์เฟซผู้ใช้)

**ที่อยู่:** `d:\งาน\อ.วิน\SproutFTP\Pages\Index.cshtml`

สิ่งที่ผู้ใช้ **เห็น** บนเว็บไซต์:

- **หัวข้อ:** "Sprout FTP Manager"
- **ฟอร์มอัปโหลด:** ปุ่มเลือกไฟล์ + ปุ่ม Upload
- **ตารางไฟล์:** แสดงชื่อไฟล์, ขนาด, วันที่, ปุ่มการทำงาน
- **ปุ่มการทำงาน:** ดาวน์โหลด (สีน้ำเงิน), ลบ (สีแดง)

---

### 4. appsettings.json (การตั้งค่า)

**ที่อยู่:** `d:\งาน\อ.วิน\SproutFTP\appsettings.json`

```json
{
  "FtpSettings": {
    "Host": "127.0.0.1",    // ที่อยู่ FTP server
    "User": "admin",        // ชื่อผู้ใช้
    "Pass": "1234"          // รหัสผ่าน
  }
}
```

---

## 🎨 วิธีแก้ไข UI

### ไฟล์ที่ต้องแก้ไข:

| ต้องการแก้ไข | แก้ไขไฟล์นี้ |
|-------------|-------------|
| เนื้อหาหลัก (ฟอร์มอัปโหลด, ตารางไฟล์) | `Pages/Index.cshtml` |
| หัวข้อ, ส่วนท้าย, เมนูนำทาง | `Pages/Shared/_Layout.cshtml` |
| สี, ฟอนต์, ระยะห่าง | `wwwroot/css/site.css` |
| สไตล์ปุ่ม | ใช้ Bootstrap classes ใน `Index.cshtml` |

### ตัวอย่าง:

#### 🔹 เปลี่ยนหัวข้อ
**ไฟล์:** `Pages/Index.cshtml` (บรรทัด 6)
```csharp
ViewData["Title"] = "Sprout FTP";  // เปลี่ยนข้อความตรงนี้
```

#### 🔹 เปลี่ยนการแสดงหัวข้อ
**ไฟล์:** `Pages/Index.cshtml` (บรรทัด 9)
```html
<h1 class="text-center mb-4">🌱 Sprout FTP Manager</h1>
```

#### 🔹 เปลี่ยนสีปุ่ม
ใน `Index.cshtml` หา Bootstrap classes เหล่านี้:
- `btn-success` = สีเขียว (ปุ่มอัปโหลด)
- `btn-danger` = สีแดง (ปุ่มลบ)
- `btn-info` = สีฟ้า (ปุ่มดาวน์โหลด)

**สีที่มี:** `btn-primary`, `btn-secondary`, `btn-success`, `btn-danger`, `btn-warning`, `btn-info`, `btn-light`, `btn-dark`

#### 🔹 เปลี่ยน FTP Server
**ไฟล์:** `appsettings.json`
```json
"FtpSettings": {
  "Host": "ftp.yourserver.com",  // เปลี่ยน IP หรือ domain
  "User": "yourusername",
  "Pass": "yourpassword"
}
```

---

## 🚀 วิธีรันโปรเจกต์

### ข้อกำหนดเบื้องต้น:
- .NET 10.0 SDK
- FTP server (หรือใช้ local FTP)

### ขั้นตอน:
1. เปิด terminal ในโฟลเดอร์โปรเจกต์
2. รันคำสั่ง: `dotnet run`
3. เปิดเบราว์เซอร์ไปที่ `https://localhost:7000` (หรือ port ที่แสดง)

### เปลี่ยน port:
แก้ไข `Properties/launchSettings.json` - หา `applicationUrl` setting

---

## 📞 คู่มืออ้างอิงด่วน

| งาน | ไฟล์ที่แก้ไข |
|------|-------------|
| เปลี่ยนข้อความหัวข้อ | `Pages/Index.cshtml` บรรทัด 6, 9 |
| เปลี่ยนสี | `Pages/Index.cshtml` (btn classes) |
| เปลี่ยน FTP server | `appsettings.json` |
| เพิ่มฟีเจอร์ใหม่ | `Pages/Index.cshtml.cs` |
| เปลี่ยน layout | `Pages/Shared/_Layout.cshtml` |
| สไตล์ที่กำหนดเอง | `wwwroot/css/site.css` |

---

## 👨‍🎓 ข้อมูลนักศึกษา

**โครงงาน:** SproutFTP - ระบบจัดการไฟล์ FTP  
**นักศึกษา:** นายวีรพงศ์ วสุมงคลพจน์  
**รหัส:** 670112418022  
**สาขา:** สาขาเทคโนโลยีสารสนเทศ หมู่ 1

---

*อัปเดตล่าสุด: กุมภาพันธ์ 2026*
