# 🏋️‍♂️ DACS_MyGymScheduler - Hệ Thống Đặt Lịch Tập Gym Và Tìm Huấn Luyện Viên

## 📋 Mô Tả Dự Án

**DACS_MyGymScheduler** là một ứng dụng web ASP.NET Core được phát triển để quản lý và đặt lịch tập gym, tìm kiếm huấn luyện viên, và quản lý hệ thống phòng tập. Dự án được xây dựng với kiến trúc MVC (Model-View-Controller) và tích hợp nhiều tính năng hiện đại.

## ✨ Tính Năng Chính

### 👥 Quản Lý Người Dùng
- **Đăng ký/Đăng nhập**: Hệ thống xác thực với email và mạng xã hội (Facebook, Google)
- **Quản lý hồ sơ**: Cập nhật thông tin cá nhân, avatar
- **Xác thực email**: Gửi email xác nhận khi đăng ký
- **Phân quyền**: Hệ thống role-based access control

### 🏢 Quản Lý Phòng Gym
- **Thêm/Sửa/Xóa phòng gym**: Quản lý thông tin phòng tập
- **Tìm kiếm phòng gym**: Tìm kiếm theo địa chỉ, loại phòng
- **Đánh giá và review**: Hệ thống rating cho phòng gym
- **Quản lý tiện ích**: Mô tả các tiện ích của phòng gym

### 👨‍🏫 Quản Lý Huấn Luyện Viên
- **Đăng ký trainer**: Thêm huấn luyện viên mới
- **Quản lý chuyên môn**: Phân loại theo chuyên môn
- **Tìm kiếm trainer**: Tìm kiếm theo chuyên môn, địa điểm
- **Đánh giá trainer**: Hệ thống review cho huấn luyện viên

### 📚 Quản Lý Lớp Học
- **Tạo lớp học**: Đăng ký các lớp tập mới
- **Lịch trình**: Quản lý lịch học theo ngày, giờ
- **Đăng ký lớp**: Người dùng đăng ký tham gia lớp học
- **Check-in**: Hệ thống điểm danh cho lớp học

### 💳 Quản Lý Gói Thành Viên
- **Các loại gói**: Bronze, Silver, Gold với quyền lợi khác nhau
- **Thanh toán**: Tích hợp VNPay để thanh toán online
- **Quản lý thời hạn**: Theo dõi thời hạn gói thành viên
- **Gia hạn**: Tự động nhắc nhở gia hạn gói

### 🔍 Tìm Kiếm Nâng Cao
- **Tìm kiếm phòng gym**: Theo địa chỉ, loại phòng, giá cả
- **Tìm kiếm trainer**: Theo chuyên môn, địa điểm
- **Tìm kiếm lớp học**: Theo loại lớp, thời gian, địa điểm

### 📧 Hệ Thống Email
- **Email xác nhận**: Gửi email xác nhận đăng ký
- **Email nhắc nhở**: Nhắc nhở lịch học, gia hạn gói
- **Email hàng tuần**: Báo cáo hoạt động hàng tuần

## 🛠️ Công Nghệ Sử Dụng

### Backend
- **ASP.NET Core 8.0**: Framework chính
- **Entity Framework Core**: ORM cho database
- **SQL Server**: Hệ quản trị cơ sở dữ liệu
- **Dapper**: Micro ORM cho truy vấn phức tạp
- **AutoMapper**: Mapping giữa các object

### Frontend
- **Bootstrap 4.2.1**: Framework CSS
- **jQuery 3.3.1**: JavaScript library
- **Font Awesome**: Icon library
- **Responsive Design**: Tương thích mobile

### Authentication & Authorization
- **Cookie Authentication**: Xác thực bằng cookie
- **Facebook Authentication**: Đăng nhập bằng Facebook
- **Google Authentication**: Đăng nhập bằng Google

### Payment Integration
- **VNPay**: Tích hợp thanh toán online
- **Sandbox Environment**: Môi trường test thanh toán

### Background Services
- **Hangfire**: Background job processing
- **Memory Storage**: Lưu trữ job trong memory
- **Recurring Jobs**: Tự động gửi email hàng tuần

### Email Services
- **MailKit**: Thư viện gửi email
- **SMTP Gmail**: Sử dụng Gmail SMTP
- **HTML Templates**: Template email đẹp mắt

## 📁 Cấu Trúc Dự Án

```
DACS_MyGymScheduler/
├── toanggg/
│   ├── Controllers/           # Controllers cho các chức năng
│   ├── Data/                  # Entity models và DbContext
│   ├── Models/                # ViewModels và DTOs
│   ├── Services/              # Business logic services
│   ├── Helpers/               # Utility classes
│   ├── Views/                 # Razor views
│   ├── wwwroot/              # Static files (CSS, JS, Images)
│   ├── Areas/                # Area routing (Admin, Trainers)
│   └── Properties/           # Project properties
```

### Các Entity Chính

- **User**: Người dùng hệ thống
- **Gym**: Phòng tập gym
- **Trainer**: Huấn luyện viên
- **Class**: Lớp học
- **ClassSchedule**: Lịch trình lớp học
- **MembershipType**: Loại gói thành viên
- **UserMembership**: Gói thành viên của user
- **GymType**: Loại phòng gym

## 🚀 Cài Đặt Và Chạy Dự Án

### Yêu Cầu Hệ Thống
- **.NET 8.0 SDK**
- **SQL Server 2019+**
- **Visual Studio 2022** hoặc **VS Code**

### Bước 1: Clone Repository
```bash
git clone https://github.com/phn47/DACS_MyGymScheduler.git
cd DACS_MyGymScheduler
```

### Bước 2: Cấu Hình Database
1. Tạo database SQL Server với tên `DACS`
2. Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "QLPHONGTAP": "Data Source=YOUR_SERVER;Initial Catalog=DACS;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"
  }
}
```

### Bước 3: Cấu Hình Email
Cập nhật thông tin email trong `appsettings.json`:
```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": "587",
      "Username": "your-email@gmail.com",
      "Password": "your-app-password",
      "EnableSsl": true
    }
  }
}
```

### Bước 4: Cấu Hình VNPay
Cập nhật thông tin VNPay trong `appsettings.json`:
```json
{
  "VnPay": {
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
  }
}
```

### Bước 5: Cấu Hình OAuth
Cập nhật thông tin Facebook và Google OAuth:
```json
{
  "Authentication": {
    "Facebook": {
      "AppId": "YOUR_FACEBOOK_APP_ID",
      "AppSecret": "YOUR_FACEBOOK_APP_SECRET"
    },
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

### Bước 6: Chạy Migration
```bash
dotnet ef database update
```

### Bước 7: Chạy Dự Án
```bash
dotnet run
```

Truy cập ứng dụng tại: `https://localhost:7014`

## 🔧 Các Package NuGet Chính

- **Microsoft.EntityFrameworkCore**: ORM framework
- **Microsoft.EntityFrameworkCore.SqlServer**: SQL Server provider
- **Dapper**: Micro ORM
- **AutoMapper.Extensions.Microsoft.DependencyInjection**: Object mapping
- **Hangfire**: Background job processing
- **MailKit**: Email sending
- **Microsoft.AspNetCore.Authentication.Facebook**: Facebook OAuth
- **Microsoft.AspNetCore.Authentication.Google**: Google OAuth

## 📊 Database Schema

### Bảng Chính
- **Users**: Thông tin người dùng
- **Gyms**: Thông tin phòng gym
- **Trainers**: Thông tin huấn luyện viên
- **Classes**: Thông tin lớp học
- **ClassSchedules**: Lịch trình lớp học
- **MembershipTypes**: Loại gói thành viên
- **UserMemberships**: Gói thành viên của user
- **GymTypes**: Loại phòng gym

## 🔐 Bảo Mật

- **HTTPS**: Sử dụng HTTPS cho tất cả kết nối
- **Cookie Authentication**: Xác thực bằng cookie an toàn
- **OAuth 2.0**: Đăng nhập bằng mạng xã hội
- **Email Verification**: Xác thực email khi đăng ký
- **Input Validation**: Validate tất cả input từ user
- **SQL Injection Protection**: Sử dụng parameterized queries

## 📈 Tính Năng Nâng Cao

### Background Services
- **Weekly Email Service**: Gửi email báo cáo hàng tuần
- **Membership Cleanup**: Tự động xử lý gói hết hạn
- **Hangfire Dashboard**: Quản lý background jobs

### Payment Integration
- **VNPay Integration**: Thanh toán online
- **Payment Callback**: Xử lý callback từ VNPay
- **Order Management**: Quản lý đơn hàng thanh toán

### Search Functionality
- **Advanced Search**: Tìm kiếm nâng cao
- **Filter Options**: Bộ lọc theo nhiều tiêu chí
- **Search Service**: Service riêng cho tìm kiếm

## 🤝 Đóng Góp

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📝 License

Dự án này được phát triển cho mục đích học tập và nghiên cứu.

## 👨‍💻 Tác Giả

**DACS Team** - Dự án môn học Database Application Development

## 📞 Liên Hệ

- **Email**: phamnam1449@gmail.com
- **GitHub**: [https://github.com/phn47/DACS_MyGymScheduler](https://github.com/phn47/DACS_MyGymScheduler)

---

⭐ Nếu dự án này hữu ích, hãy cho chúng tôi một star! 