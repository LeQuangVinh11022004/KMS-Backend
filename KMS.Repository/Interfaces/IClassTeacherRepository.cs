using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IClassTeacherRepository
    {
        // Read
        Task<IEnumerable<KmsClassTeacher>> GetTeachersByClassAsync(int classId);
        Task<IEnumerable<KmsClassTeacher>> GetClassesByTeacherAsync(int teacherId);
        Task<KmsClassTeacher?> GetAsync(int classId, int teacherId);
        Task<bool> IsTeacherInClassAsync(int classId, int teacherId);
        Task<int> CountTeachersInClassAsync(int classId);

        // Create
        Task<KmsClassTeacher> AssignAsync(KmsClassTeacher classTeacher);

        // Delete
        Task RemoveAsync(int classId, int teacherId);
        Task RemoveAllTeachersFromClassAsync(int classId);
    }
}
