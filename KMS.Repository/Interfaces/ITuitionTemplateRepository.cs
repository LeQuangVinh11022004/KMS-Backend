using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface ITuitionTemplateRepository
    {
        // Read
        Task<KmsTuitionTemplate?> GetByIdAsync(int templateId);
        Task<KmsTuitionTemplate?> GetByIdWithItemsAsync(int templateId);
        Task<IEnumerable<KmsTuitionTemplate>> GetAllAsync();
        Task<IEnumerable<KmsTuitionTemplate>> GetActiveTemplatesAsync();
        Task<bool> TemplateNameExistsAsync(string name, int? excludeId = null);

        // Create
        Task<KmsTuitionTemplate> CreateAsync(KmsTuitionTemplate template);

        // Update
        Task UpdateAsync(KmsTuitionTemplate template);

        // Delete
        Task DeleteAsync(int templateId);

        // Items
        Task<KmsTuitionItem?> GetItemByIdAsync(int itemId);
        Task<KmsTuitionItem> CreateItemAsync(KmsTuitionItem item);
        Task DeleteItemsByTemplateIdAsync(int templateId);

        // Stats
        Task<int> CountTemplatesAsync();
        Task<bool> IsUsedByInvoicesAsync(int templateId);
    }
}
