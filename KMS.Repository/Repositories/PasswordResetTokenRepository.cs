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
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public PasswordResetTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KmsPasswordResetToken> CreateAsync(KmsPasswordResetToken token)
        {
            _context.KmsPasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<KmsPasswordResetToken?> GetByTokenAsync(string token)
        {
            return await _context.KmsPasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task MarkAsUsedAsync(int tokenId)
        {
            var token = await _context.KmsPasswordResetTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task InvalidateAllUserTokensAsync(int userId)
        {
            var tokens = await _context.KmsPasswordResetTokens
                .Where(t => t.UserId == userId && t.IsUsed != true)
                .ToListAsync();

            foreach (var token in tokens)
                token.IsUsed = true;

            await _context.SaveChangesAsync();
        }
    }
}
