using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.ClassTeacher;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class ClassTeacherService : IClassTeacherService
    {
        private readonly IClassTeacherRepository _classTeacherRepository;
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;

        public ClassTeacherService(
            IClassTeacherRepository classTeacherRepository,
            IClassRepository classRepository,
            ITeacherRepository teacherRepository)
        {
            _classTeacherRepository = classTeacherRepository;
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<BaseResponseDTO> GetTeachersByClassAsync(int classId)
        {
            try
            {
                var kmsClass = await _classRepository.GetByIdAsync(classId);
                if (kmsClass == null)
                    return new BaseResponseDTO { Success = false, Message = "Class not found" };

                var records = await _classTeacherRepository.GetTeachersByClassAsync(classId);
                var dtos = records.Select(MapToTeacherDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} teachers in class",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetClassesByTeacherAsync(int teacherId)
        {
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(teacherId);
                if (teacher == null)
                    return new BaseResponseDTO { Success = false, Message = "Teacher not found" };

                var records = await _classTeacherRepository.GetClassesByTeacherAsync(teacherId);
                var dtos = records.Select(MapToTeacherDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} classes for teacher",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> AssignTeacherAsync(int classId, AssignTeacherDTO dto, int assignedBy)
        {
            try
            {
                var kmsClass = await _classRepository.GetByIdAsync(classId);
                if (kmsClass == null)
                    return new BaseResponseDTO { Success = false, Message = "Class not found" };

                if (kmsClass.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "Class is inactive" };

                var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
                if (teacher == null)
                    return new BaseResponseDTO { Success = false, Message = "Teacher not found" };

                if (teacher.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "Teacher is inactive" };

                if (await _classTeacherRepository.IsTeacherInClassAsync(classId, dto.TeacherId))
                    return new BaseResponseDTO { Success = false, Message = "Teacher is already assigned to this class" };

                var validRoles = new[] { "Main", "Assistant" };
                if (!validRoles.Contains(dto.Role))
                    return new BaseResponseDTO { Success = false, Message = "Invalid role. Must be 'Main' or 'Assistant'" };

                if (dto.Role == "Main" && await _classTeacherRepository.ClassHasMainTeacherAsync(classId))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Class already has a Main teacher. Remove or change the existing Main teacher first."
                    };

                var record = new KmsClassTeacher
                {
                    ClassId = classId,
                    TeacherId = dto.TeacherId,
                    Role = dto.Role,
                    AssignedAt = DateTime.Now,
                    AssignedBy = assignedBy
                };

                var created = await _classTeacherRepository.AssignAsync(record);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Teacher '{teacher.User?.FullName}' assigned as {dto.Role} successfully",
                    Data = MapToTeacherDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> UpdateTeacherRoleAsync(
            int classId, int teacherId, UpdateTeacherRoleDTO dto)
        {
            try
            {
                var record = await _classTeacherRepository.GetAsync(classId, teacherId);
                if (record == null)
                    return new BaseResponseDTO { Success = false, Message = "Teacher is not assigned to this class" };

                var validRoles = new[] { "Main", "Assistant" };
                if (!validRoles.Contains(dto.Role))
                    return new BaseResponseDTO { Success = false, Message = "Invalid role. Must be 'Main' or 'Assistant'" };

                if (dto.Role == "Main" && record.Role != "Main")
                {
                    if (await _classTeacherRepository.ClassHasMainTeacherAsync(classId))
                        return new BaseResponseDTO
                        {
                            Success = false,
                            Message = "Class already has a Main teacher. Remove or change the existing Main teacher first."
                        };
                }

                record.Role = dto.Role;
                await _classTeacherRepository.UpdateAsync(record);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Teacher role updated to '{dto.Role}' successfully",
                    Data = MapToTeacherDTO(record)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> RemoveTeacherFromClassAsync(int classId, int teacherId)
        {
            try
            {
                var record = await _classTeacherRepository.GetAsync(classId, teacherId);
                if (record == null)
                    return new BaseResponseDTO { Success = false, Message = "Teacher is not assigned to this class" };

                await _classTeacherRepository.RemoveAsync(classId, teacherId);

                return new BaseResponseDTO { Success = true, Message = "Teacher removed from class successfully" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private ClassTeacherDTO MapToTeacherDTO(KmsClassTeacher ct)
        {
            return new ClassTeacherDTO
            {
                ClassId = ct.ClassId,
                ClassName = ct.Class?.ClassName ?? "",
                TeacherId = ct.TeacherId,
                TeacherCode = ct.Teacher?.TeacherCode,
                TeacherName = ct.Teacher?.User?.FullName ?? "",
                Email = ct.Teacher?.User?.Email,
                Specialization = ct.Teacher?.Specialization,
                Role = ct.Role ?? "Main",
                AssignedAt = ct.AssignedAt ?? DateTime.Now,
                AssignedByName = ct.AssignedByNavigation?.FullName
            };
        }
    }
}
