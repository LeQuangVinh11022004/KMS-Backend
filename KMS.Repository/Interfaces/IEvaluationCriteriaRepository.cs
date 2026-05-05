using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IEvaluationCriteriaRepository
    {
        Task<List<KmsEvaluationCriteria>> GetAllAsync(bool activeOnly = true);
        Task<KmsEvaluationCriteria?> GetByIdAsync(int criteriaId);
        Task<KmsEvaluationCriteria> AddAsync(KmsEvaluationCriteria entity);
        Task UpdateAsync(KmsEvaluationCriteria entity);
        Task DeleteAsync(KmsEvaluationCriteria entity);
    }
}
