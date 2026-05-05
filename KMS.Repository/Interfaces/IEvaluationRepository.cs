using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IEvaluationRepository
    {
        Task<KmsStudentEvaluation?> GetByIdAsync(int evaluationId);
        Task<List<KmsStudentEvaluation>> GetByStudentIdAsync(int studentId, string? periodType = null);
        Task<List<KmsStudentEvaluation>> GetByClassIdAsync(int classId, string periodType, DateOnly periodStart);
        Task<bool> ExistsAsync(int studentId, string periodType, DateOnly periodStart);
        Task<KmsStudentEvaluation> AddAsync(KmsStudentEvaluation entity);
        Task UpdateAsync(KmsStudentEvaluation entity);

        // Details
        Task AddDetailsAsync(List<KmsEvaluationDetail> details);
        Task DeleteDetailsByEvaluationIdAsync(int evaluationId);
    }
}
