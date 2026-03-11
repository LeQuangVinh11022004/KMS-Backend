using KMS.Repository.Interfaces;
using KMS.Service.DTOs.AuditLog;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<BaseResponseDTO> GetAllAsync()
        {
            try
            {
                var logs = await _auditLogRepository.GetAllAsync();
                var dtos = logs.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} audit logs",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetFilteredAsync(AuditLogFilterDTO filter)
        {
            try
            {
                var logs = await _auditLogRepository.GetFilteredAsync(
                    filter.UserId, filter.Action, filter.TableName,
                    filter.FromDate, filter.ToDate);

                var dtos = logs.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} logs",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private AuditLogDTO MapToDTO(KMS.Repository.Entities.KmsAuditLog l) => new()
        {
            LogId = l.LogId,
            UserId = l.UserId,
            UserName = l.User?.FullName,
            Action = l.Action,
            TableName = l.TableName,
            RecordId = l.RecordId,
            OldValue = l.OldValue,
            NewValue = l.NewValue,
            IPAddress = l.Ipaddress,
            UserAgent = l.UserAgent,
            CreatedAt = l.CreatedAt ?? DateTime.Now
        };
    }
}
