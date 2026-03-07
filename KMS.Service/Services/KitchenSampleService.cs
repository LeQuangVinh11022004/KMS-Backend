using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.KitchenSample;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class KitchenSampleService : IKitchenSampleService
    {
        private readonly IKitchenSampleRepository _repo;

        public KitchenSampleService(IKitchenSampleRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<KitchenSampleDTO>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();

            return data.Select(x => new KitchenSampleDTO
            {
                MenuId = x.MenuId,
                SampleDate = x.SampleDate,
                SampleTime = x.SampleTime,
                DisposalTime = x.DisposalTime,
                Note = x.Note
            }).ToList();
        }

        public async Task<KitchenSampleDTO?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new KitchenSampleDTO
            {
                MenuId = x.MenuId,
                SampleDate = x.SampleDate,
                SampleTime = x.SampleTime,
                DisposalTime = x.DisposalTime,
                Note = x.Note
            };
        }

        public async Task CreateAsync(KitchenSampleDTO dto, int userId)
        {
            var entity = new KmsKitchenSample
            {
                MenuId = dto.MenuId,
                SampleDate = dto.SampleDate ?? DateOnly.FromDateTime(DateTime.Now),
                SampleTime = dto.SampleTime ?? TimeOnly.FromDateTime(DateTime.Now),
                DisposalTime = dto.DisposalTime,
                Note = dto.Note,
                StoredBy = userId
            };

            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, KitchenSampleDTO dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Kitchen sample not found");

            entity.MenuId = dto.MenuId;
            entity.SampleDate = dto.SampleDate;
            entity.SampleTime = dto.SampleTime;
            entity.DisposalTime = dto.DisposalTime;
            entity.Note = dto.Note;

            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Kitchen sample not found");

            await _repo.DeleteAsync(entity);
        }
    }
}