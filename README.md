<div align="center">

<img src="https://img.shields.io/badge/SP26SE095-Capstone%20Project-brightgreen?style=for-the-badge" />

# 🌞 KMS — Sunshine Kindergarten Management System

**A centralized digital platform to manage students, teachers, classes, attendance, tuition, and parent communication — all in one place.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
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
│         .NET 8 Web API  (3-Layer Architecture)      │
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
| **Parent Registration** | Online enrollment form → Admin approve/reject → auto-create account + send email |
| **Student** | Student profiles, medical info, enrollment |
| **Teacher** | Teacher profiles, class assignments |
| **Classes** | Create classes per school year, manage capacity |
| **Class Assignment** | Enroll students, assign teachers (Main/Assistant) |
| **Student-Parent** | Link parents to students, manage primary contact |
| **Attendance** | Daily attendance sheet per class, history, checkout time |
| **Announcements** | School-wide or class-specific announcements |
| **Notifications** | System notifications per user |
| **Class Activities** | Daily activity posts with photos |
| **Menus** | Daily meal plans per class |
| **Kitchen Samples** | Food sample tracking for safety compliance |
| **Health Checks** | Periodic health record per student |
| **Medical Incidents** | Log injuries or sickness events |
| **Timetable** | Weekly schedule per class |
| **Invoice** | Monthly tuition invoices |
| **Payment** | Payment confirmation and tracking |
| **Tuition Template** | Reusable fee templates |
| **School Years / Semesters** | Academic calendar management |
| **Campuses** | Multi-campus support |
| **Password Management** | Admin reset, self-change, OTP-based forgot password |
| **System Settings** | Configurable school-wide settings |
| **Daily Reports** | Report daily about event, menu, activity children |
| **Chat Histories** | AI to support children know what their child do based on report |


---

## 🛠️ Tech Stack

**Backend**
- ASP.NET Core 8 Web API
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
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) — Web Admin (React)
- [Node.js 18+](https://nodejs.org/) — Mobile App (React Native)
- Android Studio — Android Emulator for React Native

### Backend Setup

```bash
# 1. Clone the repository
git clone https://github.com/your-org/KMS.CapstoneProject.git
cd KMS.CapstoneProject

# 2. Create the database
# Run the SQL script in /database/KMS_Schema.sql on your SQL Server

# 3. Configure connection string
# Edit KMS.API/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=KMS_SP26SE095;Trusted_Connection=True;"
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

## 👥 Team

| Role | Responsibility |
|------|---------------|
| Backend Developer | .NET API, Database Design |
| Frontend Developer | ReactJS/NextJS Web Admin |
| Mobile Developer | React Native Teacher & Parent App |
| QA / Documentation | Testing, SRS, User Manual |


