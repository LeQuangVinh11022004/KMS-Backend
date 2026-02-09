
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<BaseResponseDTO> GetAllStudentsAsync()
        {
            try
            {
                var students = await _studentRepository.GetAllAsync();
                var studentDTOs = students.Select(s => MapToDTO(s)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {studentDTOs.Count} students",
                    Data = studentDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetStudentByIdAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                {
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Student retrieved successfully",
                    Data = MapToDTO(student)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> CreateStudentAsync(CreateStudentDTO dto)
        {
            try
            {
                if (await _studentRepository.StudentCodeExistsAsync(dto.StudentCode))
                {
                    return new BaseResponseDTO { Success = false, Message = "Student code already exists" };
                }

                var student = new KmsStudent
                {
                    StudentCode = dto.StudentCode,
                    FullName = dto.FullName,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    Photo = dto.Photo,
                    BloodType = dto.BloodType,
                    Allergies = dto.Allergies,
                    MedicalNotes = dto.MedicalNotes,
                    EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true,
                    CreatedBy = dto.CreatedBy
                };

                var created = await _studentRepository.CreateAsync(student);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Student created successfully",
                    Data = MapToDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> UpdateStudentAsync(int studentId, UpdateStudentDTO dto)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                {
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };
                }

                if (dto.FullName != null) student.FullName = dto.FullName;
                if (dto.DateOfBirth.HasValue) student.DateOfBirth = dto.DateOfBirth.Value;
                if (dto.Gender != null) student.Gender = dto.Gender;
                if (dto.Address != null) student.Address = dto.Address;
                if (dto.Photo != null) student.Photo = dto.Photo;
                if (dto.BloodType != null) student.BloodType = dto.BloodType;
                if (dto.Allergies != null) student.Allergies = dto.Allergies;
                if (dto.MedicalNotes != null) student.MedicalNotes = dto.MedicalNotes;
                if (dto.IsActive.HasValue) student.IsActive = dto.IsActive.Value;
                student.UpdatedBy = dto.UpdatedBy;

                await _studentRepository.UpdateAsync(student);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Student updated successfully",
                    Data = MapToDTO(student)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> DeleteStudentAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                {
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };
                }

                await _studentRepository.DeleteAsync(studentId);

                return new BaseResponseDTO { Success = true, Message = "Student deleted successfully" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> SearchStudentsAsync(string keyword)
        {
            try
            {
                var students = await _studentRepository.SearchStudentsAsync(keyword);
                var studentDTOs = students.Select(s => MapToDTO(s)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {studentDTOs.Count} students",
                    Data = studentDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetActiveStudentsAsync()
        {
            try
            {
                var students = await _studentRepository.GetActiveStudentsAsync();
                var studentDTOs = students.Select(s => MapToDTO(s)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {studentDTOs.Count} active students",
                    Data = studentDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetStudentsByParentIdAsync(int parentId)
        {
            try
            {
                var students = await _studentRepository.GetStudentsByParentIdAsync(parentId);
                var studentDTOs = students.Select(s => MapToDTO(s)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {studentDTOs.Count} students",
                    Data = studentDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private StudentDTO MapToDTO(KmsStudent student)
        {
            var age = DateOnly.FromDateTime(DateTime.Now).Year - student.DateOfBirth.Year;
            if (DateOnly.FromDateTime(DateTime.Now) < student.DateOfBirth.AddYears(age)) age--;

            return new StudentDTO
            {
                StudentId = student.StudentId,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                Age = age,
                Gender = student.Gender,
                Address = student.Address,
                Photo = student.Photo,
                BloodType = student.BloodType,
                Allergies = student.Allergies,
                MedicalNotes = student.MedicalNotes,
                EnrollmentDate = student.EnrollmentDate ?? DateOnly.FromDateTime(DateTime.Now),
                IsActive = student.IsActive == true
            };
        }
    }
}