using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface ITeacherRepository
    {
        // CRUD
        Task<KmsTeacher?> GetByIdAsync(int teacherId);
        Task<KmsTeacher?> GetByUserIdAsync(int userId);
        Task<KmsTeacher?> GetByTeacherCodeAsync(string teacherCode);
        Task<IEnumerable<KmsTeacher>> GetAllAsync();
        Task<KmsTeacher> CreateAsync(KmsTeacher teacher);
        Task UpdateAsync(KmsTeacher teacher);
        Task DeleteAsync(int teacherId);

        // Filter
        Task<IEnumerable<KmsTeacher>> GetActiveTeachersAsync();
        Task<IEnumerable<KmsTeacher>> GetInactiveTeachersAsync();
        Task<IEnumerable<KmsTeacher>> SearchTeachersAsync(string keyword);

        // Check
        Task<bool> TeacherCodeExistsAsync(string teacherCode);
        Task<bool> UserIsTeacherAsync(int userId);

        // Statistics
        Task<int> CountActiveTeachersAsync();
    }
}
