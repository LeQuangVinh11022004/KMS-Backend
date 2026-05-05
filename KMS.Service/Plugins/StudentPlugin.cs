using KMS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KMS.Service.Plugins
{
    public class StudentPlugin
    {
        private readonly ApplicationDbContext _context;
        private readonly int _userId;

        public StudentPlugin(ApplicationDbContext context, int userId)
        {
            _context = context;
            _userId = userId;
        }

        [KernelFunction("get_my_students")]
        [Description("BẮT BUỘC PHẢI GỌI HÀM NÀY ĐẦU TIÊN để lấy danh sách học sinh và StudentId của phụ huynh, trước khi tra cứu điểm danh hay báo cáo.")]
        public async Task<string> GetMyStudentsAsync()
        {
            var students = await (from sp in _context.KmsStudentParents
                                  join s in _context.KmsStudents on sp.StudentId equals s.StudentId
                                  where sp.Parent.UserId == _userId
                                  select new { s.StudentId, s.FullName }).ToListAsync();

            if (!students.Any()) return "Không tìm thấy học sinh nào.";

            return "Danh sách học sinh:\n" + string.Join("\n", students.Select(s => $"- {s.FullName} (Mã StudentId: {s.StudentId})"));
        }
    }
}