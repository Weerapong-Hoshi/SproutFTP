# 🌱 SproutFTP - FTP File Manager

## 📝 Project Overview

**SproutFTP** is a web-based FTP (File Transfer Protocol) file manager built with **ASP.NET Core Razor Pages**. It allows users to manage files on a remote FTP server through a user-friendly web interface.

### Features
- ✅ **Upload** files from your computer to FTP server
- ✅ **View** list of files stored on FTP server
- ✅ **Download** files from FTP server to your computer
- ✅ **Delete** files from FTP server

---

## 🏗️ Technology Stack

| Component | Technology |
|-----------|------------|
| **Framework** | ASP.NET Core 10.0 |
| **Programming Language** | C# |
| **Web Pages** | Razor Pages |
| **FTP Library** | FluentFTP v53.0.2 |
| **Frontend** | Bootstrap 5, jQuery |
| **IDE** | Visual Studio Code |

---

## 🔄 How It Works

### Flow Diagram

```
┌─────────────┐      ┌──────────────────┐      ┌─────────────┐
│   User      │ ───► │  ASP.NET Core    │ ───► │  FTP Server │
│  Browser    │ ◄─── │  Web App         │ ◄─── │  (Remote)   │
└─────────────┘      └──────────────────┘      └─────────────┘
```

### User Actions:

1. **View Files** → Page loads → Connect to FTP → Get file list → Display table
2. **Upload File** → Select file → Send to server → FTP saves file → Refresh list
3. **Download File** → Click download → Fetch from FTP → Send to browser
4. **Delete File** → Click delete → Remove from FTP → Refresh list

---

## 📁 Project Structure

```
SproutFTP/
├── Program.cs                    # Application entry point
├── appsettings.json             # Configuration (FTP settings)
├── SproutFTP.csproj            # Project dependencies
│
├── Pages/
│   ├── Index.cshtml             # ⭐ Main UI (upload form, file table)
│   ├── Index.cshtml.cs          # ⭐ Backend logic (FTP operations)
│   ├── Error.cshtml             # Error page
│   ├── Privacy.cshtml           # Privacy policy page
│   └── Shared/
│       └── _Layout.cshtml       # Site template (header, footer)
│
└── wwwroot/
    ├── css/
    │   └── site.css             # Custom styles
    └── lib/                     # Third-party libraries
        ├── bootstrap/           # Bootstrap CSS
        └── jquery/              # jQuery
```

---

## 💻 Code Explanation

### 1. Program.cs (Entry Point)

**Location:** `d:\งาน\อ.วิน\SproutFTP\Program.cs`

**What it does:**
- Creates and configures the web application
- Enables Razor Pages
- Sets up routing and static files
- Launches the web server

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var app = builder.Build();
app.MapRazorPages();
app.Run();
```

---

### 2. Index.cshtml.cs (Backend Logic)

**Location:** `d:\งาน\อ.วิน\SproutFTP\Pages\Index.cshtml.cs`

This is the **brain** of the application:

| Method | Function |
|--------|----------|
| `GetFtpClient()` | Creates FTP connection using config settings |
| `LoadFileList()` | Gets list of files from FTP server |
| `OnGetAsync()` | Handles page load - displays file list |
| `OnPostUploadAsync()` | Handles file upload to FTP server |
| `OnGetDownloadAsync()` | Handles file download from FTP server |
| `OnPostDeleteAsync()` | Handles file deletion from FTP server |

#### FTP Connection Configuration:
```csharp
var config = new FtpConfig
{
    EncryptionMode = FtpEncryptionMode.None,    // No SSL
    InternetProtocolVersions = FtpIpVersion.IPv4, // IPv4 only
    ConnectTimeout = 10000,
};
```

---

### 3. Index.cshtml (User Interface)

**Location:** `d:\งาน\อ.วิน\SproutFTP\Pages\Index.cshtml`

This is what users **see** on the website:

- **Header:** "Sprout FTP Manager" title
- **Upload Form:** File picker + Upload button
- **File Table:** Shows filename, size, date, action buttons
- **Action Buttons:** Download (blue), Delete (red)

---

### 4. appsettings.json (Configuration)

**Location:** `d:\งาน\อ.วิน\SproutFTP\appsettings.json`

```json
{
  "FtpSettings": {
    "Host": "127.0.0.1",    // FTP server address
    "User": "admin",        // Username
    "Pass": "1234"          // Password
  }
}
```

---

## 🎨 How to Edit the UI

### Where to Make Changes:

| What to Change | Edit This File |
|----------------|----------------|
| Main content (upload form, file table) | `Pages/Index.cshtml` |
| Header, footer, navigation bar | `Pages/Shared/_Layout.cshtml` |
| Colors, fonts, spacing | `wwwroot/css/site.css` |
| Button styles | Use Bootstrap classes in `Index.cshtml` |

---

### Examples:

#### 🔹 Change the Title
**File:** `Pages/Index.cshtml` (line 6)
```csharp
ViewData["Title"] = "Sprout FTP";  // Change this text
```

#### 🔹 Change Title Display
**File:** `Pages/Index.cshtml` (line 9)
```html
<h1 class="text-center mb-4">🌱 Sprout FTP Manager</h1>
```

#### 🔹 Change Button Colors
In `Index.cshtml`, find these Bootstrap classes:
- `btn-success` = Green (Upload button)
- `btn-danger` = Red (Delete button)
- `btn-info` = Blue (Download button)

**Available colors:** `btn-primary`, `btn-secondary`, `btn-success`, `btn-danger`, `btn-warning`, `btn-info`, `btn-light`, `btn-dark`

#### 🔹 Change FTP Server
**File:** `appsettings.json`
```json
"FtpSettings": {
  "Host": "ftp.yourserver.com",  // Change IP or domain
  "User": "yourusername",
  "Pass": "yourpassword"
}
```

---

## 🎓 Professor Interview Q&A

### BASICS & CONCEPT

**Q: What is SproutFTP and what does it do?**
> **A:** SproutFTP is a web-based FTP file manager built with ASP.NET Core. It allows users to upload, view, download, and delete files on a remote FTP server through a web browser interface.

**Q: What is FTP and why use it?**
> **A:** FTP (File Transfer Protocol) is a standard network protocol used to transfer files between a client and a server. We use FluentFTP library because it provides a simple, async/await API for .NET applications.

**Q: What technology stack did you use?**
> **A:** ASP.NET Core 10.0 (Razor Pages), FluentFTP v53.0.2, Bootstrap 5, jQuery, C#

---

### HOW THE CODE WORKS

**Q: Explain how file upload works in your application.**
> **A:** When user selects a file and clicks Upload:
> 1. Form sends file to `OnPostUploadAsync()` in Index.cshtml.cs
> 2. Creates FTP connection using `GetFtpClient()`
> 3. Opens file as stream with `UploadedFile.OpenReadStream()`
> 4. Uploads stream to FTP server using `client.UploadStream()`
> 5. On success, page redirects to refresh and show new file

**Q: How do you display the list of files from the FTP server?**
> **A:** When page loads, `OnGetAsync()` calls `LoadFileList()` which connects to FTP server and uses `client.GetListing()` to get all files. Results are stored in `FileList` property and displayed in Index.cshtml using a foreach loop.

**Q: What is FluentFTP and why did you choose it?**
> **A:** FluentFTP is a popular .NET library for FTP operations. It provides async/await support, a clean API, and is well-maintained.

---

### CONFIGURATION

**Q: Where do you store FTP server credentials?**
> **A:** In `appsettings.json` under "FtpSettings" section. This is injected via ASP.NET Core's dependency injection.

**Q: Why did you set EncryptionMode to None and use IPv4?**
> **A:** These settings fix connection timeout issues:
> - `EncryptionMode = None` - Disables SSL/TLS for simpler local testing
> - `IPv4` - Forces IPv4 to avoid compatibility issues

---

### CODING & MODIFICATION

**Q: How would you add a new feature (e.g., create folder)?**
> **A:** Add a new method in `Index.cshtml.cs`:
> ```csharp
> public async Task<IActionResult> OnPostCreateFolderAsync(string folderName)
> {
>     using var client = GetFtpClient();
>     await client.Connect();
>     await client.CreateDirectory(folderName);
>     return RedirectToPage();
> }
> ```

**Q: How would you change the FTP server address?**
> **A:** Edit `appsettings.json` - change the "Host" value to your server address.

---

### ERROR HANDLING

**Q: What happens if the FTP server is down?**
> **A:** The code has try-catch blocks. If connection fails, it catches the exception, adds error message to ModelState, and displays error to user instead of crashing.

---

### CHALLENGES & SOLUTIONS

**Q: What challenges did you face during development?**
> **A:** Two main challenges:
> 1. **Connection Timeout** - Solved by setting `EncryptionMode = None` and `IPv4`
> 2. **Ambiguous Method Call** - Solved by explicitly passing `CancellationToken.None`

---

## 🚀 How to Run the Project

### Prerequisites:
- .NET 10.0 SDK
- An FTP server running (or use local FTP)

### Steps:
1. Open terminal in project folder
2. Run: `dotnet run`
3. Open browser to `https://localhost:7000` (or port shown)

### To change port:
Edit `Properties/launchSettings.json` - find `applicationUrl` setting.

---

## 📞 Quick Reference Card

| Task | File to Edit |
|------|--------------|
| Change title text | `Pages/Index.cshtml` line 6, 9 |
| Change colors | `Pages/Index.cshtml` (btn classes) |
| Change FTP server | `appsettings.json` |
| Add new feature | `Pages/Index.cshtml.cs` |
| Change layout | `Pages/Shared/_Layout.cshtml` |
| Custom styles | `wwwroot/css/site.css` |

---

## 👨‍🎓 Student Info

**Project:** SproutFTP - FTP File Manager  
**Student:** นายวีรพงศ์ วสุมงคลพจน์  
**ID:** 670112418022  
**Major:** สาขาเทคโนโลยีสารสนเทศ หมู่ 1

---

*Last Updated: February 2026*
