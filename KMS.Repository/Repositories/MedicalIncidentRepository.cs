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
    public class MedicalIncidentRepository : IMedicalIncidentRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalIncidentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsMedicalIncident>> GetAllAsync()
        {
            return await _context.KmsMedicalIncidents
                .Include(x => x.Student)
                .ToListAsync();
        }

        public async Task<KmsMedicalIncident?> GetByIdAsync(int id)
        {
            return await _context.KmsMedicalIncidents
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.IncidentId == id);
        }

        public async Task<List<KmsMedicalIncident>> GetByStudentIdAsync(int studentId)
        {
            return await _context.KmsMedicalIncidents
                .Where(x => x.StudentId == studentId)
                .ToListAsync();
        }

        public async Task AddAsync(KmsMedicalIncident entity)
        {
            await _context.KmsMedicalIncidents.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KmsMedicalIncident entity)
        {
            _context.KmsMedicalIncidents.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(KmsMedicalIncident entity)
        {
            _context.KmsMedicalIncidents.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
