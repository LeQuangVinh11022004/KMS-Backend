using KMS.Service.DTOs.ActivityPhoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IActivityPhotoService
    {
        Task<List<ActivityPhotoDTO>> GetByActivityIdAsync(int activityId);
        Task AddAsync(int activityId, ActivityPhotoDTO dto);
        Task DeleteAsync(int photoId);
    }
}
