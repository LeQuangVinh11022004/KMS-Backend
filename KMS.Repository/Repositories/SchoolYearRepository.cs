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
    public class SchoolYearRepository : ISchoolYearRepository
    {
        private readonly ApplicationDbContext _context;

        public SchoolYearRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsSchoolYear>> GetAllAsync()
            => await _context.KmsSchoolYears.ToListAsync();

        public async Task<KmsSchoolYear?> GetByIdAsync(int id)
            => await _context.KmsSchoolYears.FindAsync(id);

        public async Task AddAsync(KmsSchoolYear entity)
            => await _context.KmsSchoolYears.AddAsync(entity);

        public void Update(KmsSchoolYear entity)
            => _context.KmsSchoolYears.Update(entity);

        public async Task<bool> ExistsAsync(int id)
            => await _context.KmsSchoolYears.AnyAsync(x => x.SchoolYearId == id);
    }
}
