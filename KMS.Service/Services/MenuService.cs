using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Menu;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(IMenuRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MenuDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDTO).ToList();
        }

        public async Task<MenuDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToDTO(entity);
        }

        public async Task<List<MenuDTO>> GetByClassIdAsync(int classId)
        {
            var list = await _repo.GetByClassIdAsync(classId);
            return list.Select(MapToDTO).ToList();
        }

        public async Task CreateAsync(MenuCreateDTO dto, int createdBy)
        {
            var entity = new KmsMenu
            {
                ClassId = dto.ClassId,
                MenuDate = dto.MenuDate,
                MealType = dto.MealType,
                MenuContent = dto.MenuContent,
                Calories = dto.Calories,
                Allergens = dto.Allergens,
                Source = dto.Source,
                SupplierName = dto.SupplierName,
                PreparedBy = dto.PreparedBy,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            await _repo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, MenuUpdateDTO dto, int updatedBy)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            entity.ClassId = dto.ClassId;
            entity.MenuDate = dto.MenuDate;
            entity.MealType = dto.MealType;
            entity.MenuContent = dto.MenuContent;
            entity.Calories = dto.Calories;
            entity.Allergens = dto.Allergens;
            entity.Source = dto.Source;
            entity.SupplierName = dto.SupplierName;
            entity.PreparedBy = dto.PreparedBy;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = updatedBy;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            _repo.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static MenuDTO MapToDTO(KmsMenu x)
        {
            return new MenuDTO
            {
                MenuId = x.MenuId,
                ClassId = x.ClassId,
                MenuDate = x.MenuDate,
                MealType = x.MealType,
                MenuContent = x.MenuContent,
                Calories = x.Calories,
                Allergens = x.Allergens,
                Source = x.Source,
                SupplierName = x.SupplierName,
                PreparedBy = x.PreparedBy
            };
        }
    }
}
