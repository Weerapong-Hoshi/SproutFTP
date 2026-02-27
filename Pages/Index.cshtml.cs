using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SproutFTP.Pages
{
    /// หน้า Index ของเว็บแอปพลิเคชัน SproutFTP
    /// ทำหน้าที่เป็นหน้าหลักสำหรับจัดการไฟล์ผ่าน FTP Server
    public class IndexModel : PageModel
    {
        // ตัวแปรสำหรับเก็บการตั้งค่าคอนฟิกจาก appsettings.json
        private readonly IConfiguration _config;

        // ตัวแปรสำหรับเก็บรายการไฟล์ที่ดึงมาจาก FTP Server
        public List<FtpListItem> FileList { get; set; } = new List<FtpListItem>();

        // ตัวแปรสำหรับรับไฟล์ที่อัปโหลดจากฟอร์ม
        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        /// Constructor สำหรับรับการตั้งค่าคอนฟิก
        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        /// สร้างและตั้งค่าการเชื่อมต่อ FTP Client
        private AsyncFtpClient GetFtpClient()
        {
            // ดึงค่าการตั้งค่าจาก appsettings.json หรือใช้ค่าดีฟอลต์
            var host = _config["FtpSettings:Host"] ?? "127.0.0.1";
            var user = _config["FtpSettings:User"] ?? "admin";
            var pass = _config["FtpSettings:Pass"] ?? "1234";

            // สร้างการตั้งค่าการเชื่อมต่อ FTP
            var config = new FtpConfig
            {
                // ปิดการเข้ารหัส SSL เพื่อหลีกเลี่ยงปัญหาการเชื่อมต่อ
                EncryptionMode = FtpEncryptionMode.None,
                // บังคับใช้ IPv4 เท่านั้น
                InternetProtocolVersions = FtpIpVersion.IPv4,
                // ตั้งค่า Timeout สำหรับการเชื่อมต่อ
                ConnectTimeout = 10000,
            };

            // สร้างและคืนค่า FTP Client พร้อมการตั้งค่า
            return new AsyncFtpClient(host, user, pass, 21, config);
        }

        /// ฟังก์ชันสำหรับโหลดรายการไฟล์จาก FTP Server
        private async Task LoadFileList()
        {
            try
            {
                // ใช้ FTP Client ที่ตั้งค่าไว้
                using var client = GetFtpClient();
                // เชื่อมต่อกับ FTP Server
                await client.Connect();
                
                // ดึงรายการไฟล์และโฟลเดอร์จาก FTP Server
                var items = await client.GetListing(null, FtpListOption.ForceList);
                FileList = items.ToList();
            }
            catch (Exception ex)
            {
                // ถ้าเกิดข้อผิดพลาด ให้ล้างรายการไฟล์และแสดงข้อความ error
                FileList = new List<FtpListItem>();
                ModelState.AddModelError("", $"โหลดไฟล์ไม่สำเร็จ: {ex.Message}");
            }
        }

        /// Handler สำหรับการเข้าถึงหน้าเว็บ (GET request)
        /// จะถูกเรียกใช้เมื่อมีการเข้าถึงหน้า Index
        public async Task OnGetAsync()
        {
            // โหลดรายการไฟล์มาแสดง
            await LoadFileList();
        }

        /// Handler สำหรับการอัปโหลดไฟล์ (POST request)
        /// จะถูกเรียกใช้เมื่อมีการส่งฟอร์มอัปโหลดไฟล์
        public async Task<IActionResult> OnPostUploadAsync()
        {
            // ตรวจสอบว่ามีการเลือกไฟล์หรือไม่
            if (UploadedFile == null)
            {
                ModelState.AddModelError("", "กรุณาเลือกไฟล์");
                // โหลดรายการไฟล์กลับมาแสดงก่อนจบการทำงาน
                await LoadFileList();
                return Page();
            }

            try
            {
                // ใช้ FTP Client ที่ตั้งค่าไว้
                using var client = GetFtpClient();
                // เชื่อมต่อกับ FTP Server
                await client.Connect();

                // อัปโหลดไฟล์
                using (var stream = UploadedFile.OpenReadStream())
                {
                    // อัปโหลดไฟล์ (ไม่ใส่ / นำหน้า เพื่อลงในโฟลเดอร์ปัจจุบัน)
                    // FtpRemoteExists.Overwrite: ถ้ามีไฟล์ชื่อเดิมให้เขียนทับ
                    await client.UploadStream(stream, UploadedFile.FileName, FtpRemoteExists.Overwrite);
                }
            }
            catch (Exception ex)
            {
                // ถ้าอัปโหลดไม่สำเร็จ ให้แสดงข้อความ error
                ModelState.AddModelError("", $"อัปโหลดพลาด: {ex.Message}");
                // โหลดรายการไฟล์กลับมาแสดงกรณี Error
                await LoadFileList();
                return Page();
            }

            // ถ้าอัปโหลดสำเร็จ ให้รีเฟรชหน้าใหม่ (จะไปเรียก OnGet เอง)
            return RedirectToPage();
        }

        /// Handler สำหรับการดาวน์โหลดไฟล์ (GET request)
        /// จะถูกเรียกใช้เมื่อมีการคลิกปุ่มดาวน์โหลด
        public async Task<IActionResult> OnGetDownloadAsync(string fileName)
        {
            // ตรวจสอบว่ามีการระบุชื่อไฟล์หรือไม่
            if (string.IsNullOrEmpty(fileName)) return BadRequest();

            // ใช้ FTP Client ที่ตั้งค่าไว้
            using var client = GetFtpClient();
            // เชื่อมต่อกับ FTP Server
            await client.Connect();

            // ดาวน์โหลดไฟล์เป็น byte array
            // CancellationToken.None แก้ปัญหา Ambiguous call
            var bytes = await client.DownloadBytes(fileName, CancellationToken.None);

            // ถ้าไม่พบไฟล์ ให้ส่ง NotFound
            if (bytes == null) return NotFound();
            
            // ส่งไฟล์กลับไปยังผู้ใช้
            return File(bytes, "application/octet-stream", fileName);
        }

        /// Handler สำหรับการลบไฟล์ (POST request)
        /// จะถูกเรียกใช้เมื่อมีการคลิกปุ่มลบ
        public async Task<IActionResult> OnPostDeleteAsync(string fileName)
        {
            // ตรวจสอบว่ามีการระบุชื่อไฟล์หรือไม่
            if (string.IsNullOrEmpty(fileName)) return BadRequest();

            // ใช้ FTP Client ที่ตั้งค่าไว้
            using var client = GetFtpClient();
            // เชื่อมต่อกับ FTP Server
            await client.Connect();
            
            // ลบไฟล์
            await client.DeleteFile(fileName);

            // ลบสำเร็จ ให้รีเฟรชหน้าใหม่
            return RedirectToPage();
        }
    }
}
