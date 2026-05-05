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
    public class EvaluationCriteriaService : IEvaluationCriteriaService
    {
        private readonly IEvaluationCriteriaRepository _criteriaRepo;

        private static readonly HashSet<string> ValidCategories =
            new() { "academic", "nutrition", "hygiene", "behavior", "social" };
        private static readonly HashSet<string> ValidRatingTypes =
            new() { "scale", "label" };

        public EvaluationCriteriaService(IEvaluationCriteriaRepository criteriaRepo)
        {
            _criteriaRepo = criteriaRepo;
        }

        public async Task<BaseResponseDTO> GetAllAsync(bool activeOnly = true)
        {
            try
            {
                var entities = await _criteriaRepo.GetAllAsync(activeOnly);
                var dtos = entities.Select(MapToCriteriaDto).ToList();

                var grouped = dtos
                    .GroupBy(c => c.Category)
                    .Select(g => new
                    {
                        category = g.Key,
                        categoryLabel = CategoryLabelMap.GetLabel(g.Key),
                        items = g.ToList()
                    }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} criteria",
                    Data = grouped
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> CreateAsync(CreateCriteriaRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return new BaseResponseDTO { Success = false, Message = "Tên tiêu chí không được để trống" };

                if (!ValidCategories.Contains(request.Category))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Category không hợp lệ. Giá trị hợp lệ: {string.Join(", ", ValidCategories)}"
                    };

                if (!ValidRatingTypes.Contains(request.RatingType))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "RatingType không hợp lệ. Giá trị hợp lệ: scale, label"
                    };

                var entity = new KmsEvaluationCriteria
                {
                    Category = request.Category,
                    Name = request.Name,
                    Description = request.Description,
                    RatingType = request.RatingType,
                    SortOrder = request.SortOrder,
                    IsDefault = false,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var created = await _criteriaRepo.AddAsync(entity);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Tạo tiêu chí thành công",
                    Data = MapToCriteriaDto(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> ToggleActiveAsync(int criteriaId, bool isActive)
        {
            try
            {
                var entity = await _criteriaRepo.GetByIdAsync(criteriaId);
                if (entity == null)
                    return new BaseResponseDTO { Success = false, Message = "Không tìm thấy tiêu chí" };

                entity.IsActive = isActive;
                await _criteriaRepo.UpdateAsync(entity);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = isActive ? "Đã bật tiêu chí" : "Đã tắt tiêu chí",
                    Data = MapToCriteriaDto(entity)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> DeleteAsync(int criteriaId)
        {
            try
            {
                var entity = await _criteriaRepo.GetByIdAsync(criteriaId);
                if (entity == null)
                    return new BaseResponseDTO { Success = false, Message = "Không tìm thấy tiêu chí" };

                if (entity.IsDefault)
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Không thể xóa tiêu chí mặc định của hệ thống. Chỉ có thể tắt (isActive = false)"
                    };

                await _criteriaRepo.DeleteAsync(entity);
                return new BaseResponseDTO { Success = true, Message = "Đã xóa tiêu chí" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private static EvaluationCriteriaDto MapToCriteriaDto(KmsEvaluationCriteria e) => new()
        {
            CriteriaId = e.CriteriaId,
            Category = e.Category,
            CategoryLabel = CategoryLabelMap.GetLabel(e.Category),
            Name = e.Name,
            Description = e.Description,
            RatingType = e.RatingType,
            SortOrder = e.SortOrder,
            IsDefault = e.IsDefault,
            IsActive = e.IsActive
        };
    }
}
