using KMS.Service.DTOs;
using KMS.Service.DTOs.Invoice;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    /// <summary>Get all invoices</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _invoiceService.GetAllInvoicesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Get invoice by ID (includes items + payments)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _invoiceService.GetInvoiceByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Get invoices by student</summary>
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudent(int studentId)
    {
        var result = await _invoiceService.GetInvoicesByStudentAsync(studentId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Get overdue invoices</summary>
    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var result = await _invoiceService.GetOverdueInvoicesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Filter invoices. Query params: studentId, status, monthYear (YYYY-MM), isOverdue
    /// </summary>
    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] int? studentId,
        [FromQuery] string? status,
        [FromQuery] string? monthYear,    // "2024-01"
        [FromQuery] bool? isOverdue)
    {
        DateTime? parsedMonth = null;
        if (!string.IsNullOrEmpty(monthYear) && DateTime.TryParse(monthYear + "-01", out var parsed))
            parsedMonth = parsed;

        var filter = new InvoiceFilterDTO
        {
            StudentId = studentId,
            Status = status,
            MonthYear = parsedMonth,
            IsOverdue = isOverdue
        };

        var result = await _invoiceService.GetFilteredInvoicesAsync(filter);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Create invoice manually</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDTO dto, [FromQuery] int createdBy)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.CreateInvoiceAsync(dto, createdBy);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = ((InvoiceDTO)result.Data).InvoiceId }, result)
            : BadRequest(result);
    }

    /// <summary>Create invoice from tuition template</summary>
    [HttpPost("from-template")]
    public async Task<IActionResult> CreateFromTemplate(
        [FromQuery] int studentId,
        [FromQuery] int templateId,
        [FromQuery] string monthYear,     // "2024-01"
        [FromQuery] int createdBy)
    {
        if (!DateTime.TryParse(monthYear + "-01", out var parsedMonth))
            return BadRequest(new BaseResponseDTO { Success = false, Message = "Invalid monthYear format. Use YYYY-MM" });

        var result = await _invoiceService.CreateInvoiceFromTemplateAsync(studentId, templateId, parsedMonth, createdBy);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = ((InvoiceDTO)result.Data).InvoiceId }, result)
            : BadRequest(result);
    }

    /// <summary>Update invoice (discount, status, due date, notes)</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceDTO dto, [FromQuery] int updatedBy)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.UpdateInvoiceAsync(id, dto, updatedBy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Delete invoice (blocked if already paid)</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _invoiceService.DeleteInvoiceAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Batch mark all expired unpaid invoices as Overdue</summary>
    [HttpPost("mark-overdue")]
    public async Task<IActionResult> MarkOverdue()
    {
        var result = await _invoiceService.MarkOverdueInvoicesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
