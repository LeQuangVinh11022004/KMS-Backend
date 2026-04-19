using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface ITimetableRepository
    {
        Task<IEnumerable<KmsTimetable>> GetByClassIdAsync(int classId);
        Task<IEnumerable<KmsTimetable>> GetByTeacherIdAsync(int teacherId);
        Task<KmsTimetable?> GetByIdAsync(int id);
        Task CreateAsync(KmsTimetable timetable);
        Task UpdateAsync(KmsTimetable timetable);
        Task DeleteAsync(KmsTimetable timetable);
    }
}
