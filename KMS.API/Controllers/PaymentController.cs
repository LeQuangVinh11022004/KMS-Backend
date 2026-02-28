using KMS.Service.DTOs.Payment;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>Get all payments</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Get payment by ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get all payments for an invoice</summary>
        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoice(int invoiceId)
        {
            var result = await _paymentService.GetPaymentsByInvoiceAsync(invoiceId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Filter payments. Query params: invoiceId, paymentMethod, fromDate, toDate (YYYY-MM-DD)
        /// </summary>
        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] int? invoiceId,
            [FromQuery] string? paymentMethod,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var filter = new PaymentFilterDTO
            {
                InvoiceId = invoiceId,
                PaymentMethod = paymentMethod,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _paymentService.GetFilteredPaymentsAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Record a payment for an invoice. Auto-updates invoice status (Partial / Paid).
        /// PaymentMethod: Cash | Bank Transfer | E-Wallet | Credit Card | Debit Card
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.CreatePaymentAsync(dto);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = ((PaymentDTO)result.Data).PaymentId }, result)
                : BadRequest(result);
        }

        /// <summary>Delete payment and recalculate invoice status</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Revenue summary by date range.
        /// Query params: fromDate, toDate (YYYY-MM-DD). Defaults to current month.
        /// </summary>
        [HttpGet("revenue-summary")]
        public async Task<IActionResult> GetRevenueSummary(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _paymentService.GetRevenueSummaryAsync(fromDate, toDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
