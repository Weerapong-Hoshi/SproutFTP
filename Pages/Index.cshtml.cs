using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SproutFTP_Project.Pages
{
    public class IndexModel : PageModel
    {
        // --- CONFIG ---
        private const string FtpHost = "127.0.0.1";
        private const int FtpPort = 21;
        private const string FtpUser = "testuser";
        private const string FtpPass = "testpass";

        // ตัวแปรสำหรับส่งไปหน้า View
        public List<FtpListItem> Files { get; set; } = new();

        [TempData] // ใช้ TempData เพื่อให้ข้อความยังอยู่แม้จะ Refresh หน้า
        public string? Message { get; set; }

        // ฟังก์ชันเชื่อมต่อ (Stateless: สร้าง-ใช้-ทิ้ง)
        private AsyncFtpClient GetClient()
        {
            var client = new AsyncFtpClient(FtpHost, FtpUser, FtpPass, FtpPort);
            client.Config.ValidateAnyCertificate = true;
            // สำคัญ: Docker บน Localhost มักต้องการ Passive Mode
            client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
            return client;
        }

        // 1. เปิดหน้าเว็บ (GET)
        public async Task OnGetAsync()
        {
            await LoadFiles();
        }

        // 2. อัปโหลดไฟล์ (POST)
        // ชื่อ parameter 'uploadedFile' ต้องตรงกับ name="uploadedFile" ในหน้า html
        public async Task<IActionResult> OnPostUploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                Message = "Error: Please select a file.";
                return RedirectToPage();
            }

            try
            {
                using var client = GetClient();
                await client.Connect();

                // อ่าน Stream จาก HTTP Post แล้วส่งเข้า FTP ทันที
                using var stream = uploadedFile.OpenReadStream();
                string remotePath = "/" + uploadedFile.FileName;

                await client.UploadStream(stream, remotePath, FtpRemoteExists.Overwrite, createRemoteDir: false);
                await client.Disconnect();

                Message = $"Success: Uploaded '{uploadedFile.FileName}' successfully!";
            }
            catch (Exception ex)
            {
                Message = $"Error Uploading: {ex.Message}";
            }

            return RedirectToPage();
        }

        // 3. ลบไฟล์ (POST)
        public async Task<IActionResult> OnPostDeleteAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return RedirectToPage();

            try
            {
                using var client = GetClient();
                await client.Connect();

                string remotePath = "/" + fileName;

                // ตรวจสอบว่าเป็นไฟล์หรือโฟลเดอร์ก่อนลบ
                if (await client.DirectoryExists(remotePath))
                {
                    await client.DeleteDirectory(remotePath);
                }
                else if (await client.FileExists(remotePath))
                {
                    await client.DeleteFile(remotePath);
                }

                await client.Disconnect();
                Message = $"Success: Deleted '{fileName}'.";
            }
            catch (Exception ex)
            {
                Message = $"Error Deleting: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadFiles()
        {
            try
            {
                using var client = GetClient();
                await client.Connect();
                var items = await client.GetListing("/");
                Files = items.ToList();
                await client.Disconnect();
            }
            catch (Exception ex)
            {
                // ถ้า Connect ไม่ได้ ให้โชว์ Error แต่ไม่ให้โปรแกรมพัง
                Message = string.IsNullOrEmpty(Message) ? $"Error Loading Files: {ex.Message}" : Message;
                Files = new List<FtpListItem>();
            }
        }
    }
}