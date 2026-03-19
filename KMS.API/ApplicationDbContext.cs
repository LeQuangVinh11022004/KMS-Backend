using System;
using System.Collections.Generic;
using KMS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace KMS.API;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<KmsActivityMedium> KmsActivityMedia { get; set; }

    public virtual DbSet<KmsAnnouncement> KmsAnnouncements { get; set; }

    public virtual DbSet<KmsAttendance> KmsAttendances { get; set; }

    public virtual DbSet<KmsCampus> KmsCampuses { get; set; }

    public virtual DbSet<KmsClass> KmsClasses { get; set; }

    public virtual DbSet<KmsClassActivity> KmsClassActivities { get; set; }

    public virtual DbSet<KmsClassStudent> KmsClassStudents { get; set; }

    public virtual DbSet<KmsClassTeacher> KmsClassTeachers { get; set; }

    public virtual DbSet<KmsDailyReport> KmsDailyReports { get; set; }

    public virtual DbSet<KmsHealthCheck> KmsHealthChecks { get; set; }

    public virtual DbSet<KmsInvoice> KmsInvoices { get; set; }

    public virtual DbSet<KmsInvoiceItem> KmsInvoiceItems { get; set; }

    public virtual DbSet<KmsMedicalIncident> KmsMedicalIncidents { get; set; }

    public virtual DbSet<KmsMenu> KmsMenus { get; set; }

    public virtual DbSet<KmsNotification> KmsNotifications { get; set; }

    public virtual DbSet<KmsParent> KmsParents { get; set; }

    public virtual DbSet<KmsParentRegistration> KmsParentRegistrations { get; set; }

    public virtual DbSet<KmsPasswordResetToken> KmsPasswordResetTokens { get; set; }

    public virtual DbSet<KmsPayment> KmsPayments { get; set; }

    public virtual DbSet<KmsRole> KmsRoles { get; set; }

    public virtual DbSet<KmsSchoolYear> KmsSchoolYears { get; set; }

    public virtual DbSet<KmsSemester> KmsSemesters { get; set; }

    public virtual DbSet<KmsStudent> KmsStudents { get; set; }

    public virtual DbSet<KmsStudentParent> KmsStudentParents { get; set; }

    public virtual DbSet<KmsSystemSetting> KmsSystemSettings { get; set; }

    public virtual DbSet<KmsTeacher> KmsTeachers { get; set; }

    public virtual DbSet<KmsTimetable> KmsTimetables { get; set; }

    public virtual DbSet<KmsTuitionItem> KmsTuitionItems { get; set; }

    public virtual DbSet<KmsTuitionTemplate> KmsTuitionTemplates { get; set; }

    public virtual DbSet<KmsUser> KmsUsers { get; set; }

    public virtual DbSet<KmsUserRole> KmsUserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=KMS_SP26SE095;User Id=sa;Password=12345;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KmsActivityMedium>(entity =>
        {
<<<<<<< HEAD:KMS.API/ApplicationDbContext.cs
            entity.HasKey(e => e.MediaId).HasName("PK__KMS_Acti__B2C2B5CF0C4FF887");
=======
            entity.HasKey(e => e.MediaId).HasName("PK__KMS_Acti__B2C2B5CFF47E6433");
>>>>>>> origin/main:KMS.Repository/Data/ApplicationDbContext.cs

            entity.ToTable("KMS_ActivityMedia");

            entity.Property(e => e.Caption).HasMaxLength(255);
            entity.Property(e => e.MediaType)
                .HasMaxLength(20)
                .HasDefaultValue("Photo");
<<<<<<< HEAD:KMS.API/ApplicationDbContext.cs
            entity.Property(e => e.MediaUrl).HasMaxLength(500);
=======
            entity.Property(e => e.MediaUrl)
                .IsRequired()
                .HasMaxLength(500);
>>>>>>> origin/main:KMS.Repository/Data/ApplicationDbContext.cs
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Activity).WithMany(p => p.KmsActivityMedia)
                .HasForeignKey(d => d.ActivityId)
<<<<<<< HEAD:KMS.API/ApplicationDbContext.cs
                .HasConstraintName("FK__KMS_Activ__Activ__56B3DD81");
=======
                .HasConstraintName("FK__KMS_Activ__Activ__7755B73D");
>>>>>>> origin/main:KMS.Repository/Data/ApplicationDbContext.cs
        });

        modelBuilder.Entity<KmsAnnouncement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId).HasName("PK__KMS_Anno__9DE4457485C4816E");

            entity.ToTable("KMS_Announcements");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasDefaultValue("Normal");
            entity.Property(e => e.PublishedAt).HasColumnType("datetime");
            entity.Property(e => e.TargetAudience).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsAnnouncementCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Annou__Creat__46B27FE2");

            entity.HasOne(d => d.TargetClass).WithMany(p => p.KmsAnnouncements)
                .HasForeignKey(d => d.TargetClassId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__KMS_Annou__Targe__45BE5BA9");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.KmsAnnouncementUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__KMS_Annou__Updat__47A6A41B");
        });

        modelBuilder.Entity<KmsAttendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__KMS_Atte__8B69261C36F38E6E");

            entity.ToTable("KMS_Attendance");

            entity.HasIndex(e => new { e.ClassId, e.AttendanceDate }, "IX_Attendance_ClassId_Date");

            entity.HasIndex(e => e.AttendanceDate, "IX_Attendance_Date");

            entity.HasIndex(e => new { e.StudentId, e.AttendanceDate }, "IX_Attendance_StudentId_Date");

            entity.HasIndex(e => new { e.StudentId, e.AttendanceDate }, "UQ_Student_Date").IsUnique();

            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TakenAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.KmsAttendances)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Atten__Class__02FC7413");

            entity.HasOne(d => d.Student).WithMany(p => p.KmsAttendances)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__KMS_Atten__Stude__02084FDA");

            entity.HasOne(d => d.TakenByNavigation).WithMany(p => p.KmsAttendanceTakenByNavigations)
                .HasForeignKey(d => d.TakenBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Atten__Taken__03F0984C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.KmsAttendanceUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
<<<<<<< HEAD:KMS.API/ApplicationDbContext.cs
                .HasConstraintName("FK__KMS_Atten__Updat__17F790F9");
=======
                .HasConstraintName("FK__KMS_Atten__Updat__04E4BC85");
>>>>>>> origin/main:KMS.Repository/Data/ApplicationDbContext.cs
        });

        modelBuilder.Entity<KmsCampus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__KMS_Camp__FD598DD6507322D1");

            entity.ToTable("KMS_Campuses");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CampusName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<KmsClass>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__KMS_Clas__CB1927C02DD732CB");

            entity.ToTable("KMS_Classes");

            entity.HasIndex(e => e.IsActive, "IX_Classes_IsActive");

            entity.HasIndex(e => e.SchoolYearId, "IX_Classes_SchoolYearId");

            entity.Property(e => e.AgeGroup).HasMaxLength(20);
            entity.Property(e => e.ClassName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentEnrollment).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxCapacity).HasDefaultValue(20);
            entity.Property(e => e.Room).HasMaxLength(50);

            entity.HasOne(d => d.Campus).WithMany(p => p.KmsClasses)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK__KMS_Class__Campu__5CA1C101");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsClasses)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__KMS_Class__Creat__6EF57B66");

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.KmsClasses)
                .HasForeignKey(d => d.SchoolYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Class__Schoo__6E01572D");
        });

        modelBuilder.Entity<KmsClassActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__KMS_Clas__45F4A79195D4BA14");

            entity.ToTable("KMS_ClassActivities");

            entity.HasIndex(e => new { e.ClassId, e.ActivityDate }, "IX_Activities_ClassId_Date");

            entity.Property(e => e.ActivityDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.KmsClassActivities)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__KMS_Class__Class__19DFD96B");

            entity.HasOne(d => d.PostedByNavigation).WithMany(p => p.KmsClassActivityPostedByNavigations)
                .HasForeignKey(d => d.PostedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Class__Poste__1AD3FDA4");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.KmsClassActivityUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__KMS_Class__Updat__1BC821DD");
        });

        modelBuilder.Entity<KmsClassStudent>(entity =>
        {
            entity.HasKey(e => new { e.ClassId, e.StudentId }).HasName("PK__KMS_Clas__483575797B0B5F7D");

            entity.ToTable("KMS_ClassStudents");

            entity.Property(e => e.EnrolledAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.KmsClassStudents)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__KMS_Class__Class__73BA3083");

            entity.HasOne(d => d.EnrolledByNavigation).WithMany(p => p.KmsClassStudents)
                .HasForeignKey(d => d.EnrolledBy)
                .HasConstraintName("FK__KMS_Class__Enrol__75A278F5");

            entity.HasOne(d => d.Student).WithMany(p => p.KmsClassStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__KMS_Class__Stude__74AE54BC");
        });

        modelBuilder.Entity<KmsClassTeacher>(entity =>
        {
            entity.HasKey(e => new { e.ClassId, e.TeacherId }).HasName("PK__KMS_Clas__95C602566DFDB495");

            entity.ToTable("KMS_ClassTeachers");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Main");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.KmsClassTeachers)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK__KMS_Class__Assig__7C4F7684");

            entity.HasOne(d => d.Class).WithMany(p => p.KmsClassTeachers)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__KMS_Class__Class__7A672E12");

            entity.HasOne(d => d.Teacher).WithMany(p => p.KmsClassTeachers)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__KMS_Class__Teach__7B5B524B");
        });

        modelBuilder.Entity<KmsDailyReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__KMS_Dail__D5BD4805C55F0C0B");

            entity.ToTable("KMS_DailyReports");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Draft");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.KmsDailyReports)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Daily__Class__7EF6D905");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsDailyReportCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Daily__Creat__7FEAFD3E");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.KmsDailyReportReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK__KMS_Daily__Revie__00DF2177");

            entity.HasOne(d => d.Student).WithMany(p => p.KmsDailyReports)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__KMS_Daily__Stude__7E02B4CC");
        });

        modelBuilder.Entity<KmsDailyReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__KMS_Dail__D5BD4805C828FC60");

            entity.ToTable("KMS_DailyReports");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Draft");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.KmsDailyReports)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Daily__Class__5E54FF49");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsDailyReportCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Daily__Creat__5F492382");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.KmsDailyReportReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK__KMS_Daily__Revie__603D47BB");

            entity.HasOne(d => d.Student).WithMany(p => p.KmsDailyReports)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__KMS_Daily__Stude__5D60DB10");
        });

        modelBuilder.Entity<KmsHealthCheck>(entity =>
        {
            entity.HasKey(e => e.CheckId).HasName("PK__KMS_Heal__86815766FCDDC013");

            entity.ToTable("KMS_HealthChecks");

            entity.Property(e => e.CheckDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DentalStatus).HasMaxLength(100);
            entity.Property(e => e.EyeSight).HasMaxLength(50);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Student).WithMany(p => p.KmsHealthChecks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Healt__Stude__634EBE90");
        });

        modelBuilder.Entity<KmsInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__KMS_Invo__D796AAB598E2786D");

            entity.ToTable("KMS_Invoices");

            entity.HasIndex(e => e.DueDate, "IX_Invoices_DueDate");

            entity.HasIndex(e => e.MonthYear, "IX_Invoices_MonthYear");

            entity.HasIndex(e => e.Status, "IX_Invoices_Status");

            entity.HasIndex(e => e.StudentId, "IX_Invoices_StudentId");

            entity.HasIndex(e => e.InvoiceNumber, "UQ__KMS_Invo__D776E981BECC61B1").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Unpaid");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsInvoiceCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__KMS_Invoi__Creat__32AB8735");

            entity.HasOne(d => d.Student).WithMany(p => p.KmsInvoices)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__KMS_Invoi__Stude__30C33EC3");

            entity.HasOne(d => d.Template).WithMany(p => p.KmsInvoices)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("FK__KMS_Invoi__Templ__31B762FC");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.KmsInvoiceUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__KMS_Invoi__Updat__339FAB6E");
        });

        modelBuilder.Entity<KmsInvoiceItem>(entity =>
        {
            entity.HasKey(e => e.InvoiceItemId).HasName("PK__KMS_Invo__478FE09C1A8E2C30");

            entity.ToTable("KMS_InvoiceItems");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ItemName).HasMaxLength(100);

            entity.HasOne(d => d.Invoice).WithMany(p => p.KmsInvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
<<<<<<< HEAD:KMS.API/ApplicationDbContext.cs
                .HasConstraintName("FK__KMS_Invoi__Invoi__4B7734FF");
=======
                .HasConstraintName("FK__KMS_Invoi__Invoi__3864608B");
>>>>>>> origin/main:KMS.Repository/Data/ApplicationDbContext.cs
        });

        modelBuilder.Entity<KmsMedicalIncident>(entity =>
        {
            entity.HasKey(e => e.IncidentId).HasName("PK__KMS_Medi__3D8053B2D324F7B0");

            entity.ToTable("KMS_MedicalIncidents");

            entity.Property(e => e.IncidentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Student).WithMany(p => p.KmsMedicalIncidents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Medic__Stude__671F4F74");
        });

        modelBuilder.Entity<KmsMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__KMS_Menu__C99ED2307C246F82");

            entity.ToTable("KMS_Menus");

            entity.Property(e => e.Allergens).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MealType).HasMaxLength(20);
            entity.Property(e => e.MenuContent).HasMaxLength(1000);
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .HasDefaultValue("In-House");
            entity.Property(e => e.SupplierName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.KmsMenus)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__KMS_Menus__Class__123EB7A3");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsMenuCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__KMS_Menus__Creat__1332DBDC");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.KmsMenuUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__KMS_Menus__Updat__14270015");
        });

        modelBuilder.Entity<KmsNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__KMS_Noti__20CF2E12FBB5E45F");

            entity.ToTable("KMS_Notifications");

            entity.HasIndex(e => e.CreatedAt, "IX_Notifications_CreatedAt");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "IX_Notifications_UserId_IsRead");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.ReadAt).HasColumnType("datetime");
            entity.Property(e => e.RelatedEntityType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.KmsNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__KMS_Notif__UserI__4E53A1AA");
        });

        modelBuilder.Entity<KmsParent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__KMS_Pare__D339516F8EE6BEBB");

            entity.ToTable("KMS_Parents");

            entity.HasIndex(e => e.UserId, "UQ__KMS_Pare__1788CC4D8F2FE79A").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmergencyContact).HasMaxLength(20);
            entity.Property(e => e.Occupation).HasMaxLength(100);
            entity.Property(e => e.WorkAddress).HasMaxLength(255);

            entity.HasOne(d => d.User).WithOne(p => p.KmsParent)
                .HasForeignKey<KmsParent>(d => d.UserId)
                .HasConstraintName("FK__KMS_Paren__UserI__59FA5E80");
        });

        modelBuilder.Entity<KmsParentRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__KMS_Pare__6EF588101216F0C3");

            entity.ToTable("KMS_ParentRegistrations");

            entity.Property(e => e.ChildFullName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RequestMessage).HasMaxLength(500);
            entity.Property(e => e.ReviewNote).HasMaxLength(500);
            entity.Property(e => e.ReviewedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.KmsParentRegistrations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK__KMS_Paren__Revie__4AB81AF0");
        });

        modelBuilder.Entity<KmsPasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__KMS_Pass__658FEEEA4AF18B15");

            entity.ToTable("KMS_PasswordResetTokens");

            entity.HasIndex(e => e.Token, "UQ__KMS_Pass__1EB4F817314615C3").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.KmsPasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__KMS_Passw__UserI__45F365D3");
        });

        modelBuilder.Entity<KmsPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__KMS_Paym__9B556A38C479369C");

            entity.ToTable("KMS_Payments");

            entity.HasIndex(e => e.InvoiceId, "IX_Payments_InvoiceId");

            entity.HasIndex(e => e.PaymentDate, "IX_Payments_PaymentDate");

            entity.HasIndex(e => e.PaymentNumber, "UQ__KMS_Paym__E2C1723BC38BCAC6").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentNumber).HasMaxLength(50);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);

            entity.HasOne(d => d.Invoice).WithMany(p => p.KmsPayments)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__KMS_Payme__Invoi__3D2915A8");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.KmsPayments)
                .HasForeignKey(d => d.ReceivedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Payme__Recei__3E1D39E1");
        });

        modelBuilder.Entity<KmsRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__KMS_Role__8AFACE1A0A4AA1F5");

            entity.ToTable("KMS_Roles");

            entity.HasIndex(e => e.RoleName, "UQ__KMS_Role__8A2B61605571C66E").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<KmsSchoolYear>(entity =>
        {
            entity.HasKey(e => e.SchoolYearId).HasName("PK__KMS_Scho__9BAABCE0C55E5898");

            entity.ToTable("KMS_SchoolYears");

            entity.HasIndex(e => e.YearName, "UQ__KMS_Scho__294C4DA91A99DFA6").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.YearName).HasMaxLength(20);
        });

        modelBuilder.Entity<KmsSemester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__KMS_Seme__043301DD4DFBA7D1");

            entity.ToTable("KMS_Semesters");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SemesterName).HasMaxLength(50);

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.KmsSemesters)
                .HasForeignKey(d => d.SchoolYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_Semes__Schoo__2B3F6F97");
        });

        modelBuilder.Entity<KmsStudent>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__KMS_Stud__32C52B990201E131");

            entity.ToTable("KMS_Students");

            entity.HasIndex(e => e.EnrollmentDate, "IX_Students_EnrollmentDate");

            entity.HasIndex(e => e.IsActive, "IX_Students_IsActive");

            entity.HasIndex(e => e.StudentCode, "IX_Students_StudentCode");

            entity.HasIndex(e => e.StudentCode, "UQ__KMS_Stud__1FC886048D1FED82").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Allergies).HasMaxLength(500);
            entity.Property(e => e.BloodType).HasMaxLength(5);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MedicalNotes).HasMaxLength(1000);
            entity.Property(e => e.Photo).HasMaxLength(255);
            entity.Property(e => e.StudentCode).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Campus).WithMany(p => p.KmsStudents)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK__KMS_Stude__Campu__5D95E53A");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsStudentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__KMS_Stude__Creat__534D60F1");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.KmsStudentUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__KMS_Stude__Updat__5441852A");
        });

        modelBuilder.Entity<KmsStudentParent>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ParentId }).HasName("PK__KMS_Stud__DFF6BE8F0311BD6B");

            entity.ToTable("KMS_StudentParents");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPrimaryContact).HasDefaultValue(false);
            entity.Property(e => e.Relationship).HasMaxLength(20);

            entity.HasOne(d => d.Parent).WithMany(p => p.KmsStudentParents)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__KMS_Stude__Paren__5FB337D6");

            entity.HasOne(d => d.Student).WithMany(p => p.KmsStudentParents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__KMS_Stude__Stude__5EBF139D");
        });

        modelBuilder.Entity<KmsSystemSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__KMS_Syst__54372B1D8F77DC3C");

            entity.ToTable("KMS_SystemSettings");

            entity.HasIndex(e => e.SettingKey, "UQ__KMS_Syst__01E719ADD98C8470").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.SettingKey).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.KmsSystemSettings)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__KMS_Syste__Updat__57DD0BE4");
        });

        modelBuilder.Entity<KmsTeacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__KMS_Teac__EDF25964F7716345");

            entity.ToTable("KMS_Teachers");

            entity.HasIndex(e => e.UserId, "UQ__KMS_Teac__1788CC4D3823672E").IsUnique();

            entity.HasIndex(e => e.TeacherCode, "UQ__KMS_Teac__9B8A194EA3F47B70").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Specialization).HasMaxLength(100);
            entity.Property(e => e.StaffType)
                .HasMaxLength(50)
                .HasDefaultValue("Teacher");
            entity.Property(e => e.TeacherCode).HasMaxLength(20);

            entity.HasOne(d => d.Campus).WithMany(p => p.KmsTeachers)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK__KMS_Teach__Campu__5E8A0973");

            entity.HasOne(d => d.User).WithOne(p => p.KmsTeacher)
                .HasForeignKey<KmsTeacher>(d => d.UserId)
                .HasConstraintName("FK__KMS_Teach__UserI__6754599E");
        });

        modelBuilder.Entity<KmsTimetable>(entity =>
        {
            entity.HasKey(e => e.TimetableId).HasName("PK__KMS_Time__68413F606A5FCBEC");

            entity.ToTable("KMS_Timetables");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Room).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(100);

            entity.HasOne(d => d.Class).WithMany(p => p.KmsTimetables)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__KMS_Timet__Class__0A9D95DB");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsTimetables)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__KMS_Timet__Creat__0C85DE4D");

            entity.HasOne(d => d.Teacher).WithMany(p => p.KmsTimetables)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__KMS_Timet__Teach__0B91BA14");
        });

        modelBuilder.Entity<KmsTuitionItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__KMS_Tuit__727E838BC22AFC4C");

            entity.ToTable("KMS_TuitionItems");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ItemName).HasMaxLength(100);
            entity.Property(e => e.ItemOrder).HasDefaultValue(1);

            entity.HasOne(d => d.Template).WithMany(p => p.KmsTuitionItems)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("FK__KMS_Tuiti__Templ__29221CFB");
        });

        modelBuilder.Entity<KmsTuitionTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__KMS_Tuit__F87ADD2785F54F16");

            entity.ToTable("KMS_TuitionTemplates");

            entity.Property(e => e.BaseAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TemplateName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.KmsTuitionTemplates)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__KMS_Tuiti__Creat__245D67DE");
        });

        modelBuilder.Entity<KmsUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__KMS_User__1788CC4C11DAEF1E");

            entity.ToTable("KMS_Users");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.IsActive, "IX_Users_IsActive");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Username, "UQ__KMS_User__536C85E4802D3EB7").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__KMS_User__A9D10534C96ABFB5").IsUnique();

            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);
            entity.Property(e => e.IsPhoneVerified).HasDefaultValue(false);
            entity.Property(e => e.LastLoginAt).HasColumnType("datetime");
            entity.Property(e => e.PasswordChangedAt).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Campus).WithMany(p => p.KmsUsers)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK__KMS_Users__Campu__5F7E2DAC");
        });

        modelBuilder.Entity<KmsUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__KMS_User__AF2760AD25F393C5");

            entity.ToTable("KMS_UserRoles");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.KmsUserRoleAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK__KMS_UserR__Assig__403A8C7D");

            entity.HasOne(d => d.Role).WithMany(p => p.KmsUserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KMS_UserR__RoleI__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.KmsUserRoleUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__KMS_UserR__UserI__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
