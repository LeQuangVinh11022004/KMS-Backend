using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsStudent
{
    public int StudentId { get; set; }

    public string? StudentCode { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string? Address { get; set; }

    public string? Photo { get; set; }

    public string? BloodType { get; set; }

    public string? Allergies { get; set; }

    public string? MedicalNotes { get; set; }

    public DateOnly? EnrollmentDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual KmsUser? CreatedByNavigation { get; set; }

    public virtual ICollection<KmsAttendance> KmsAttendances { get; set; } = new List<KmsAttendance>();

    public virtual ICollection<KmsClassStudent> KmsClassStudents { get; set; } = new List<KmsClassStudent>();

    public virtual ICollection<KmsInvoice> KmsInvoices { get; set; } = new List<KmsInvoice>();

    public virtual ICollection<KmsStudentParent> KmsStudentParents { get; set; } = new List<KmsStudentParent>();

    public virtual KmsUser? UpdatedByNavigation { get; set; }
}
