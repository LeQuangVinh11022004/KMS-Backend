using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.SystemSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface ISystemSettingService
    {
        Task<BaseResponseDTO> GetAllAsync();
        Task<BaseResponseDTO> GetByKeyAsync(string key);
        Task<BaseResponseDTO> UpdateAsync(string key, UpdateSettingDTO dto, int updatedBy);
        Task<BaseResponseDTO> UpdateMultipleAsync(UpdateMultipleSettingsDTO dto, int updatedBy);
    }
}
