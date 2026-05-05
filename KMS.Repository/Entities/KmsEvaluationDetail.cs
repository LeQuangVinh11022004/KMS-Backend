using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Entities
{
    public partial class KmsEvaluationDetail
    {
        public int DetailId { get; set; }
        public int EvaluationId { get; set; }
        public int CriteriaId { get; set; }
        public short? Score { get; set; }        
        public string? RatingLabel { get; set; } 
        public string? Comment { get; set; }
        public virtual KmsStudentEvaluation Evaluation { get; set; }
        public virtual KmsEvaluationCriteria Criteria { get; set; }
    }
}
