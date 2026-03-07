using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Repositories
{
    public class KitchenSampleRepository : IKitchenSampleRepository
    {
        private readonly ApplicationDbContext _context;

        public KitchenSampleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsKitchenSample>> GetAllAsync()
        {
            return await _context.KmsKitchenSamples
                .ToListAsync();
        }

        public async Task<KmsKitchenSample?> GetByIdAsync(int id)
        {
            return await _context.KmsKitchenSamples
                .FirstOrDefaultAsync(x => x.SampleId == id);
        }

        public async Task AddAsync(KmsKitchenSample entity)
        {
            await _context.KmsKitchenSamples.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KmsKitchenSample entity)
        {
            _context.KmsKitchenSamples.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(KmsKitchenSample entity)
        {
            _context.KmsKitchenSamples.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}