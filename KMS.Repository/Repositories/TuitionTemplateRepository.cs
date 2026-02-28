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
    public class TuitionTemplateRepository : ITuitionTemplateRepository
    {
        private readonly ApplicationDbContext _context;

        public TuitionTemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<KmsTuitionTemplate?> GetByIdAsync(int templateId)
        {
            return await _context.KmsTuitionTemplates
                .FirstOrDefaultAsync(t => t.TemplateId == templateId);
        }

        public async Task<KmsTuitionTemplate?> GetByIdWithItemsAsync(int templateId)
        {
            return await _context.KmsTuitionTemplates
                .Include(t => t.KmsTuitionItems.OrderBy(i => i.ItemOrder))
                .FirstOrDefaultAsync(t => t.TemplateId == templateId);
        }

        public async Task<IEnumerable<KmsTuitionTemplate>> GetAllAsync()
        {
            return await _context.KmsTuitionTemplates
                .Include(t => t.KmsTuitionItems.OrderBy(i => i.ItemOrder))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsTuitionTemplate>> GetActiveTemplatesAsync()
        {
            return await _context.KmsTuitionTemplates
                .Include(t => t.KmsTuitionItems.OrderBy(i => i.ItemOrder))
                .Where(t => t.IsActive == true)
                .OrderBy(t => t.TemplateName)
                .ToListAsync();
        }

        public async Task<bool> TemplateNameExistsAsync(string name, int? excludeId = null)
        {
            return await _context.KmsTuitionTemplates
                .AnyAsync(t => t.TemplateName == name && (excludeId == null || t.TemplateId != excludeId));
        }

        // ============================================================
        // CREATE
        // ============================================================

        public async Task<KmsTuitionTemplate> CreateAsync(KmsTuitionTemplate template)
        {
            _context.KmsTuitionTemplates.Add(template);
            await _context.SaveChangesAsync();
            await _context.Entry(template)
                .Collection(t => t.KmsTuitionItems)
                .LoadAsync();
            return template;
        }

        // ============================================================
        // UPDATE
        // ============================================================

        public async Task UpdateAsync(KmsTuitionTemplate template)
        {
            _context.KmsTuitionTemplates.Update(template);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // DELETE
        // ============================================================

        public async Task DeleteAsync(int templateId)
        {
            var template = await _context.KmsTuitionTemplates.FindAsync(templateId);
            if (template != null)
            {
                _context.KmsTuitionTemplates.Remove(template);
                await _context.SaveChangesAsync();
            }
        }

        // ============================================================
        // ITEMS
        // ============================================================

        public async Task<KmsTuitionItem?> GetItemByIdAsync(int itemId)
        {
            return await _context.KmsTuitionItems.FindAsync(itemId);
        }

        public async Task<KmsTuitionItem> CreateItemAsync(KmsTuitionItem item)
        {
            _context.KmsTuitionItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemsByTemplateIdAsync(int templateId)
        {
            var items = await _context.KmsTuitionItems
                .Where(i => i.TemplateId == templateId)
                .ToListAsync();
            _context.KmsTuitionItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // STATS
        // ============================================================

        public async Task<int> CountTemplatesAsync()
        {
            return await _context.KmsTuitionTemplates.CountAsync();
        }

        public async Task<bool> IsUsedByInvoicesAsync(int templateId)
        {
            return await _context.KmsInvoices
                .AnyAsync(i => i.TemplateId == templateId);
        }
    }
}
