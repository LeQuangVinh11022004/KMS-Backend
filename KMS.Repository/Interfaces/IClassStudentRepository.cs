using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IClassStudentRepository
    {
        // Read
        Task<IEnumerable<KmsClassStudent>> GetStudentsByClassAsync(int classId);
        Task<IEnumerable<KmsClassStudent>> GetClassesByStudentAsync(int studentId);
        Task<KmsClassStudent?> GetAsync(int classId, int studentId);
        Task<bool> IsStudentInClassAsync(int classId, int studentId);
        Task<bool> IsStudentInAnyActiveClassAsync(int studentId);
        Task<int> CountStudentsInClassAsync(int classId);

        // Create
        Task<KmsClassStudent> EnrollAsync(KmsClassStudent classStudent);

        // Delete
        Task RemoveAsync(int classId, int studentId);
        Task RemoveAllStudentsFromClassAsync(int classId);

        Task<bool> IsStudentInAnyClassAsync(int studentId);
    }
}
