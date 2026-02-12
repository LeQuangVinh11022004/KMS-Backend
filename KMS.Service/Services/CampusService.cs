using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Campus;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class CampusService : ICampusService
    {
        private readonly ICampusRepository _repo;

        public CampusService(ICampusRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CampusDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(x => new CampusDTO
            {
                CampusId = x.CampusId,
                CampusName = x.CampusName,
                Address = x.Address,
                Phone = x.Phone,
                IsActive = x.IsActive
            }).ToList();
        }

        public async Task<CampusDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new CampusDTO
            {
                CampusId = entity.CampusId,
                CampusName = entity.CampusName,
                Address = entity.Address,
                Phone = entity.Phone,
                IsActive = entity.IsActive
            };
        }

        public async Task CreateAsync(CampusDTO dto)
        {
            var entity = new KmsCampus
            {
                CampusName = dto.CampusName,
                Address = dto.Address,
                Phone = dto.Phone,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, CampusDTO dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Campus not found");

            entity.CampusName = dto.CampusName;
            entity.Address = dto.Address;
            entity.Phone = dto.Phone;
            entity.IsActive = dto.IsActive;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }

        // Soft delete
        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Campus not found");

            entity.IsActive = false;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }
    }
}
