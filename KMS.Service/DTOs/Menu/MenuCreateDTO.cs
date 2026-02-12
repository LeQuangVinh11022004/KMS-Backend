using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Menu
{
    public class MenuCreateDTO
    {
        public int? ClassId { get; set; }
        public DateOnly MenuDate { get; set; }
        public string MealType { get; set; }
        public string MenuContent { get; set; }
        public int? Calories { get; set; }
        public string Allergens { get; set; }
        public string Source { get; set; }
        public string SupplierName { get; set; }
        public int? PreparedBy { get; set; }
    }
}
