using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.SystemSetting;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _settingRepository;

        public SystemSettingService(ISystemSettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<BaseResponseDTO> GetAllAsync()
        {
            try
            {
                var settings = await _settingRepository.GetAllAsync();
                var dtos = settings.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} settings",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetByKeyAsync(string key)
        {
            try
            {
                var setting = await _settingRepository.GetByKeyAsync(key);
                if (setting == null)
                    return new BaseResponseDTO { Success = false, Message = $"Setting '{key}' not found" };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Setting retrieved successfully",
                    Data = MapToDTO(setting)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> UpdateAsync(string key, UpdateSettingDTO dto, int updatedBy)
        {
            try
            {
                var setting = await _settingRepository.GetByKeyAsync(key);
                if (setting == null)
                    return new BaseResponseDTO { Success = false, Message = $"Setting '{key}' not found" };

                setting.SettingValue = dto.SettingValue;
                setting.UpdatedBy = updatedBy;
                setting.UpdatedAt = DateTime.Now;

                await _settingRepository.UpdateAsync(setting);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Setting '{key}' updated successfully",
                    Data = MapToDTO(setting)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> UpdateMultipleAsync(UpdateMultipleSettingsDTO dto, int updatedBy)
        {
            try
            {
                var updated = new List<string>();
                var notFound = new List<string>();

                foreach (var (key, value) in dto.Settings)
                {
                    var setting = await _settingRepository.GetByKeyAsync(key);
                    if (setting == null)
                    {
                        notFound.Add(key);
                        continue;
                    }

                    setting.SettingValue = value;
                    setting.UpdatedBy = updatedBy;
                    setting.UpdatedAt = DateTime.Now;

                    await _settingRepository.UpdateAsync(setting);
                    updated.Add(key);
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Updated {updated.Count} settings. Not found: {notFound.Count}.",
                    Data = new { Updated = updated, NotFound = notFound }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private SystemSettingDTO MapToDTO(KMS.Repository.Entities.KmsSystemSetting s) => new()
        {
            SettingId = s.SettingId,
            SettingKey = s.SettingKey,
            SettingValue = s.SettingValue,
            Description = s.Description,
            UpdatedByName = s.UpdatedByNavigation?.FullName,
            UpdatedAt = s.UpdatedAt
        };
    }
}
