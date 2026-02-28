using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class ParentRegistrationService : IParentRegistrationService
    {
        private readonly IParentRegistrationRepository _registrationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public ParentRegistrationService(
            IParentRegistrationRepository registrationRepository,
            IUserRepository userRepository,
            IParentRepository parentRepository,
            IUserRoleRepository userRoleRepository)
        {
            _registrationRepository = registrationRepository;
            _userRepository = userRepository;
            _parentRepository = parentRepository;
            _userRoleRepository = userRoleRepository;
        }

        // ============================================================
        // PUBLIC - Phụ huynh tự submit
        // ============================================================

        public async Task<BaseResponseDTO> SubmitRegistrationAsync(CreateParentRegistrationDTO dto)
        {
            try
            {
                // Validate email format
                if (!IsValidEmailFormat(dto.Email))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Email không đúng định dạng (ví dụ: example@gmail.com)"
                    };

                // Validate phone format nếu có nhập
                if (!string.IsNullOrEmpty(dto.Phone) && !IsValidPhoneFormat(dto.Phone))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Số điện thoại không đúng định dạng (phải bắt đầu bằng số, ví dụ: 0901234567)"
                    };

                // Kiểm tra email đã có tài khoản chưa
                if (await _registrationRepository.EmailAlreadyRegisteredAsync(dto.Email))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Email này đã được đăng ký tài khoản. Vui lòng liên hệ nhà trường nếu cần hỗ trợ."
                    };

                // Kiểm tra đã có đơn Pending chưa (tránh spam)
                if (await _registrationRepository.EmailAlreadyPendingAsync(dto.Email))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Email này đã có đơn đăng ký đang chờ duyệt. Vui lòng đợi nhà trường xét duyệt."
                    };

                var registration = new KmsParentRegistration
                {
                    Email = dto.Email,
                    Phone = dto.Phone,
                    FullName = dto.FullName,
                    ChildFullName = dto.ChildFullName,
                    ChildDateOfBirth = dto.ChildDateOfBirth.HasValue
                        ? DateOnly.FromDateTime(dto.ChildDateOfBirth.Value)
                        : null,
                    RequestMessage = dto.RequestMessage,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                var created = await _registrationRepository.CreateAsync(registration);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Đơn đăng ký đã được gửi thành công. Nhà trường sẽ liên hệ với bạn sau khi xét duyệt.",
                    Data = MapToDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // ADMIN - Read
        // ============================================================

        public async Task<BaseResponseDTO> GetAllRegistrationsAsync()
        {
            try
            {
                var registrations = await _registrationRepository.GetAllAsync();
                var dtos = registrations.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} registrations",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetRegistrationByIdAsync(int registrationId)
        {
            try
            {
                var registration = await _registrationRepository.GetByIdAsync(registrationId);
                if (registration == null)
                    return new BaseResponseDTO { Success = false, Message = "Registration not found" };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Registration retrieved successfully",
                    Data = MapToDTO(registration)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetPendingRegistrationsAsync()
        {
            try
            {
                var registrations = await _registrationRepository.GetByStatusAsync("Pending");
                var dtos = registrations.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} pending registrations",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // ADMIN - Approve → Tự động tạo User + Parent + gán Role
        // ============================================================

        public async Task<BaseResponseDTO> ApproveRegistrationAsync(
            int registrationId, int reviewedBy, ReviewRegistrationDTO dto)
        {
            try
            {
                var registration = await _registrationRepository.GetByIdAsync(registrationId);
                if (registration == null)
                    return new BaseResponseDTO { Success = false, Message = "Registration not found" };

                if (registration.Status != "Pending")
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Cannot approve - registration is already {registration.Status}"
                    };

                // Kiểm tra lại email chưa có tài khoản (phòng race condition)
                if (await _registrationRepository.EmailAlreadyRegisteredAsync(registration.Email))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Email này đã có tài khoản trong hệ thống"
                    };

                // Validate email format khớp với CHECK constraint DB: %_@__%.__%
                if (!IsValidEmailFormat(registration.Email))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Email '{registration.Email}' không đúng định dạng. Vui lòng cập nhật đơn đăng ký."
                    };

                // ── Tạo username từ email (phần trước @) ──────────────────
                var baseUsername = registration.Email.Split('@')[0].ToLower();
                var username = baseUsername;
                var counter = 1;

                // Nếu username đã tồn tại thì thêm số vào sau
                while (await _userRepository.UsernameExistsAsync(username))
                {
                    username = $"{baseUsername}{counter}";
                    counter++;
                }

                // ── Mật khẩu tạm = số điện thoại, fallback = "Parent@123" ─
                var tempPassword = !string.IsNullOrEmpty(registration.Phone)
                    ? registration.Phone
                    : "Parent@123";

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

                // Chỉ set Phone nếu hợp lệ, tránh CK_Phone_Format constraint
                string? validPhone = !string.IsNullOrEmpty(registration.Phone)
                    && IsValidPhoneFormat(registration.Phone)
                    ? registration.Phone
                    : null;

                // ── Tạo KmsUser ───────────────────────────────────────────
                var newUser = new KmsUser
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    FullName = registration.FullName,
                    Email = registration.Email,
                    Phone = validPhone,
                    IsActive = true,
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var createdUser = await _userRepository.CreateAsync(newUser);

                // ── Tạo KmsParent liên kết với User ───────────────────────
                var newParent = new KmsParent
                {
                    UserId = createdUser.UserId,
                    CreatedAt = DateTime.Now
                };

                await _parentRepository.CreateAsync(newParent);

                // ── Gán Role "Parent" ─────────────────────────────────────
                var parentRole = await _userRoleRepository.GetRoleByNameAsync("Parent");
                if (parentRole != null)
                {
                    await _userRoleRepository.AssignRoleToUserAsync(
                        createdUser.UserId, parentRole.RoleId, reviewedBy);
                }

                // ── Cập nhật trạng thái đơn đăng ký ──────────────────────
                registration.Status = "Approved";
                registration.ReviewedBy = reviewedBy;
                registration.ReviewedAt = DateTime.Now;
                registration.ReviewNote = dto.ReviewNote;

                await _registrationRepository.UpdateAsync(registration);

                // ── Trả về thông tin tài khoản để Admin gửi cho phụ huynh ─
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Đã duyệt đơn và tạo tài khoản thành công",
                    Data = new ApproveRegistrationResultDTO
                    {
                        Registration = MapToDTO(registration),
                        Username = username,
                        TempPassword = tempPassword,
                        FullName = registration.FullName,
                        Email = registration.Email,
                        Phone = registration.Phone
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // ADMIN - Reject
        // ============================================================

        public async Task<BaseResponseDTO> RejectRegistrationAsync(
            int registrationId, int reviewedBy, ReviewRegistrationDTO dto)
        {
            try
            {
                var registration = await _registrationRepository.GetByIdAsync(registrationId);
                if (registration == null)
                    return new BaseResponseDTO { Success = false, Message = "Registration not found" };

                if (registration.Status != "Pending")
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Cannot reject - registration is already {registration.Status}"
                    };

                registration.Status = "Rejected";
                registration.ReviewedBy = reviewedBy;
                registration.ReviewedAt = DateTime.Now;
                registration.ReviewNote = dto.ReviewNote;

                await _registrationRepository.UpdateAsync(registration);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Đã từ chối đơn đăng ký",
                    Data = MapToDTO(registration)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // MAPPER
        // ============================================================

        // ============================================================
        // HELPERS
        // ============================================================

        private static bool IsValidPhoneFormat(string phone)
        {
            // DB constraint: Phone LIKE '[0-9]%' → phải bắt đầu bằng số
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return char.IsDigit(phone[0]);
        }

        private static bool IsValidEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email && email.Contains('.');
            }
            catch
            {
                return false;
            }
        }

        private ParentRegistrationDTO MapToDTO(KmsParentRegistration r)
        {
            return new ParentRegistrationDTO
            {
                RegistrationId = r.RegistrationId,
                Email = r.Email,
                Phone = r.Phone,
                FullName = r.FullName,
                ChildFullName = r.ChildFullName,
                ChildDateOfBirth = r.ChildDateOfBirth.HasValue
                    ? r.ChildDateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
                    : null,
                RequestMessage = r.RequestMessage,
                Status = r.Status ?? "Pending",
                ReviewNote = r.ReviewNote,
                CreatedAt = r.CreatedAt ?? DateTime.Now,
                ReviewedAt = r.ReviewedAt,
                ReviewedByName = r.ReviewedByNavigation?.FullName
            };
        }
    }
}