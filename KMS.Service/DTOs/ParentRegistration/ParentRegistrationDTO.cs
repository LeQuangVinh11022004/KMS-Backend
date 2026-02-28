namespace KMS.Service.DTOs
{
    public class ParentRegistrationDTO
    {
        public int RegistrationId { get; set; }
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string FullName { get; set; } = null!;
        public string? ChildFullName { get; set; }
        public DateTime? ChildDateOfBirth { get; set; }
        public string? RequestMessage { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ReviewNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByName { get; set; }
    }

    public class CreateParentRegistrationDTO
    {
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string FullName { get; set; } = null!;
        public string? ChildFullName { get; set; }
        public DateTime? ChildDateOfBirth { get; set; }
        public string? RequestMessage { get; set; }
    }

    public class ReviewRegistrationDTO
    {
        public string? ReviewNote { get; set; }
    }

    public class ApproveRegistrationResultDTO
    {
        public ParentRegistrationDTO Registration { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string TempPassword { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Note { get; set; } = "Vui lòng gửi thông tin đăng nhập này cho phụ huynh và yêu cầu đổi mật khẩu sau lần đăng nhập đầu tiên.";
    }
}