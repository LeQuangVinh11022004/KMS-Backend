using KMS.Service.DTOs;
using KMS.Service.DTOs.TuitionTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface ITuitionTemplateService
    {
        Task<BaseResponseDTO> GetAllTemplatesAsync();
        Task<BaseResponseDTO> GetActiveTemplatesAsync();
        Task<BaseResponseDTO> GetTemplateByIdAsync(int templateId);
        Task<BaseResponseDTO> CreateTemplateAsync(CreateTuitionTemplateDTO dto);
        Task<BaseResponseDTO> UpdateTemplateAsync(int templateId, UpdateTuitionTemplateDTO dto);
        Task<BaseResponseDTO> DeleteTemplateAsync(int templateId);
        Task<BaseResponseDTO> ToggleTemplateStatusAsync(int templateId);
    }
}
