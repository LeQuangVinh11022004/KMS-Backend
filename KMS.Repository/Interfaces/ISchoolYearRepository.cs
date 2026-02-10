using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface ISchoolYearRepository
    {
        Task<List<KmsSchoolYear>> GetAllAsync();
        Task<KmsSchoolYear?> GetByIdAsync(int id);
        Task AddAsync(KmsSchoolYear entity);
        void Update(KmsSchoolYear entity);
        Task<bool> ExistsAsync(int id);
    }
}
