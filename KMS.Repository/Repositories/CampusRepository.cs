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
    public class CampusRepository : ICampusRepository
    {
        private readonly ApplicationDbContext _context;

        public CampusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsCampus>> GetAllAsync()
        {
            return await _context.KmsCampuses
                .Where(x => x.IsActive == true)
                .ToListAsync();
        }

        public async Task<KmsCampus?> GetByIdAsync(int id)
        {
            return await _context.KmsCampuses
                .FirstOrDefaultAsync(x => x.CampusId == id);
        }

        public async Task AddAsync(KmsCampus entity)
        {
            await _context.KmsCampuses.AddAsync(entity);
        }

        public void Update(KmsCampus entity)
        {
            _context.KmsCampuses.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
