using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Invoice;
using KMS.Service.DTOs.Payment;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITuitionTemplateRepository _templateRepository;
        private readonly IStudentRepository _studentRepository;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            ITuitionTemplateRepository templateRepository,
            IStudentRepository studentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _templateRepository = templateRepository;
            _studentRepository = studentRepository;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<BaseResponseDTO> GetAllInvoicesAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllAsync();
                var dtos = invoices.Select(MapToDTO).ToList();
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} invoices",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetInvoiceByIdAsync(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Invoice retrieved successfully",
                    Data = MapToDTO(invoice)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetInvoicesByStudentAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };

                var invoices = await _invoiceRepository.GetByStudentIdAsync(studentId);
                var dtos = invoices.Select(MapToDTO).ToList();
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} invoices for student",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetFilteredInvoicesAsync(InvoiceFilterDTO filter)
        {
            try
            {
                // Convert DateTime? → DateOnly? để truyền vào repository
                DateOnly? monthFilter = filter.MonthYear.HasValue
                    ? new DateOnly(filter.MonthYear.Value.Year, filter.MonthYear.Value.Month, 1)
                    : null;

                var invoices = await _invoiceRepository.GetFilteredAsync(
                    filter.StudentId, filter.Status, monthFilter, filter.IsOverdue);

                var dtos = invoices.Select(MapToDTO).ToList();
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} invoices",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetOverdueInvoicesAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetOverdueInvoicesAsync();
                var dtos = invoices.Select(MapToDTO).ToList();
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} overdue invoices",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // CREATE (manual)
        // ============================================================

        public async Task<BaseResponseDTO> CreateInvoiceAsync(CreateInvoiceDTO dto, int createdBy)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(dto.StudentId);
                if (student == null)
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };

                // DateTime → DateOnly
                var monthDateOnly = new DateOnly(dto.MonthYear.Year, dto.MonthYear.Month, 1);

                if (await _invoiceRepository.InvoiceExistsForStudentMonthAsync(dto.StudentId, monthDateOnly))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Invoice already exists for this student in {dto.MonthYear:MM/yyyy}"
                    };

                if (dto.TotalAmount < 0 || dto.DiscountAmount < 0)
                    return new BaseResponseDTO { Success = false, Message = "Amounts cannot be negative" };

                if (dto.DiscountAmount > dto.TotalAmount)
                    return new BaseResponseDTO { Success = false, Message = "Discount cannot exceed total amount" };

                var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync();
                var finalAmount = dto.TotalAmount - dto.DiscountAmount;

                var invoice = new KmsInvoice
                {
                    InvoiceNumber = invoiceNumber,
                    StudentId = dto.StudentId,
                    TemplateId = dto.TemplateId,
                    MonthYear = monthDateOnly,
                    TotalAmount = dto.TotalAmount,
                    DiscountAmount = dto.DiscountAmount,
                    FinalAmount = finalAmount,
                    Status = "Unpaid",
                    DueDate = dto.DueDate.HasValue
                        ? new DateOnly(dto.DueDate.Value.Year, dto.DueDate.Value.Month, dto.DueDate.Value.Day)
                        : null,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                var created = await _invoiceRepository.CreateAsync(invoice);

                foreach (var itemDto in dto.Items)
                {
                    await _invoiceRepository.CreateItemAsync(new KmsInvoiceItem
                    {
                        InvoiceId = created.InvoiceId,
                        ItemName = itemDto.ItemName,
                        Amount = itemDto.Amount
                    });
                }

                var result = await _invoiceRepository.GetByIdWithDetailsAsync(created.InvoiceId);
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Invoice created successfully",
                    Data = MapToDTO(result!)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // CREATE FROM TEMPLATE
        // ============================================================

        public async Task<BaseResponseDTO> CreateInvoiceFromTemplateAsync(
            int studentId, int templateId, DateTime monthYear, int createdBy)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return new BaseResponseDTO { Success = false, Message = "Student not found" };

                var template = await _templateRepository.GetByIdWithItemsAsync(templateId);
                if (template == null)
                    return new BaseResponseDTO { Success = false, Message = "Tuition template not found" };

                if (template.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "Tuition template is inactive" };

                // DateTime → DateOnly
                var monthDateOnly = new DateOnly(monthYear.Year, monthYear.Month, 1);

                if (await _invoiceRepository.InvoiceExistsForStudentMonthAsync(studentId, monthDateOnly))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Invoice already exists for this student in {monthYear:MM/yyyy}"
                    };

                var totalAmount = template.KmsTuitionItems?.Sum(i => i.Amount) ?? template.BaseAmount;
                var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync();

                var invoice = new KmsInvoice
                {
                    InvoiceNumber = invoiceNumber,
                    StudentId = studentId,
                    TemplateId = templateId,
                    MonthYear = monthDateOnly,
                    TotalAmount = totalAmount,
                    DiscountAmount = 0,
                    FinalAmount = totalAmount,
                    Status = "Unpaid",
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                var created = await _invoiceRepository.CreateAsync(invoice);

                foreach (var item in template.KmsTuitionItems ?? new List<KmsTuitionItem>())
                {
                    await _invoiceRepository.CreateItemAsync(new KmsInvoiceItem
                    {
                        InvoiceId = created.InvoiceId,
                        ItemName = item.ItemName,
                        Amount = item.Amount
                    });
                }

                var result = await _invoiceRepository.GetByIdWithDetailsAsync(created.InvoiceId);
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Invoice created from template successfully",
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

        public async Task<BaseResponseDTO> UpdateInvoiceAsync(int invoiceId, UpdateInvoiceDTO dto, int updatedBy)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                if (invoice.Status == "Paid")
                    return new BaseResponseDTO { Success = false, Message = "Cannot edit a fully paid invoice" };

                if (invoice.Status == "Cancelled")
                    return new BaseResponseDTO { Success = false, Message = "Cannot edit a cancelled invoice" };

                if (dto.DiscountAmount.HasValue)
                {
                    if (dto.DiscountAmount.Value < 0)
                        return new BaseResponseDTO { Success = false, Message = "Discount cannot be negative" };
                    if (dto.DiscountAmount.Value > invoice.TotalAmount)
                        return new BaseResponseDTO { Success = false, Message = "Discount cannot exceed total amount" };

                    invoice.DiscountAmount = dto.DiscountAmount.Value;
                    invoice.FinalAmount = invoice.TotalAmount - dto.DiscountAmount.Value;
                }

                if (dto.Status != null)
                {
                    var validStatuses = new[] { "Unpaid", "Partial", "Paid", "Overdue", "Cancelled" };
                    if (!validStatuses.Contains(dto.Status))
                        return new BaseResponseDTO { Success = false, Message = "Invalid status value" };
                    invoice.Status = dto.Status;
                }

                if (dto.DueDate.HasValue)
                    invoice.DueDate = new DateOnly(dto.DueDate.Value.Year, dto.DueDate.Value.Month, dto.DueDate.Value.Day);

                if (dto.Notes != null) invoice.Notes = dto.Notes;

                invoice.UpdatedAt = DateTime.Now;
                invoice.UpdatedBy = updatedBy;

                await _invoiceRepository.UpdateAsync(invoice);

                var result = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId);
                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Invoice updated successfully",
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

        public async Task<BaseResponseDTO> DeleteInvoiceAsync(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                if (invoice.Status == "Paid")
                    return new BaseResponseDTO { Success = false, Message = "Cannot delete a paid invoice" };

                await _invoiceRepository.DeleteAsync(invoiceId);
                return new BaseResponseDTO { Success = true, Message = "Invoice deleted successfully" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // BATCH - Mark Overdue
        // ============================================================

        public async Task<BaseResponseDTO> MarkOverdueInvoicesAsync()
        {
            try
            {
                var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync();
                int count = 0;

                foreach (var invoice in overdueInvoices.Where(i => i.Status != "Overdue"))
                {
                    invoice.Status = "Overdue";
                    await _invoiceRepository.UpdateAsync(invoice);
                    count++;
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Marked {count} invoices as overdue"
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

        private InvoiceDTO MapToDTO(KmsInvoice i)
        {
            var payments = i.KmsPayments?.ToList() ?? new();
            var paidAmount = payments.Sum(p => p.PaidAmount);

            return new InvoiceDTO
            {
                InvoiceId = i.InvoiceId,
                InvoiceNumber = i.InvoiceNumber,
                StudentId = i.StudentId,
                StudentName = i.Student?.FullName ?? "",
                StudentCode = i.Student?.StudentCode,
                TemplateId = i.TemplateId,
                TemplateName = i.Template?.TemplateName,
                MonthYear = i.MonthYear.ToDateTime(TimeOnly.MinValue),
                TotalAmount = i.TotalAmount,
                DiscountAmount = i.DiscountAmount ?? 0,
                FinalAmount = i.FinalAmount,
                Status = i.Status ?? "Unpaid",
                DueDate = i.DueDate.HasValue
                    ? i.DueDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null,
                Notes = i.Notes,
                CreatedAt = i.CreatedAt ?? DateTime.Now,
                Items = i.KmsInvoiceItems?.Select(item => new InvoiceItemDTO
                {
                    InvoiceItemId = item.InvoiceItemId,
                    InvoiceId = item.InvoiceId,
                    ItemName = item.ItemName,
                    Amount = item.Amount
                }).ToList() ?? new(),
                Payments = payments.Select(p => new PaymentDTO
                {
                    PaymentId = p.PaymentId,
                    PaymentNumber = p.PaymentNumber,
                    InvoiceId = p.InvoiceId,
                    InvoiceNumber = i.InvoiceNumber,
                    StudentName = i.Student?.FullName ?? "",
                    PaidAmount = p.PaidAmount,
                    PaymentDate = p.PaymentDate.ToDateTime(TimeOnly.MinValue),
                    PaymentMethod = p.PaymentMethod,
                    TransactionReference = p.TransactionReference,
                    Notes = p.Notes,
                    ReceivedBy = p.ReceivedBy,
                    ReceivedByName = p.ReceivedByNavigation?.FullName ?? "",
                    CreatedAt = p.CreatedAt ?? DateTime.Now
                }).ToList(),
                PaidAmount = paidAmount,
                RemainingAmount = i.FinalAmount - paidAmount
            };
        }
    }
}