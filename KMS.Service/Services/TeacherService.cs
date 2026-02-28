
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.Teacher;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;

        public TeacherService(ITeacherRepository teacherRepository, IUserRepository userRepository)
        {
            _teacherRepository = teacherRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResponseDTO> GetAllTeachersAsync()
        {
            try
            {
                var teachers = await _teacherRepository.GetAllAsync();
                var teacherDTOs = teachers.Select(t => MapToDTO(t)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {teacherDTOs.Count} teachers",
                    Data = teacherDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> GetTeacherByIdAsync(int teacherId)
        {
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(teacherId);
                if (teacher == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Teacher not found"
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Teacher retrieved successfully",
                    Data = MapToDTO(teacher)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> CreateTeacherAsync(CreateTeacherDTO dto)
        {
            try
            {
                // Validate user exists
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Check if teacher code exists
                if (await _teacherRepository.TeacherCodeExistsAsync(dto.TeacherCode))
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Teacher code already exists"
                    };
                }

                // Check if user is already a teacher
                if (await _teacherRepository.UserIsTeacherAsync(dto.UserId))
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "User is already a teacher"
                    };
                }

                var teacher = new KmsTeacher
                {
                    UserId = dto.UserId,
                    TeacherCode = dto.TeacherCode,
                    Specialization = dto.Specialization,
                    HireDate = dto.HireDate,
                    Salary = dto.Salary,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var created = await _teacherRepository.CreateAsync(teacher);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Teacher created successfully",
                    Data = MapToDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> UpdateTeacherAsync(int teacherId, UpdateTeacherDTO dto)
        {
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(teacherId);
                if (teacher == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Teacher not found"
                    };
                }

                // Update fields
                if (dto.Specialization != null) teacher.Specialization = dto.Specialization;
                if (dto.HireDate.HasValue) teacher.HireDate = dto.HireDate;
                if (dto.Salary.HasValue) teacher.Salary = dto.Salary;
                if (dto.IsActive.HasValue) teacher.IsActive = dto.IsActive.Value;

                await _teacherRepository.UpdateAsync(teacher);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Teacher updated successfully",
                    Data = MapToDTO(teacher)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> DeleteTeacherAsync(int teacherId)
        {
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(teacherId);
                if (teacher == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Teacher not found"
                    };
                }

                await _teacherRepository.DeleteAsync(teacherId);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Teacher deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> SearchTeachersAsync(string keyword)
        {
            try
            {
                var teachers = await _teacherRepository.SearchTeachersAsync(keyword);
                var teacherDTOs = teachers.Select(t => MapToDTO(t)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {teacherDTOs.Count} teachers",
                    Data = teacherDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> GetActiveTeachersAsync()
        {
            try
            {
                var teachers = await _teacherRepository.GetActiveTeachersAsync();
                var teacherDTOs = teachers.Select(t => MapToDTO(t)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {teacherDTOs.Count} active teachers",
                    Data = teacherDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        private TeacherDTO MapToDTO(KmsTeacher teacher)
        {
            return new TeacherDTO
            {
                TeacherId = teacher.TeacherId,
                UserId = teacher.UserId,
                TeacherCode = teacher.TeacherCode,
                FullName = teacher.User?.FullName ?? "",
                Email = teacher.User?.Email,
                Phone = teacher.User?.Phone,
                Specialization = teacher.Specialization,
                HireDate = teacher.HireDate,
                Salary = teacher.Salary,
                IsActive = teacher.IsActive == true,
                CreatedAt = teacher.CreatedAt ?? DateTime.Now
            };
        }
    }
}