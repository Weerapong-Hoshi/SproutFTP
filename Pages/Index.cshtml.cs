using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace SproutFTP.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        public List<FtpListItem> FileList { get; set; } = new List<FtpListItem>();

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        // สร้าง Connection Config (บังคับ IPv4 + No SSL แก้ปัญหา Timeout)
        private AsyncFtpClient GetFtpClient()
        {
            var host = _config["FtpSettings:Host"] ?? "127.0.0.1";
            var user = _config["FtpSettings:User"] ?? "admin";
            var pass = _config["FtpSettings:Pass"] ?? "1234";

            var config = new FtpConfig
            {
                EncryptionMode = FtpEncryptionMode.None, // ปิด SSL
                InternetProtocolVersions = FtpIpVersion.IPv4, // บังคับ IPv4
                ConnectTimeout = 10000,
            };

            return new AsyncFtpClient(host, user, pass, 21, config);
        }

        // ฟังก์ชันโหลดรายชื่อไฟล์ (แยกออกมาเรียกใช้ซ้ำได้)
        private async Task LoadFileList()
        {
            try
            {
                using var client = GetFtpClient();
                await client.Connect();
                // ForceList: บังคับโหลดใหม่ไม่ใช้ Cache, null: หาจาก Folder ปัจจุบัน
                var items = await client.GetListing(null, FtpListOption.ForceList);
                FileList = items.ToList();
            }
            catch (Exception ex)
            {
                FileList = new List<FtpListItem>();
                ModelState.AddModelError("", $"โหลดไฟล์ไม่สำเร็จ: {ex.Message}");
            }
        }

        public async Task OnGetAsync()
        {
            await LoadFileList();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (UploadedFile == null)
            {
                ModelState.AddModelError("", "กรุณาเลือกไฟล์");
                await LoadFileList(); // โหลดตารางกลับมาแสดงก่อนจบการทำงาน
                return Page();
            }

            try
            {
                using var client = GetFtpClient();
                await client.Connect();

                using (var stream = UploadedFile.OpenReadStream())
                {
                    // อัปโหลดไฟล์ (ไม่ใส่ / นำหน้า เพื่อลงในโฟลเดอร์ปัจจุบัน)
                    await client.UploadStream(stream, UploadedFile.FileName, FtpRemoteExists.Overwrite);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"อัปโหลดพลาด: {ex.Message}");
                await LoadFileList(); // โหลดตารางกลับมาแสดงกรณี Error
                return Page();
            }

            // สำเร็จ -> รีเฟรชหน้าใหม่ (จะไปเรียก OnGet เอง)
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDownloadAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest();

            using var client = GetFtpClient();
            await client.Connect();

            // CancellationToken.None แก้ปัญหา Ambiguous call
            var bytes = await client.DownloadBytes(fileName, CancellationToken.None);

            if (bytes == null) return NotFound();
            return File(bytes, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> OnPostDeleteAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest();

            using var client = GetFtpClient();
            await client.Connect();
            await client.DeleteFile(fileName);

            return RedirectToPage();
        }
    }
}