# 🌱 Sprout FTP

> เว็บแอปพลิเคชันสำหรับอัปโหลด ดาวน์โหลด และจัดการไฟล์รูปภาพ ผ่านระบบ FTP โดยมีหน้าเว็บเป็นตัวกลาง

---

# 📌 1. โปรเจกต์นี้คืออะไร?

Sprout FTP คือเว็บแอปพลิเคชันที่ทำหน้าที่เป็น "ตัวกลาง" ระหว่างผู้ใช้งานกับ FTP Server

แทนที่ผู้ใช้จะต้องใช้โปรแกรม FTP โดยตรง (เช่น FileZilla) โปรเจกต์นี้ทำให้สามารถ:

- 📤 อัปโหลดไฟล์ผ่านหน้าเว็บ
- 📥 ดาวน์โหลดไฟล์ผ่านหน้าเว็บ
- 🗂 ดูรายการไฟล์ทั้งหมด
- 🗑 ลบไฟล์
- ✏ เปลี่ยนชื่อไฟล์ (สามารถต่อยอดเพิ่มได้)

ทั้งหมดทำผ่าน Web Browser ปกติ

---

# 🧠 2. FTP คืออะไร? (สำหรับผู้เริ่มต้น)

**FTP (File Transfer Protocol)** คือมาตรฐานการรับส่งไฟล์ผ่านเครือข่าย

ทำงานแบบ Client–Server Model

- Client = ฝั่งผู้ร้องขอ (เช่น โปรแกรม หรือเว็บแอปของเรา)
- Server = เครื่องที่เก็บไฟล์จริง

### การทำงานพื้นฐาน

1. Client เชื่อมต่อไปยัง Server
2. Login ด้วย Username / Password
3. ส่งคำสั่ง เช่น:
   - LIST → ขอรายชื่อไฟล์
   - STOR → อัปโหลดไฟล์
   - RETR → ดาวน์โหลดไฟล์
   - DELE → ลบไฟล์

4. Server ตอบกลับตามคำสั่ง

---

# 🏗 3. สถาปัตยกรรมของระบบ (ภาพรวมใหญ่)

ผู้ใช้ → เว็บเบราว์เซอร์ → ASP.NET Web App → FTP Server → พื้นที่เก็บไฟล์

### อธิบายง่าย ๆ

- ผู้ใช้กดปุ่มบนเว็บ
- เว็บแอป (C#) รับคำสั่ง
- เว็บแอปไปคุยกับ FTP Server
- FTP Server จัดการไฟล์
- ผลลัพธ์ถูกส่งกลับมาแสดงบนหน้าเว็บ

---

# 🔌 4. พอร์ตที่ใช้ใน FTP

## 🔹 Port 21 (Control Port)

ใช้ส่งคำสั่ง เช่น Login, LIST, STOR, RETR

เปรียบเหมือน “ห้องควบคุม”

## 🔹 Data Port (21100–21110)

ใช้ส่งข้อมูลไฟล์จริง ๆ

เปรียบเหมือน “ประตูขนส่งสินค้า”

โปรเจกต์นี้ใช้ **Passive Mode** ซึ่งปลอดภัยและเหมาะกับ Docker

---

# 🧰 5. เทคโนโลยีที่ใช้ในโปรเจกต์

## 🔹 C#

ภาษาโปรแกรมหลักของฝั่ง Server

## 🔹 ASP.NET Razor Pages

เฟรมเวิร์กสำหรับสร้างเว็บแบบ Server-Side Rendering

## 🔹 FluentFTP

ไลบรารีสำหรับเชื่อมต่อและสั่งงาน FTP ด้วย C#

## 🔹 HTML + CSS + Bootstrap

ใช้สร้างและตกแต่งหน้าเว็บ

## 🔹 Docker

ใช้สร้าง FTP Server จำลองในเครื่องเรา

---

# 🐳 6. การทำงานของ Docker

เราใช้ Image:

`delfer/alpine-ftp-server`

ซึ่งภายในมี FTP Server ชื่อ **vsftpd** ติดตั้งไว้แล้ว

### docker-compose.yml

```yaml
version: "3.8"
services:
  ftp-server:
    image: delfer/alpine-ftp-server
    container_name: sprout-ftp-server
    ports:
      - "21:21"
      - "21100-21110:21100-21110"
    environment:
      - USERS=admin|1234
      - ADDRESS=127.0.0.1
      - MIN_PORT=21100
      - MAX_PORT=21110
    volumes:
      - ./ftp-data:/ftp/admin
```

### คำอธิบาย

- USERS → สร้าง user admin รหัส 1234
- ports → เปิดพอร์ตสำหรับ Control และ Passive Mode
- volumes → ผูกโฟลเดอร์ ftp-data ในเครื่องเรา กับโฟลเดอร์ใน Container

---

# 🚀 7. วิธีรันโปรเจกต์

## ขั้นตอนที่ 1

ติดตั้ง Docker

## ขั้นตอนที่ 2

รันคำสั่ง:

```bash
docker-compose up
```

## ขั้นตอนที่ 3

เปิดเว็บที่:

[http://localhost](http://localhost)

## หยุดระบบ

```bash
docker-compose down
```

---

# 📂 8. โครงสร้างโปรเจกต์

```
SproutFTP/
│
├── docker-compose.yml
├── appsettings.json
├── Program.cs
├── Pages/
│   ├── Index.cshtml
│   └── Index.cshtml.cs
├── ftp-data/
└── SproutFTP.csproj
```

---

# 🔄 9. ลำดับการทำงานของระบบ

## เมื่อเปิดหน้าเว็บ

1. Browser ส่ง HTTP GET
2. Web App เชื่อมต่อ FTP
3. ดึงรายการไฟล์
4. ส่ง HTML กลับไปแสดงผล

## เมื่ออัปโหลดไฟล์

1. Browser ส่ง HTTP POST พร้อมไฟล์
2. Web App รับ IFormFile
3. ใช้ FluentFTP อัปโหลด
4. Redirect กลับหน้าเดิม

## เมื่อดาวน์โหลด

1. Browser ส่ง HTTP GET
2. Web App ดึงไฟล์จาก FTP
3. ส่งไฟล์กลับด้วย IActionResult

## เมื่อลบไฟล์

1. Browser ส่ง HTTP POST
2. Web App สั่ง DeleteFile
3. Redirect กลับหน้าเดิม

---

# ⚡ 10. แนวคิดสำคัญที่ใช้ในโค้ด

## Async / Await

ช่วยให้เว็บไม่ค้างเวลารออัปโหลดไฟล์ใหญ่

## IFormFile

ตัวแทนไฟล์ที่ผู้ใช้ส่งมา

## IActionResult

คำตอบที่ Server ส่งกลับไปยัง Browser

## FtpRemoteExists.Overwrite

ถ้าไฟล์ชื่อซ้ำ ให้ทับของเดิม

---

# 🔐 11. ความปลอดภัยที่ควรเพิ่มในอนาคต

- จำกัดขนาดไฟล์
- ตรวจสอบประเภทไฟล์จริง (ไม่ดูแค่นามสกุล)
- เพิ่มระบบ Login หน้าเว็บ
- เข้ารหัส FTP (FTPS)

---

# 🎯 สรุปแนวคิดของโปรเจกต์นี้

Docker = ตู้เก็บไฟล์

FTP Server = ระบบจัดการตู้

ASP.NET Web App = พนักงานรับคำสั่ง

Browser = ลูกค้า

ผู้ใช้กดปุ่ม → เว็บรับคำสั่ง → เว็บไปคุยกับ FTP → FTP จัดการไฟล์ → เว็บแสดงผล

---

# 📚 เหมาะกับใคร?

- ผู้เริ่มต้นที่อยากเข้าใจ FTP
- นักศึกษาที่ต้องการเข้าใจ Client–Server
- คนที่อยากฝึกใช้ Docker กับ Web App

---

# 🌟 แนวทางพัฒนาต่อ

- เพิ่มระบบสมัครสมาชิก
- ทำระบบโฟลเดอร์ย่อย
- ทำ Preview รูปภาพ
- เพิ่ม Progress Bar ตอนอัปโหลด
- เปลี่ยนจาก FTP เป็น Cloud Storage

---

# ✅ จบภาพรวมทั้งหมดของระบบ Sprout FTP

หากเข้าใจ README นี้ คุณจะเข้าใจทั้ง:

- โครงสร้าง Web App
- การทำงานของ FTP
- การใช้ Docker จำลอง Server
- การเชื่อมต่อเครือข่ายพื้นฐาน

นี่คือโปรเจกต์ที่ดีมากสำหรับการเรียนรู้พื้นฐานระบบเครือข่ายและ Web Backend 🎉
