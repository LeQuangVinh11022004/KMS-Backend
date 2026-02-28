
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Parent;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class ParentService : IParentService
    {
        private readonly IParentRepository _parentRepository;
        private readonly IUserRepository _userRepository;

        public ParentService(IParentRepository parentRepository, IUserRepository userRepository)
        {
            _parentRepository = parentRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResponseDTO> GetAllParentsAsync()
        {
            try
            {
                var parents = await _parentRepository.GetAllAsync();
                var parentDTOs = parents.Select(p => MapToDTO(p)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {parentDTOs.Count} parents",
                    Data = parentDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetParentByIdAsync(int parentId)
        {
            try
            {
                var parent = await _parentRepository.GetByIdAsync(parentId);
                if (parent == null)
                {
                    return new BaseResponseDTO { Success = false, Message = "Parent not found" };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Parent retrieved successfully",
                    Data = MapToDTO(parent)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> CreateParentAsync(CreateParentDTO dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    return new BaseResponseDTO { Success = false, Message = "User not found" };
                }

                if (await _parentRepository.UserIsParentAsync(dto.UserId))
                {
                    return new BaseResponseDTO { Success = false, Message = "User is already a parent" };
                }

                var parent = new KmsParent
                {
                    UserId = dto.UserId,
                    Occupation = dto.Occupation,
                    WorkAddress = dto.WorkAddress,
                    EmergencyContact = dto.EmergencyContact,
                    CreatedAt = DateTime.Now
                };

                var created = await _parentRepository.CreateAsync(parent);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Parent created successfully",
                    Data = MapToDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> UpdateParentAsync(int parentId, UpdateParentDTO dto)
        {
            try
            {
                var parent = await _parentRepository.GetByIdAsync(parentId);
                if (parent == null)
                {
                    return new BaseResponseDTO { Success = false, Message = "Parent not found" };
                }

                if (dto.Occupation != null) parent.Occupation = dto.Occupation;
                if (dto.WorkAddress != null) parent.WorkAddress = dto.WorkAddress;
                if (dto.EmergencyContact != null) parent.EmergencyContact = dto.EmergencyContact;

                await _parentRepository.UpdateAsync(parent);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Parent updated successfully",
                    Data = MapToDTO(parent)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> DeleteParentAsync(int parentId)
        {
            try
            {
                var parent = await _parentRepository.GetByIdAsync(parentId);
                if (parent == null)
                {
                    return new BaseResponseDTO { Success = false, Message = "Parent not found" };
                }

                await _parentRepository.DeleteAsync(parentId);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Parent deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetParentsByStudentIdAsync(int studentId)
        {
            try
            {
                var parents = await _parentRepository.GetParentsByStudentIdAsync(studentId);
                var parentDTOs = parents.Select(p => MapToDTO(p)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {parentDTOs.Count} parents",
                    Data = parentDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> SearchParentsAsync(string keyword)
        {
            try
            {
                var parents = await _parentRepository.SearchParentsAsync(keyword);
                var parentDTOs = parents.Select(p => MapToDTO(p)).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {parentDTOs.Count} parents",
                    Data = parentDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private ParentDTO MapToDTO(KmsParent parent)
        {
            return new ParentDTO
            {
                ParentId = parent.ParentId,
                UserId = parent.UserId,
                FullName = parent.User?.FullName ?? "",
                Email = parent.User?.Email,
                Phone = parent.User?.Phone,
                Occupation = parent.Occupation,
                WorkAddress = parent.WorkAddress,
                EmergencyContact = parent.EmergencyContact,
                CreatedAt = parent.CreatedAt ?? DateTime.Now
            };
        }
    }
}
