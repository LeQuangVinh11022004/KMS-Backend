using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Entities
{
    public partial class KmsStudentEvaluation
    {
        public int EvaluationId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string? PeriodType { get; set; }      // weekly | monthly | semester
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public bool? IsGoodStudent { get; set; }    // null = chưa xác định
        public string? GeneralComment { get; set; }
        public int? EvaluatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public virtual KmsStudent Student { get; set; }
        public virtual KmsClass Class { get; set; }
        public virtual KmsUser EvaluatedByNavigation { get; set; }
        public virtual ICollection<KmsEvaluationDetail> KmsEvaluationDetails { get; set; } = new List<KmsEvaluationDetail>();
    }
}
