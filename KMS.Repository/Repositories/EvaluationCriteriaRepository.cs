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
    public class EvaluationCriteriaRepository : IEvaluationCriteriaRepository
    {
        private readonly ApplicationDbContext _context;

        public EvaluationCriteriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsEvaluationCriteria>> GetAllAsync(bool activeOnly = true)
        {
            var query = _context.KmsEvaluationCriteria.AsQueryable();

            if (activeOnly)
                query = query.Where(c => c.IsActive);

            return await query
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CriteriaId)
                .ToListAsync();
        }

        public async Task<KmsEvaluationCriteria?> GetByIdAsync(int criteriaId)
        {
            return await _context.KmsEvaluationCriteria
                .FirstOrDefaultAsync(c => c.CriteriaId == criteriaId);
        }

        public async Task<KmsEvaluationCriteria> AddAsync(KmsEvaluationCriteria entity)
        {
            _context.KmsEvaluationCriteria.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(KmsEvaluationCriteria entity)
        {
            _context.KmsEvaluationCriteria.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(KmsEvaluationCriteria entity)
        {
            _context.KmsEvaluationCriteria.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}