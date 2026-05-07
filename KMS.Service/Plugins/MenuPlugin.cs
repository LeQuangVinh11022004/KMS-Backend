using KMS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using System.Linq;

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

            if (!menus.Any())
            {
                return JsonSerializer.Serialize(new { hasData = false, message = "no_menu_today" });
            }

            var menuData = menus.Select(m => new { mealType = m.MealType, menuContent = m.MenuContent }).ToList();
            return JsonSerializer.Serialize(new { hasData = true, menus = menuData });
        }
    }
}