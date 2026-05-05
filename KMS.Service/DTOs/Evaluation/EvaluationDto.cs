using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Evaluation
{

    public enum PeriodType { weekly, monthly, semester }

    public enum CheckupResult { Normal, NeedsAttention, Referred }

    public enum EvaluationCategory { academic, nutrition, hygiene, behavior, social }

    public enum RatingType { scale, label }

    public class EvaluationCriteriaDto
    {
        public int CriteriaId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string CategoryLabel { get; set; } = string.Empty; // "Học tập", "Ăn uống",...
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string RatingType { get; set; } = string.Empty;    // "scale" | "label"
        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCriteriaRequest
    {
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string RatingType { get; set; } = "scale";
        public int SortOrder { get; set; }
    }

    public class ToggleCriteriaRequest
    {
        public bool IsActive { get; set; }
    }

    public class EvaluationDetailRequest
    {
        public int CriteriaId { get; set; }
        public short? Score { get; set; }           // 1-10, dùng khi rating_type = scale
        public string? RatingLabel { get; set; }    // Tốt/Đạt/Cần cố gắng, dùng khi rating_type = label
        public string? Comment { get; set; }
    }

    public class CreateEvaluationRequest
    {
        public int ClassId { get; set; }
        public string PeriodType { get; set; } = "monthly";
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public bool? IsGoodStudent { get; set; }
        public string? GeneralComment { get; set; }
        public List<EvaluationDetailRequest> Details { get; set; } = new();
    }

    public class UpdateEvaluationRequest
    {
        public bool? IsGoodStudent { get; set; }
        public string? GeneralComment { get; set; }
        public List<EvaluationDetailRequest> Details { get; set; } = new();
    }

    public class EvaluationDetailDto
    {
        public int DetailId { get; set; }
        public int CriteriaId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string CategoryLabel { get; set; } = string.Empty;
        public string RatingType { get; set; } = string.Empty;
        public short? Score { get; set; }
        public string? RatingLabel { get; set; }
        public string? Comment { get; set; }
    }

    public class EvaluationCategoryGroupDto
    {
        public string Category { get; set; } = string.Empty;
        public string CategoryLabel { get; set; } = string.Empty;
        public List<EvaluationDetailDto> Details { get; set; } = new();
        public double? AverageScore { get; set; }   // trung bình các tiêu chí scale trong nhóm
    }

    public class EvaluationDto
    {
        public int EvaluationId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? StudentPhoto { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string PeriodType { get; set; } = string.Empty;
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public bool? IsGoodStudent { get; set; }
        public string? GeneralComment { get; set; }
        public string? EvaluatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double? OverallAverageScore { get; set; }
        public List<EvaluationCategoryGroupDto> CategoryGroups { get; set; } = new();
    }

    public class StudentEvaluationSummaryItem
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public int? EvaluationId { get; set; }
        public bool? IsGoodStudent { get; set; }
        public double? AverageScore { get; set; }
        public bool IsEvaluated => EvaluationId.HasValue;
    }

    public class ClassEvaluationSummaryDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string PeriodType { get; set; } = string.Empty;
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public int TotalStudents { get; set; }
        public int EvaluatedCount { get; set; }
        public int GoodStudentCount { get; set; }
        public int NotEvaluatedCount => TotalStudents - EvaluatedCount;
        public List<StudentEvaluationSummaryItem> Students { get; set; } = new();
    }

    public static class CategoryLabelMap
    {
        private static readonly Dictionary<string, string> _map = new()
        {
            { "academic",  "Học tập" },
            { "nutrition", "Ăn uống" },
            { "hygiene",   "Vệ sinh" },
            { "behavior",  "Hành vi / Thái độ" },
            { "social",    "Kỹ năng xã hội" }
        };

        public static string GetLabel(string category)
            => _map.TryGetValue(category, out var label) ? label : category;
    }
}
