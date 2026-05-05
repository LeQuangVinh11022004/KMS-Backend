using KMS.Service.DTOs.Evaluation;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IEvaluationService
    {
        Task<BaseResponseDTO> GetByIdAsync(int evaluationId);
        Task<BaseResponseDTO> GetByStudentIdAsync(int studentId, string? periodType = null);
        Task<BaseResponseDTO> GetClassSummaryAsync(int classId, string periodType, DateOnly periodStart);
        Task<BaseResponseDTO> GetGoodStudentsAsync(int classId, string periodType, DateOnly periodStart);
        Task<BaseResponseDTO> CreateAsync(int studentId, int evaluatedBy, CreateEvaluationRequest request);
        Task<BaseResponseDTO> UpdateAsync(int evaluationId, UpdateEvaluationRequest request);
    }
}
