using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Entities
{
    public partial class KmsEvaluationCriteria
    {
        public int CriteriaId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RatingType { get; set; }
        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<KmsEvaluationDetail> KmsEvaluationDetails { get; set; } = new List<KmsEvaluationDetail>();
    }
}
