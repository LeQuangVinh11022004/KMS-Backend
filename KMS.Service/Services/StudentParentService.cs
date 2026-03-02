using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.StudentParent;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class StudentParentService : IStudentParentService
    {
        private readonly IStudentParentRepository _studentParentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IParentRepository _parentRepository;

        private static readonly string[] ValidRelationships = ["Father", "Mother", "Guardian", "Other"];

        public StudentParentService(
            IStudentParentRepository studentParentRepository,
            IStudentRepository studentRepository,
            IParentRepository parentRepository)
        {
            _studentParentRepository = studentParentRepository;
            _studentRepository = studentRepository;
            _parentRepository = parentRepository;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<BaseResponseDTO> GetParentsByStudentAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };

                var records = await _studentParentRepository.GetParentsByStudentAsync(studentId);
                var dtos = records.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} parents for student '{student.FullName}'",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetStudentsByParentAsync(int parentId)
        {
            try
            {
                var parent = await _parentRepository.GetByIdAsync(parentId);
                if (parent == null)
                    return new BaseResponseDTO { Success = false, Message = "Parent not found" };

                var records = await _studentParentRepository.GetStudentsByParentAsync(parentId);
                var dtos = records.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} students for parent",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // LINK
        // ============================================================

        public async Task<BaseResponseDTO> LinkAsync(int studentId, LinkStudentParentDTO dto)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };

                var parent = await _parentRepository.GetByIdAsync(dto.ParentId);
                if (parent == null)
                    return new BaseResponseDTO { Success = false, Message = "Parent not found" };

                // Validate relationship
                if (!ValidRelationships.Contains(dto.Relationship))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Invalid relationship. Must be one of: {string.Join(", ", ValidRelationships)}"
                    };

                // Kiểm tra đã link chưa
                if (await _studentParentRepository.LinkExistsAsync(studentId, dto.ParentId))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "This parent is already linked to this student"
                    };

                // Nếu set IsPrimaryContact = true thì bỏ primary của người cũ
                if (dto.IsPrimaryContact)
                    await ClearPrimaryContactAsync(studentId);

                // Nếu chưa có primary contact nào thì tự động set người đầu tiên làm primary
                var hasPrimary = await _studentParentRepository.HasPrimaryContactAsync(studentId);
                var isPrimary = dto.IsPrimaryContact || !hasPrimary;

                var record = new KmsStudentParent
                {
                    StudentId = studentId,
                    ParentId = dto.ParentId,
                    Relationship = dto.Relationship,
                    IsPrimaryContact = isPrimary,
                    AssignedAt = DateTime.Now
                };

                var created = await _studentParentRepository.LinkAsync(record);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Parent linked to student successfully as {dto.Relationship}",
                    Data = MapToDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // UPDATE
        // ============================================================

        public async Task<BaseResponseDTO> UpdateLinkAsync(int studentId, int parentId, UpdateStudentParentDTO dto)
        {
            try
            {
                var record = await _studentParentRepository.GetAsync(studentId, parentId);
                if (record == null)
                    return new BaseResponseDTO { Success = false, Message = "Link not found" };

                if (dto.Relationship != null)
                {
                    if (!ValidRelationships.Contains(dto.Relationship))
                        return new BaseResponseDTO
                        {
                            Success = false,
                            Message = $"Invalid relationship. Must be one of: {string.Join(", ", ValidRelationships)}"
                        };
                    record.Relationship = dto.Relationship;
                }

                if (dto.IsPrimaryContact.HasValue)
                {
                    if (dto.IsPrimaryContact.Value)
                    {
                        // Bỏ primary của người cũ trước
                        await ClearPrimaryContactAsync(studentId);
                        record.IsPrimaryContact = true;
                    }
                    else
                    {
                        record.IsPrimaryContact = false;
                    }
                }

                await _studentParentRepository.UpdateAsync(record);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Link updated successfully",
                    Data = MapToDTO(record)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // UNLINK
        // ============================================================

        public async Task<BaseResponseDTO> UnlinkAsync(int studentId, int parentId)
        {
            try
            {
                var record = await _studentParentRepository.GetAsync(studentId, parentId);
                if (record == null)
                    return new BaseResponseDTO { Success = false, Message = "Link not found" };

                var wasPrimary = record.IsPrimaryContact == true;

                await _studentParentRepository.UnlinkAsync(studentId, parentId);

                // Nếu xóa primary contact thì tự động set người tiếp theo làm primary
                if (wasPrimary)
                {
                    var remaining = await _studentParentRepository.GetParentsByStudentAsync(studentId);
                    var next = remaining.FirstOrDefault();
                    if (next != null)
                    {
                        next.IsPrimaryContact = true;
                        await _studentParentRepository.UpdateAsync(next);
                    }
                }

                return new BaseResponseDTO { Success = true, Message = "Parent unlinked from student successfully" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private async Task ClearPrimaryContactAsync(int studentId)
        {
            var existing = await _studentParentRepository.GetParentsByStudentAsync(studentId);
            foreach (var sp in existing.Where(sp => sp.IsPrimaryContact == true))
            {
                sp.IsPrimaryContact = false;
                await _studentParentRepository.UpdateAsync(sp);
            }
        }

        private StudentParentDTO MapToDTO(KmsStudentParent sp)
        {
            return new StudentParentDTO
            {
                StudentId = sp.StudentId,
                StudentCode = sp.Student?.StudentCode ?? "",
                StudentName = sp.Student?.FullName ?? "",
                ParentId = sp.ParentId,
                ParentName = sp.Parent?.User?.FullName ?? "",
                ParentEmail = sp.Parent?.User?.Email,
                ParentPhone = sp.Parent?.User?.Phone,
                Relationship = sp.Relationship,
                IsPrimaryContact = sp.IsPrimaryContact == true,
                AssignedAt = sp.AssignedAt ?? DateTime.Now
            };
        }
    }
}