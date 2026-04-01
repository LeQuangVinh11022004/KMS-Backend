using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMS.Service.DTOs.ActivityMedia;

namespace KMS.Service.Interfaces
{
    public interface IActivityMediaService
    {
        Task<List<ActivityMediaDTO>> GetByActivityIdAsync(int activityId, string baseUrl);

        Task<ActivityMediaDTO> AddAsync(int activityId, ActivityMediaCreateDTO dto, string webRootPath, string baseUrl);
        Task DeleteAsync(int mediaId);
    }
}
