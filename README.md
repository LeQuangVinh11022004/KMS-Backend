<div align="center">

<img src="https://img.shields.io/badge/SP26SE095-Capstone%20Project-brightgreen?style=for-the-badge" />

# 🌞 KMS — Sunshine Kindergarten Management System

**A centralized digital platform to manage students, teachers, classes, attendance, tuition, and parent communication — all in one place.**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![JWT](https://img.shields.io/badge/Auth-JWT-000000?style=flat-square&logo=jsonwebtokens)](https://jwt.io/)
[![Swagger](https://img.shields.io/badge/API%20Docs-Swagger-85EA2D?style=flat-square&logo=swagger)](https://swagger.io/)

</div>

---

## 📖 Overview

Sunshine Kindergarten currently handles daily operations through notebooks, Excel sheets, and chat groups — leading to fragmented data, duplicated workloads, and poor transparency for parents.

**KMS** replaces all of that with a unified system where:
- Admins manage the entire school from a single dashboard
- Teachers take attendance and post class activities digitally
- Parents stay informed about their child in real time
- Accountants handle tuition and payments without spreadsheets

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────┐
│                   CLIENT LAYER                      │
│                                                     │
│   Web Admin (ReactJS)    Mobile App (ReactNative)   │
│   Admin   /              Teacher / Parent           │
│   Accountant/Staff                                  │
└────────────────────┬────────────────────────────────┘
                     │ HTTPS / REST API
┌────────────────────▼────────────────────────────────┐
│                   API LAYER                         │
│                                                     │
│         .NET 9 Web API  (3-Layer Architecture)      │
│         Controller → Service → Repository           │
│         JWT Authentication + Role-based Access      │
└────────────────────┬────────────────────────────────┘
                     │ Entity Framework Core
┌────────────────────▼────────────────────────────────┐
│                DATABASE LAYER                       │
│                                                     │
│              PostgreSQL                             │
│              KMS_SP26SE095                          │
└─────────────────────────────────────────────────────┘
```

---

## ✨ Features

### 👤 Admin / Principal
- Manage students, teachers, classes, and school years
- Assign teachers to classes, enroll students
- Manage user accounts, roles, and permissions
- View system-wide reports (attendance, tuition, announcements)
- Manage system settings (school name, max class size, etc.)

### 👩‍🏫 Teacher
- Take daily attendance (Present / Absent / Late / Excused / Sick)
- Post class activities with photos
- Manage class timetable and daily menus
- Report medical incidents and health checks
- Evaluate students (Weekly/Monthly/Semester)

### 💰 Accountant / Staff
- Manage tuition fee templates
- Generate monthly invoices for students
- Confirm and track payment status
- View revenue reports

### 👪 Parent
- Register children for enrollment (online form)
- View child's daily attendance history
- Receive school announcements and notifications
- View class activities, daily menus, and timetable
- View tuition invoices and payment status
- Ask AI about their children
  
---

## 🗂️ Module Overview

| Module | Description |
|--------|-------------|
| **Auth** | JWT login, role-based access (Admin, Principal, Teacher, Accountant, Parent) |
| **User Management** | CRUD users, assign roles |
| **Password Management** | Admin reset, self-change, OTP-based forgot password |
| **Parent Registration** | Online enrollment form → Admin approve/reject → auto-create account + send email |
| **Student** | Student profiles, medical info, enrollment |
| **Teacher** | Teacher profiles, class assignments |
| **Campuses** | Multi-campus support |
| **School Years / Semesters** | Academic calendar management |
| **Classes** | Create classes per school year, manage capacity |
| **Class Assignment** | Enroll students, assign teachers (Main/Assistant) |
| **Student-Parent** | Link parents to students, manage primary contact |
| **Attendance** | Daily attendance sheet per class, history, checkout time |
| **Timetable** | Weekly schedule per class |
| **Menus** | Daily meal plans per class |
| **Class Activities** | Daily activity posts with photos/media |
| **Tuition Template** | Reusable fee templates and custom tuition items |
| **Invoice** | Monthly tuition invoices generation and tracking |
| **Payment** | Payment confirmation and tracking (integrated with VNPay Sandbox) |
| **Announcements** | School-wide or class-specific announcements with priority targets |
| **Notifications** | Real-time system notifications per user type |
| **Health Checks** | Periodic physical health record (height, weight, vision) per student |
| **Medical Incidents** | Log campus injuries or sickness events and actions taken |
| **Daily Reports** | Report daily about living and educational activities of children |
| **Student Evaluation** | Periodic evaluation (Phiếu bé ngoan) based on custom metrics (Academic, Behavior, Hygiene) |
| **Chat Histories** | AI Chatbot to support parents know what their child do based on daily reports |
| **System Settings** | Configurable school-wide settings and global keys |

---

## 🛠️ Tech Stack

**Backend**
- ASP.NET Core 9 Web API
- Entity Framework Core (DB First)
- PostgreSQL 
- BCrypt.Net (password hashing)
- JWT Bearer Authentication
- Gmail SMTP (email notifications)
- Swagger / OpenAPI

**Frontend**
- ReactJS/NextJS + TypeScript (Web Admin)
- React Native (Mobile — Teacher & Parent)

**Infrastructure**
- Azure / AWS (deployment)
- Supabase (deployment DB)
- GitHub (version control)

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) — Web Admin (React)
- [Node.js 18+](https://nodejs.org/) — Mobile App (React Native)
- Android Studio — Android Emulator for React Native

### Backend Setup

```bash
# 1. Clone the repository
git clone https://github.com/your-org/KMS.CapstoneProject.git
cd KMS.CapstoneProject

# 2. Create the database
# Run the SQL script in /database/KMS_Schema.sql on your PostgreSQL instance / Supabase

# 3. Configure connection string
# Edit KMS.API/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-supabase-db.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-password;"
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE"
  },
  "VNPay": {
    "TmnCode": "YOUR_VNPAY_TMN_CODE",
    "HashSecret": "YOUR_VNPAY_HASH_SECRET",
    "BaseUrl": "[https://sandbox.vnpayment.vn/paymentv2/vpcpay.html](https://sandbox.vnpayment.vn/paymentv2/vpcpay.html)"
  }
}

Other settings such as JWT and Email can be configured in `appsettings.json`.

# 4. Run the API
cd KMS.API
dotnet run

# API will be available at https://localhost:7137
# Swagger UI at https://localhost:7137/swagger
```

---
## 🔑 Demo Accounts

Để tiện cho Hội đồng nghiệm thu và kiểm thử hệ thống, nhóm đã chuẩn bị sẵn các tài khoản demo tương ứng với từng phân quyền (Role) trong hệ thống:

| Role | Username| Password | Ghi chú |
|------|------------------|----------|---------|
| **Admin** | admin001 | Admin@123 Toàn quyền quản trị hệ thống |
| **Teacher** | teacher001| Admin@123 | Giáo viên điểm danh, báo cáo |
| **Parent** | parent001 | Admin@123 | Phụ huynh |

*Lưu ý: Hệ thống đang sử dụng môi trường VNPay Sandbox để test thanh toán, thông tin thẻ test được mô tả chi tiết trong tài liệu đính kèm.*

## 👥 Team

| Role | Responsibility |
|------|---------------|
| Backend Developer | .NET API, Database Design |
| Frontend Developer | ReactJS/NextJS Web Admin |
| Mobile Developer | React Native Teacher & Parent App |
| QA / Documentation | Testing, SRS, User Manual |


