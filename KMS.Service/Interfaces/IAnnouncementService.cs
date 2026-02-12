using KMS.Service.DTOs.Announcement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IAnnouncementService
    {
        Task<List<AnnouncementDTO>> GetAllAsync();
        Task<AnnouncementDTO?> GetByIdAsync(int id);
        Task CreateAsync(AnnouncementDTO dto, int userId);
        Task UpdateAsync(int id, AnnouncementDTO dto, int userId);
        Task DeleteAsync(int id);
    }
}
