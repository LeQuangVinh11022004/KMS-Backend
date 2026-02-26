using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.TuitionTemplate;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class TuitionTemplateService : ITuitionTemplateService
    {
        private readonly ITuitionTemplateRepository _templateRepository;

        public TuitionTemplateService(ITuitionTemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<BaseResponseDTO> GetAllTemplatesAsync()
        {
            try
            {
                var templates = await _templateRepository.GetAllAsync();
                var dtos = templates.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} tuition templates",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetActiveTemplatesAsync()
        {
            try
            {
                var templates = await _templateRepository.GetActiveTemplatesAsync();
                var dtos = templates.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} active tuition templates",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetTemplateByIdAsync(int templateId)
        {
            try
            {
                var template = await _templateRepository.GetByIdWithItemsAsync(templateId);
                if (template == null)
                    return new BaseResponseDTO { Success = false, Message = "Tuition template not found" };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Tuition template retrieved successfully",
                    Data = MapToDTO(template)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // CREATE
        // ============================================================

        public async Task<BaseResponseDTO> CreateTemplateAsync(CreateTuitionTemplateDTO dto)
        {
            try
            {
                if (await _templateRepository.TemplateNameExistsAsync(dto.TemplateName))
                    return new BaseResponseDTO { Success = false, Message = "Template name already exists" };

                if (dto.BaseAmount < 0)
                    return new BaseResponseDTO { Success = false, Message = "Base amount cannot be negative" };

                var template = new KmsTuitionTemplate
                {
                    TemplateName = dto.TemplateName,
                    Description = dto.Description,
                    BaseAmount = dto.BaseAmount,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var created = await _templateRepository.CreateAsync(template);

                // Create items
                foreach (var itemDto in dto.Items)
                {
                    var item = new KmsTuitionItem
                    {
                        TemplateId = created.TemplateId,
                        ItemName = itemDto.ItemName,
                        Amount = itemDto.Amount,
                        ItemOrder = itemDto.ItemOrder
                    };
                    await _templateRepository.CreateItemAsync(item);
                }

                // Reload with items
                var result = await _templateRepository.GetByIdWithItemsAsync(created.TemplateId);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Tuition template created successfully",
                    Data = MapToDTO(result!)
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

        public async Task<BaseResponseDTO> UpdateTemplateAsync(int templateId, UpdateTuitionTemplateDTO dto)
        {
            try
            {
                var template = await _templateRepository.GetByIdWithItemsAsync(templateId);
                if (template == null)
                    return new BaseResponseDTO { Success = false, Message = "Tuition template not found" };

                if (dto.TemplateName != null)
                {
                    if (await _templateRepository.TemplateNameExistsAsync(dto.TemplateName, templateId))
                        return new BaseResponseDTO { Success = false, Message = "Template name already exists" };
                    template.TemplateName = dto.TemplateName;
                }

                if (dto.Description != null) template.Description = dto.Description;
                if (dto.BaseAmount.HasValue)
                {
                    if (dto.BaseAmount.Value < 0)
                        return new BaseResponseDTO { Success = false, Message = "Base amount cannot be negative" };
                    template.BaseAmount = dto.BaseAmount.Value;
                }
                if (dto.IsActive.HasValue) template.IsActive = dto.IsActive.Value;

                await _templateRepository.UpdateAsync(template);

                // Replace items if provided
                if (dto.Items != null)
                {
                    await _templateRepository.DeleteItemsByTemplateIdAsync(templateId);
                    foreach (var itemDto in dto.Items)
                    {
                        await _templateRepository.CreateItemAsync(new KmsTuitionItem
                        {
                            TemplateId = templateId,
                            ItemName = itemDto.ItemName,
                            Amount = itemDto.Amount,
                            ItemOrder = itemDto.ItemOrder
                        });
                    }
                }

                var result = await _templateRepository.GetByIdWithItemsAsync(templateId);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Tuition template updated successfully",
                    Data = MapToDTO(result!)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // DELETE
        // ============================================================

        public async Task<BaseResponseDTO> DeleteTemplateAsync(int templateId)
        {
            try
            {
                var template = await _templateRepository.GetByIdAsync(templateId);
                if (template == null)
                    return new BaseResponseDTO { Success = false, Message = "Tuition template not found" };

                // Prevent deletion if used by invoices
                if (await _templateRepository.IsUsedByInvoicesAsync(templateId))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Cannot delete template - it is being used by one or more invoices. Deactivate it instead."
                    };

                await _templateRepository.DeleteAsync(templateId);

                return new BaseResponseDTO { Success = true, Message = "Tuition template deleted successfully" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // TOGGLE STATUS
        // ============================================================

        public async Task<BaseResponseDTO> ToggleTemplateStatusAsync(int templateId)
        {
            try
            {
                var template = await _templateRepository.GetByIdAsync(templateId);
                if (template == null)
                    return new BaseResponseDTO { Success = false, Message = "Tuition template not found" };

                template.IsActive = !(template.IsActive ?? false);
                await _templateRepository.UpdateAsync(template);

                var status = template.IsActive == true ? "activated" : "deactivated";
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Tuition template {status} successfully",
                    Data = MapToDTO(template)
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

        private TuitionTemplateDTO MapToDTO(KmsTuitionTemplate t)
        {
            return new TuitionTemplateDTO
            {
                TemplateId = t.TemplateId,
                TemplateName = t.TemplateName,
                Description = t.Description,
                BaseAmount = t.BaseAmount,
                IsActive = t.IsActive ?? true,
                CreatedAt = t.CreatedAt ?? DateTime.Now,
                Items = t.KmsTuitionItems?.Select(i => new TuitionItemDTO
                {
                    ItemId = i.ItemId,
                    TemplateId = i.TemplateId,
                    ItemName = i.ItemName,
                    Amount = i.Amount,
                    ItemOrder = i.ItemOrder ?? 1
                }).ToList() ?? new()
            };
        }
    }
}
