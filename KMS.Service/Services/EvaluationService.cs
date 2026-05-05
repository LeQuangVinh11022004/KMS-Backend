using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Evaluation;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IEvaluationRepository _evalRepo;
        private readonly IEvaluationCriteriaRepository _criteriaRepo;

        private static readonly HashSet<string> ValidPeriodTypes =
            new() { "weekly", "monthly", "semester" };
        private static readonly HashSet<string> ValidRatingLabels =
            new() { "Tốt", "Đạt", "Cần cố gắng" };

        public EvaluationService(
            IEvaluationRepository evalRepo,
            IEvaluationCriteriaRepository criteriaRepo)
        {
            _evalRepo = evalRepo;
            _criteriaRepo = criteriaRepo;
        }

        public async Task<BaseResponseDTO> GetByIdAsync(int evaluationId)
        {
            try
            {
                var entity = await _evalRepo.GetByIdAsync(evaluationId);
                if (entity == null)
                    return new BaseResponseDTO { Success = false, Message = "Không tìm thấy phiếu đánh giá" };

                return new BaseResponseDTO { Success = true, Message = "Success", Data = MapToEvaluationDto(entity) };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetByStudentIdAsync(int studentId, string? periodType = null)
        {
            try
            {
                if (periodType != null && !ValidPeriodTypes.Contains(periodType))
                    return new BaseResponseDTO { Success = false, Message = "PeriodType không hợp lệ" };

                var entities = await _evalRepo.GetByStudentIdAsync(studentId, periodType);
                var dtos = entities.Select(MapToEvaluationSummary).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} evaluation(s)",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetClassSummaryAsync(
            int classId, string periodType, DateOnly periodStart)
        {
            try
            {
                if (!ValidPeriodTypes.Contains(periodType))
                    return new BaseResponseDTO { Success = false, Message = "PeriodType không hợp lệ" };

                var evaluations = await _evalRepo.GetByClassIdAsync(classId, periodType, periodStart);

                var summaryItems = evaluations.Select(e => new StudentEvaluationSummaryItem
                {
                    StudentId = e.StudentId,
                    FullName = e.Student?.FullName ?? "",
                    Photo = e.Student?.Photo,
                    EvaluationId = e.EvaluationId,
                    IsGoodStudent = e.IsGoodStudent,
                    AverageScore = CalcAverageScore(e.KmsEvaluationDetails.ToList())
                }).ToList();

                var summary = new ClassEvaluationSummaryDto
                {
                    ClassId = classId,
                    PeriodType = periodType,
                    PeriodStart = periodStart,
                    TotalStudents = summaryItems.Count,
                    EvaluatedCount = summaryItems.Count(s => s.IsEvaluated),
                    GoodStudentCount = summaryItems.Count(s => s.IsGoodStudent == true),
                    Students = summaryItems
                };

                return new BaseResponseDTO { Success = true, Message = "Success", Data = summary };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetGoodStudentsAsync(
            int classId, string periodType, DateOnly periodStart)
        {
            try
            {
                if (!ValidPeriodTypes.Contains(periodType))
                    return new BaseResponseDTO { Success = false, Message = "PeriodType không hợp lệ" };

                var evaluations = await _evalRepo.GetByClassIdAsync(classId, periodType, periodStart);

                var goodStudents = evaluations
                    .Where(e => e.IsGoodStudent == true)
                    .Select(e => new StudentEvaluationSummaryItem
                    {
                        StudentId = e.StudentId,
                        FullName = e.Student?.FullName ?? "",
                        Photo = e.Student?.Photo,
                        EvaluationId = e.EvaluationId,
                        IsGoodStudent = true,
                        AverageScore = CalcAverageScore(e.KmsEvaluationDetails.ToList())
                    })
                    .OrderBy(s => s.FullName)
                    .ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {goodStudents.Count} good student(s)",
                    Data = goodStudents
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> CreateAsync(
            int studentId, int evaluatedBy, CreateEvaluationRequest request)
        {
            try
            {
                if (!ValidPeriodTypes.Contains(request.PeriodType))
                    return new BaseResponseDTO { Success = false, Message = "PeriodType không hợp lệ" };

                if (request.PeriodEnd < request.PeriodStart)
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "PeriodEnd phải lớn hơn hoặc bằng PeriodStart"
                    };

                if (await _evalRepo.ExistsAsync(studentId, request.PeriodType, request.PeriodStart))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Học sinh này đã có phiếu đánh giá kỳ {request.PeriodType} bắt đầu {request.PeriodStart:yyyy-MM-dd}"
                    };

                var validateResult = await ValidateDetailsAsync(request.Details);
                if (!validateResult.Success) return validateResult;

                var evaluation = new KmsStudentEvaluation
                {
                    StudentId = studentId,
                    ClassId = request.ClassId,
                    PeriodType = request.PeriodType,
                    PeriodStart = request.PeriodStart,
                    PeriodEnd = request.PeriodEnd,
                    IsGoodStudent = request.IsGoodStudent,
                    GeneralComment = request.GeneralComment,
                    EvaluatedBy = evaluatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var created = await _evalRepo.AddAsync(evaluation);

                var details = request.Details.Select(d => new KmsEvaluationDetail
                {
                    EvaluationId = created.EvaluationId,
                    CriteriaId = d.CriteriaId,
                    Score = d.Score,
                    RatingLabel = d.RatingLabel,
                    Comment = d.Comment
                }).ToList();

                await _evalRepo.AddDetailsAsync(details);

                var full = await _evalRepo.GetByIdAsync(created.EvaluationId);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Tạo phiếu đánh giá thành công",
                    Data = MapToEvaluationDto(full!)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> UpdateAsync(int evaluationId, UpdateEvaluationRequest request)
        {
            try
            {
                var entity = await _evalRepo.GetByIdAsync(evaluationId);
                if (entity == null)
                    return new BaseResponseDTO { Success = false, Message = "Không tìm thấy phiếu đánh giá" };

                var validateResult = await ValidateDetailsAsync(request.Details);
                if (!validateResult.Success) return validateResult;

                entity.IsGoodStudent = request.IsGoodStudent;
                entity.GeneralComment = request.GeneralComment;
                entity.UpdatedAt = DateTime.Now;
                await _evalRepo.UpdateAsync(entity);

                await _evalRepo.DeleteDetailsByEvaluationIdAsync(evaluationId);

                var details = request.Details.Select(d => new KmsEvaluationDetail
                {
                    EvaluationId = evaluationId,
                    CriteriaId = d.CriteriaId,
                    Score = d.Score,
                    RatingLabel = d.RatingLabel,
                    Comment = d.Comment
                }).ToList();

                await _evalRepo.AddDetailsAsync(details);

                var full = await _evalRepo.GetByIdAsync(evaluationId);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Cập nhật phiếu đánh giá thành công",
                    Data = MapToEvaluationDto(full!)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ---- Private helpers ----

        private async Task<BaseResponseDTO> ValidateDetailsAsync(List<EvaluationDetailRequest> details)
        {
            if (details == null || details.Count == 0)
                return new BaseResponseDTO { Success = false, Message = "Phiếu đánh giá phải có ít nhất 1 tiêu chí" };

            var allCriteria = await _criteriaRepo.GetAllAsync(activeOnly: true);
            var criteriaMap = allCriteria.ToDictionary(c => c.CriteriaId);
            var errors = new List<string>();

            foreach (var d in details)
            {
                if (!criteriaMap.TryGetValue(d.CriteriaId, out var criteria))
                {
                    errors.Add($"Tiêu chí ID {d.CriteriaId} không tồn tại hoặc đã bị tắt");
                    continue;
                }

                if (criteria.RatingType == "scale")
                {
                    if (!d.Score.HasValue)
                        errors.Add($"Tiêu chí '{criteria.Name}' cần có điểm số (score)");
                    else if (d.Score < 1 || d.Score > 10)
                        errors.Add($"Tiêu chí '{criteria.Name}': điểm phải từ 1 đến 10, nhận được {d.Score}");
                }
                else if (criteria.RatingType == "label")
                {
                    if (string.IsNullOrWhiteSpace(d.RatingLabel))
                        errors.Add($"Tiêu chí '{criteria.Name}' cần có nhãn đánh giá (ratingLabel)");
                    else if (!ValidRatingLabels.Contains(d.RatingLabel))
                        errors.Add($"Tiêu chí '{criteria.Name}': nhãn không hợp lệ '{d.RatingLabel}'. Hợp lệ: Tốt, Đạt, Cần cố gắng");
                }
            }

            var duplicates = details.GroupBy(d => d.CriteriaId)
                .Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            if (duplicates.Any())
                errors.Add($"Có tiêu chí bị trùng: CriteriaId {string.Join(", ", duplicates)}");

            if (errors.Any())
                return new BaseResponseDTO { Success = false, Message = string.Join("\n", errors) };

            return new BaseResponseDTO { Success = true };
        }

        private static EvaluationDto MapToEvaluationDto(KmsStudentEvaluation e)
        {
            var details = e.KmsEvaluationDetails?.ToList() ?? new();

            var detailDtos = details.Select(d => new EvaluationDetailDto
            {
                DetailId = d.DetailId,
                CriteriaId = d.CriteriaId,
                CriteriaName = d.Criteria?.Name ?? "",
                Category = d.Criteria?.Category ?? "",
                CategoryLabel = CategoryLabelMap.GetLabel(d.Criteria?.Category ?? ""),
                RatingType = d.Criteria?.RatingType ?? "",
                Score = d.Score,
                RatingLabel = d.RatingLabel,
                Comment = d.Comment
            }).ToList();

            var categoryOrder = new[] { "academic", "nutrition", "hygiene", "behavior", "social" };
            var categoryGroups = detailDtos
                .GroupBy(d => d.Category)
                .OrderBy(g => Array.IndexOf(categoryOrder, g.Key))
                .Select(g => new EvaluationCategoryGroupDto
                {
                    Category = g.Key,
                    CategoryLabel = CategoryLabelMap.GetLabel(g.Key),
                    Details = g.ToList(),
                    AverageScore = g.Any(d => d.Score.HasValue)
                        ? Math.Round(g.Where(d => d.Score.HasValue).Average(d => (double)d.Score!.Value), 2)
                        : null
                }).ToList();

            return new EvaluationDto
            {
                EvaluationId = e.EvaluationId,
                StudentId = e.StudentId,
                StudentName = e.Student?.FullName ?? "",
                StudentPhoto = e.Student?.Photo,
                ClassId = e.ClassId,
                ClassName = e.Class?.ClassName ?? "",
                PeriodType = e.PeriodType,
                PeriodStart = e.PeriodStart,
                PeriodEnd = e.PeriodEnd,
                IsGoodStudent = e.IsGoodStudent,
                GeneralComment = e.GeneralComment,
                EvaluatedByName = e.EvaluatedByNavigation?.FullName,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                OverallAverageScore = CalcAverageScore(details),
                CategoryGroups = categoryGroups
            };
        }

        private static object MapToEvaluationSummary(KmsStudentEvaluation e) => new
        {
            e.EvaluationId,
            e.StudentId,
            StudentName = e.Student?.FullName ?? "",
            StudentPhoto = e.Student?.Photo,
            e.ClassId,
            ClassName = e.Class?.ClassName ?? "",
            e.PeriodType,
            e.PeriodStart,
            e.PeriodEnd,
            e.IsGoodStudent,
            e.GeneralComment,
            EvaluatedByName = e.EvaluatedByNavigation?.FullName,
            e.CreatedAt,
            e.UpdatedAt
        };

        private static double? CalcAverageScore(List<KmsEvaluationDetail> details)
        {
            var scores = details.Where(d => d.Score.HasValue).Select(d => (double)d.Score!.Value).ToList();
            return scores.Any() ? Math.Round(scores.Average(), 2) : null;
        }
    }
}
