using KMS.Repository.Data;
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
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsClass>> GetAllAsync()
            => await _context.KmsClasses.ToListAsync();

        public async Task<KmsClass?> GetByIdAsync(int id)
            => await _context.KmsClasses.FirstOrDefaultAsync(x => x.ClassId == id);

        public async Task AddAsync(KmsClass entity)
            => await _context.KmsClasses.AddAsync(entity);

        public void Update(KmsClass entity)
            => _context.KmsClasses.Update(entity);

        public void Delete(KmsClass entity)
            => _context.KmsClasses.Remove(entity);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
