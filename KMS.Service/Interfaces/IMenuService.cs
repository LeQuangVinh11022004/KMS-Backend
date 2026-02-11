using KMS.Service.DTOs.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDTO>> GetAllAsync();
        Task<MenuDTO?> GetByIdAsync(int id);
        Task<List<MenuDTO>> GetByClassIdAsync(int classId);
        Task CreateAsync(MenuCreateDTO dto, int createdBy);
        Task<bool> UpdateAsync(int id, MenuUpdateDTO dto, int updatedBy);
        Task<bool> DeleteAsync(int id);
    }
}
