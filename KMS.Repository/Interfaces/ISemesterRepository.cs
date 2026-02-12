using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface ISemesterRepository
    {
        Task<List<KmsSemester>> GetAllAsync();
        Task<KmsSemester?> GetByIdAsync(int id);
        Task AddAsync(KmsSemester entity);
        void Update(KmsSemester entity);
        void Delete(KmsSemester entity);
        Task SaveChangesAsync();
    }
}
