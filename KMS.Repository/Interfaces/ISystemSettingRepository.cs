using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface ISystemSettingRepository
    {
        Task<IEnumerable<KmsSystemSetting>> GetAllAsync();
        Task<KmsSystemSetting?> GetByKeyAsync(string key);
        Task<KmsSystemSetting?> GetByIdAsync(int settingId);
        Task UpdateAsync(KmsSystemSetting setting);
    }
}
