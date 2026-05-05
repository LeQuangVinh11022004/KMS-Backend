using KMS.Service.DTOs.Evaluation;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IEvaluationCriteriaService
    {
        Task<BaseResponseDTO> GetAllAsync(bool activeOnly = true);
        Task<BaseResponseDTO> CreateAsync(CreateCriteriaRequest request);
        Task<BaseResponseDTO> ToggleActiveAsync(int criteriaId, bool isActive);
        Task<BaseResponseDTO> DeleteAsync(int criteriaId);
    }
}
