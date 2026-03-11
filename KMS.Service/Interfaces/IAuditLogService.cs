using KMS.Service.DTOs.AuditLog;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IAuditLogService
    {
        Task<BaseResponseDTO> GetAllAsync();
        Task<BaseResponseDTO> GetFilteredAsync(AuditLogFilterDTO filter);
    }
}
