using KMS.Service.DTOs.DailyReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IDailyReportService
    {
        Task<List<DailyReportDTO>> GetAllAsync();

        Task<DailyReportDTO?> GetByIdAsync(int id);

        Task CreateAsync(CreateDailyReportDTO dto, int userId);

        Task UpdateAsync(int id, UpdateDailyReportDTO dto);

        Task DeleteAsync(int id);
    }
}
