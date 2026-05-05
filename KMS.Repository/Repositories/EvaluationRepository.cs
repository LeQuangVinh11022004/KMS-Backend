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
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly ApplicationDbContext _context;

        public EvaluationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KmsStudentEvaluation?> GetByIdAsync(int evaluationId)
        {
            return await _context.KmsStudentEvaluations
                .Include(e => e.Student)
                .Include(e => e.Class)
                .Include(e => e.EvaluatedByNavigation)
                .Include(e => e.KmsEvaluationDetails)
                    .ThenInclude(d => d.Criteria)
                .FirstOrDefaultAsync(e => e.EvaluationId == evaluationId);
        }

        public async Task<List<KmsStudentEvaluation>> GetByStudentIdAsync(
            int studentId, string? periodType = null)
        {
            var query = _context.KmsStudentEvaluations
                .Include(e => e.Student)
                .Include(e => e.Class)
                .Include(e => e.EvaluatedByNavigation)
                .Where(e => e.StudentId == studentId);

            if (!string.IsNullOrEmpty(periodType))
                query = query.Where(e => e.PeriodType == periodType);

            return await query
                .OrderByDescending(e => e.PeriodStart)
                .ToListAsync();
        }

        public async Task<List<KmsStudentEvaluation>> GetByClassIdAsync(
            int classId, string periodType, DateOnly periodStart)
        {
            return await _context.KmsStudentEvaluations
                .Include(e => e.Student)
                .Include(e => e.KmsEvaluationDetails)
                    .ThenInclude(d => d.Criteria)
                .Where(e => e.ClassId == classId
                         && e.PeriodType == periodType
                         && e.PeriodStart == periodStart)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int studentId, string periodType, DateOnly periodStart)
        {
            return await _context.KmsStudentEvaluations
                .AnyAsync(e => e.StudentId == studentId
                            && e.PeriodType == periodType
                            && e.PeriodStart == periodStart);
        }

        public async Task<KmsStudentEvaluation> AddAsync(KmsStudentEvaluation entity)
        {
            _context.KmsStudentEvaluations.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(KmsStudentEvaluation entity)
        {
            _context.KmsStudentEvaluations.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddDetailsAsync(List<KmsEvaluationDetail> details)
        {
            _context.KmsEvaluationDetails.AddRange(details);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDetailsByEvaluationIdAsync(int evaluationId)
        {
            var details = await _context.KmsEvaluationDetails
                .Where(d => d.EvaluationId == evaluationId)
                .ToListAsync();

            _context.KmsEvaluationDetails.RemoveRange(details);
            await _context.SaveChangesAsync();
        }
    }
}
