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
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ApplicationDbContext _context;

        public SemesterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsSemester>> GetAllAsync()
        {
            return await _context.KmsSemesters
                .Include(x => x.SchoolYear)
                .ToListAsync();
        }

        public async Task<KmsSemester?> GetByIdAsync(int id)
        {
            return await _context.KmsSemesters
                .FirstOrDefaultAsync(x => x.SemesterId == id);
        }

        public async Task AddAsync(KmsSemester entity)
        {
            await _context.KmsSemesters.AddAsync(entity);
        }

        public void Update(KmsSemester entity)
        {
            _context.KmsSemesters.Update(entity);
        }

        public void Delete(KmsSemester entity)
        {
            _context.KmsSemesters.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
