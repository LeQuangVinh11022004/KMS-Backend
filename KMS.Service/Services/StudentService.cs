
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Parent;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.Student;
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
                int currentYear = DateTime.Now.Year;
                string prefix = $"STU{currentYear}";

                var lastStudentCode = await _studentRepository.GetLatestStudentCodeAsync(prefix);

                int nextSequence = 1; // Mac dinh bat dau tu 001

                if (!string.IsNullOrEmpty(lastStudentCode) && lastStudentCode.Length >= 10)
                {
                    // cat phan chuoi so cuoi cung (3 chu so) de lay so thu tu hien tai
                    string lastSequenceStr = lastStudentCode.Substring(7);
                    if (int.TryParse(lastSequenceStr, out int lastNumber))
                    {
                        nextSequence = lastNumber + 1;
                    }
                }

                // format lai thanh chuoi 3 chu so
                string newStudentCode = $"{prefix}{nextSequence:D3}";

                var student = new KmsStudent
                {
                    StudentCode = newStudentCode,
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

                if (dto.ClassId.HasValue)
                {
                    var currentClassMapping = student.KmsClassStudents?.FirstOrDefault();

                    if (currentClassMapping != null)
                    {
                        if (currentClassMapping.ClassId != dto.ClassId.Value)
                        {
                            if (dto.ClassId.Value > 0)
                            {
                                student.KmsClassStudents!.Remove(currentClassMapping);
                                student.KmsClassStudents.Add(new Repository.Entities.KmsClassStudent
                                {
                                    StudentId = student.StudentId,
                                    ClassId = dto.ClassId.Value
                                });
                            }
                            else
                            {
                                student.KmsClassStudents!.Remove(currentClassMapping);
                            }
                        }
                    }
                    else
                    {
                        if (dto.ClassId.Value > 0)
                        {
                            student.KmsClassStudents ??= new List<KmsClassStudent>();
                            student.KmsClassStudents.Add(new KmsClassStudent
                            {
                                StudentId = student.StudentId,
                                ClassId = dto.ClassId.Value
                            });
                        }
                    }
                }
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

            var currentClass = student.KmsClassStudents?.FirstOrDefault();

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
                IsActive = student.IsActive == true,
                ClassId = currentClass?.ClassId,       
                ClassName = currentClass?.Class?.ClassName,
                Parents = student.KmsStudentParents?.Select(sp => new ParentDTO
                {
                    ParentId = sp.Parent.ParentId,
                    UserId = sp.Parent.UserId,                          
                    FullName = sp.Parent.User?.FullName ?? "",
                    Email = sp.Parent.User?.Email,
                    Phone = sp.Parent.User?.Phone,
                    Occupation = sp.Parent.Occupation,                   
                    WorkAddress = sp.Parent.WorkAddress,               
                    EmergencyContact = sp.Parent.EmergencyContact,      
                    CreatedAt = sp.Parent.CreatedAt ?? DateTime.Now 
                }).ToList() ?? new()
            };
        }
    }
}