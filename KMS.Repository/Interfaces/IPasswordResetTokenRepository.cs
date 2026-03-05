using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task<KmsPasswordResetToken> CreateAsync(KmsPasswordResetToken token);
        Task<KmsPasswordResetToken?> GetByTokenAsync(string token);
        Task MarkAsUsedAsync(int tokenId);
        Task InvalidateAllUserTokensAsync(int userId); // Hủy token cũ khi tạo token mới
    }
}
