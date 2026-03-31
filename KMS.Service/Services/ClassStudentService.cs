using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.ClassStudent;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEmailService _emailService;
        private readonly ISystemSettingRepository _settingRepository;

        public ClassStudentService(
            IClassStudentRepository classStudentRepository,
            IClassRepository classRepository,
            IStudentRepository studentRepository,
            IEmailService emailService,
            ISystemSettingRepository settingRepository)
        {
            _classStudentRepository = classStudentRepository;
            _classRepository = classRepository;
            _studentRepository = studentRepository;
            _emailService = emailService;
            _settingRepository = settingRepository;
        }

        public async Task<BaseResponseDTO> GetStudentsByClassAsync(int classId)
        {
            try
            {
                var kmsClass = await _classRepository.GetByIdAsync(classId);
                if (kmsClass == null)
                    return new BaseResponseDTO { Success = false, Message = "Class not found" };

                var records = await _classStudentRepository.GetStudentsByClassAsync(classId);
                var dtos = records.Select(MapToStudentDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} students in class",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetClassesByStudentAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };

                var records = await _classStudentRepository.GetClassesByStudentAsync(studentId);
                var dtos = records.Select(MapToStudentDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} classes for student",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> EnrollStudentAsync(int classId, EnrollStudentDTO dto, int enrolledBy)
        {
            try
            {
                var kmsClass = await _classRepository.GetByIdAsync(classId);
                if (kmsClass == null)
                    return new BaseResponseDTO { Success = false, Message = "Class not found" };

                if (kmsClass.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "Class is inactive" };

                var student = await _studentRepository.GetByIdAsync(dto.StudentId);
                if (student == null)
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };

                if (student.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "Student is inactive" };

                if (await _classStudentRepository.IsStudentInClassAsync(classId, dto.StudentId))
                    return new BaseResponseDTO { Success = false, Message = "Student is already enrolled in this class" };
                
                if (await _classStudentRepository.IsStudentInAnyClassAsync(dto.StudentId))
                    return new BaseResponseDTO { Success = false, Message = "Student is already enrolled in another class" };

                var currentCount = await _classStudentRepository.CountStudentsInClassAsync(classId);
                var maxStudentSetting = await _settingRepository.GetByKeyAsync("MaxStudentsPerClass");
                int maxCapacity = 20;
                if (maxStudentSetting != null && int.TryParse(maxStudentSetting.SettingValue, out int parsedValue))
                {
                    maxCapacity = parsedValue;
                }

                int finalLimit = kmsClass.MaxCapacity ?? maxCapacity;

                if (currentCount >= finalLimit)
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Lớp đã đầy. Giới hạn hiện tại là {finalLimit} học sinh."
                    };

                var record = new KmsClassStudent
                {
                    ClassId = classId,
                    StudentId = dto.StudentId,
                    EnrolledAt = DateTime.Now,
                    EnrolledBy = enrolledBy
                };

                var created = await _classStudentRepository.EnrollAsync(record);


                // Gửi email thông báo xếp lớp cho phụ huynh
                var primaryParent = created.Student.KmsStudentParents
                    .FirstOrDefault(sp => sp.IsPrimaryContact == true)
                    ?? created.Student.KmsStudentParents.FirstOrDefault();

                var parentEmail = primaryParent?.Parent?.User?.Email;
                var parentName = primaryParent?.Parent?.User?.FullName ?? "Quý phụ huynh";

                if (!string.IsNullOrEmpty(parentEmail))
                {
                    await _emailService.SendStudentEnrolledEmailAsync(
                        toEmail: parentEmail,
                        parentName: parentName,
                        studentName: student.FullName,
                        dateOfBirth: student.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                        className: created.Class.ClassName,
                        schoolYear: created.Class.SchoolYear?.YearName ?? "",
                        enrolledAt: DateTime.Now
                    );
                }

                kmsClass.CurrentEnrollment = currentCount + 1;
                _classRepository.Update(kmsClass);
                await _classRepository.SaveChangesAsync();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Student '{student.FullName}' enrolled successfully",
                    Data = MapToStudentDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> EnrollMultipleStudentsAsync(
            int classId, EnrollMultipleStudentsDTO dto, int enrolledBy)
        {
            try
            {
                var kmsClass = await _classRepository.GetByIdAsync(classId);
                if (kmsClass == null)
                    return new BaseResponseDTO { Success = false, Message = "Class not found" };

                if (kmsClass.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "Class is inactive" };

                var currentCount = await _classStudentRepository.CountStudentsInClassAsync(classId);
                var maxStudentSetting = await _settingRepository.GetByKeyAsync("MaxStudentsPerClass");
                int maxCapacity = 20;
                if (maxStudentSetting != null && int.TryParse(maxStudentSetting.SettingValue, out int parsedValue))
                {
                    maxCapacity = parsedValue;
                }

                int finalLimit = kmsClass.MaxCapacity ?? maxCapacity;
                var remaining = finalLimit - currentCount;

                if (dto.StudentIds.Count > remaining)
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Cannot enroll {dto.StudentIds.Count} students - only {remaining} slots available"
                    };

                var enrolled = new List<string>();
                var skipped = new List<string>();

                foreach (var studentId in dto.StudentIds)
                {
                    var student = await _studentRepository.GetByIdAsync(studentId);
                    if (student == null || student.IsActive != true)
                    {
                        skipped.Add($"StudentId {studentId} (not found or inactive)");
                        continue;
                    }

                    if (await _classStudentRepository.IsStudentInClassAsync(classId, studentId))
                    {
                        skipped.Add($"{student.FullName} (already enrolled)");
                        continue;
                    }

                    if (await _classStudentRepository.IsStudentInAnyClassAsync(studentId))
                    {
                        skipped.Add($"{student.FullName} (already enrolled in another class)");
                        continue;
                    }

                    await _classStudentRepository.EnrollAsync(new KmsClassStudent
                    {
                        ClassId = classId,
                        StudentId = studentId,
                        EnrolledAt = DateTime.Now,
                        EnrolledBy = enrolledBy
                    });
                    enrolled.Add(student.FullName);
                }

                var newCount = await _classStudentRepository.CountStudentsInClassAsync(classId);
                kmsClass.CurrentEnrollment = newCount;
                _classRepository.Update(kmsClass);
                await _classRepository.SaveChangesAsync();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Enrolled {enrolled.Count} students. Skipped {skipped.Count}.",
                    Data = new { Enrolled = enrolled, Skipped = skipped }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> RemoveStudentFromClassAsync(int classId, int studentId)
        {
            try
            {
                var record = await _classStudentRepository.GetAsync(classId, studentId);
                if (record == null)
                    return new BaseResponseDTO { Success = false, Message = "Student is not enrolled in this class" };

                await _classStudentRepository.RemoveAsync(classId, studentId);

                var kmsClass = await _classRepository.GetByIdAsync(classId);
                if (kmsClass != null)
                {
                    var newCount = await _classStudentRepository.CountStudentsInClassAsync(classId);
                    kmsClass.CurrentEnrollment = newCount;
                    _classRepository.Update(kmsClass);
                    await _classRepository.SaveChangesAsync();
                }

                return new BaseResponseDTO { Success = true, Message = "Student removed from class successfully" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> TransferStudentAsync(
            int studentId, int fromClassId, int toClassId, int enrolledBy)
        {
            try
            {
                if (!await _classStudentRepository.IsStudentInClassAsync(fromClassId, studentId))
                    return new BaseResponseDTO { Success = false, Message = "Student is not in the source class" };

                var toClass = await _classRepository.GetByIdAsync(toClassId);
                if (toClass == null)
                    return new BaseResponseDTO { Success = false, Message = "Destination class not found" };

                if (toClass.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "Destination class is inactive" };

                var toCount = await _classStudentRepository.CountStudentsInClassAsync(toClassId);
                if (toClass.MaxCapacity.HasValue && toCount >= toClass.MaxCapacity.Value)
                    return new BaseResponseDTO { Success = false, Message = "Destination class is full" };

                if (await _classStudentRepository.IsStudentInClassAsync(toClassId, studentId))
                    return new BaseResponseDTO { Success = false, Message = "Student is already in the destination class" };

                await _classStudentRepository.RemoveAsync(fromClassId, studentId);

                var fromClass = await _classRepository.GetByIdAsync(fromClassId);
                if (fromClass != null)
                {
                    fromClass.CurrentEnrollment = await _classStudentRepository.CountStudentsInClassAsync(fromClassId);
                    _classRepository.Update(fromClass);
                    await _classRepository.SaveChangesAsync();
                }

                await _classStudentRepository.EnrollAsync(new KmsClassStudent
                {
                    ClassId = toClassId,
                    StudentId = studentId,
                    EnrolledAt = DateTime.Now,
                    EnrolledBy = enrolledBy
                });

                toClass.CurrentEnrollment = await _classStudentRepository.CountStudentsInClassAsync(toClassId);
                _classRepository.Update(toClass);
                await _classRepository.SaveChangesAsync();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Student transferred from class {fromClassId} to class {toClassId} successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private ClassStudentDTO MapToStudentDTO(KmsClassStudent cs)
        {
            return new ClassStudentDTO
            {
                ClassId = cs.ClassId,
                ClassName = cs.Class?.ClassName ?? "",
                StudentId = cs.StudentId,
                StudentCode = cs.Student?.StudentCode ?? "",
                StudentName = cs.Student?.FullName ?? "",
                DateOfBirth = cs.Student?.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Gender = cs.Student?.Gender,
                EnrolledAt = cs.EnrolledAt ?? DateTime.Now,
                EnrolledByName = cs.EnrolledByNavigation?.FullName
            };
        }
    }
}
