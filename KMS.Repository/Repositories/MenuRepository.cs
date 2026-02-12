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
    public class MenuRepository : IMenuRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsMenu>> GetAllAsync()
            => await _context.KmsMenus.ToListAsync();

        public async Task<KmsMenu?> GetByIdAsync(int id)
            => await _context.KmsMenus.FindAsync(id);

        public async Task<List<KmsMenu>> GetByClassIdAsync(int classId)
            => await _context.KmsMenus
                .Where(x => x.ClassId == classId)
                .ToListAsync();

        public async Task AddAsync(KmsMenu entity)
            => await _context.KmsMenus.AddAsync(entity);

        public void Update(KmsMenu entity)
            => _context.KmsMenus.Update(entity);

        public void Delete(KmsMenu entity)
            => _context.KmsMenus.Remove(entity);
    }
}
