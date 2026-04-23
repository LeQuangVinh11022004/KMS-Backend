using KMS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KMS.Service.Plugins
{
    public class MenuPlugin
    {
        private readonly ApplicationDbContext _context;
        public MenuPlugin(ApplicationDbContext context) => _context = context;

        [KernelFunction("get_today_menu")]
        [Description("Lấy thực đơn món ăn của trường hôm nay")]
        public async Task<string> GetTodayMenuAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var menus = await _context.KmsMenus.Where(x => x.MenuDate == today).ToListAsync();

            if (!menus.Any()) return "Hôm nay chưa có thực đơn.";
            return string.Join("\n", menus.Select(m => $"- {m.MealType}: {m.MenuContent}"));
        }
    }
}