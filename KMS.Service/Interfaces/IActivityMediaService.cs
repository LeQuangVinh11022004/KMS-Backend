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
        Task<List<ActivityMediaDTO>> GetByActivityIdAsync(int activityId);

        Task AddAsync(int activityId, ActivityMediaDTO dto);

        Task DeleteAsync(int mediaId);
    }
}
