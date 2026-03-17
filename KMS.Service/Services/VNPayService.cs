using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.VNPay;
using KMS.Service.Interfaces;
using KMS.Service.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace KMS.Service.Services
{
    public class VNPayService : IVNPayService
    {
        private readonly VNPaySettings _settings;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEmailService _emailService;

        public VNPayService(
            IOptions<VNPaySettings> settings,
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository,
            IEmailService emailService)
        {
            _settings = settings.Value;
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _emailService = emailService;
        }

        // ============================================================
        // TẠO URL THANH TOÁN
        // ============================================================

        public async Task<BaseResponseDTO> CreatePaymentUrlAsync(
            CreateVNPayPaymentDTO dto, int userId, string ipAddress)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                if (invoice.Status == "Paid")
                    return new BaseResponseDTO { Success = false, Message = "Hóa đơn này đã được thanh toán" };

                if (invoice.Status == "Cancelled")
                    return new BaseResponseDTO { Success = false, Message = "Hóa đơn đã bị hủy" };

                // OrderId = INV{invoiceId}_{timestamp} → lưu vào TransactionReference sau
                var orderId = $"INV{dto.InvoiceId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                var amount = (long)(invoice.FinalAmount * 100); // VNPay nhân 100
                var payDate = DateTime.Now.ToString("yyyyMMddHHmmss");

                // Build params theo đúng thứ tự alphabet (yêu cầu của VNPay)
                var vnpParams = new SortedDictionary<string, string>
                {
                    { "vnp_Amount",     amount.ToString() },
                    { "vnp_Command",    _settings.Command },
                    { "vnp_CreateDate", payDate },
                    { "vnp_CurrCode",   _settings.CurrCode },
                    { "vnp_IpAddr",     ipAddress },
                    { "vnp_Locale",     _settings.Locale },
                    { "vnp_OrderInfo",  dto.OrderInfo },
                    { "vnp_OrderType",  "other" },
                    { "vnp_ReturnUrl",  _settings.ReturnUrl },
                    { "vnp_TmnCode",    _settings.TmnCode },
                    { "vnp_TxnRef",     orderId },
                    { "vnp_Version",    _settings.Version },
                };

                if (!string.IsNullOrEmpty(dto.BankCode))
                    vnpParams.Add("vnp_BankCode", dto.BankCode);

                // Tạo chữ ký HMAC-SHA512
                var queryString = BuildQueryString(vnpParams);
                var secureHash = HmacSHA512(_settings.HashSecret, queryString);
                var paymentUrl = $"{_settings.BaseUrl}?{queryString}&vnp_SecureHash={secureHash}";

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Tạo URL thanh toán thành công",
                    Data = new VNPayPaymentUrlDTO
                    {
                        PaymentUrl = paymentUrl,
                        OrderId = orderId,
                        Amount = invoice.FinalAmount
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // XỬ LÝ CALLBACK TỪ VNPAY
        // Sau khi thanh toán xong → VNPay gọi ReturnUrl với các params
        // → Verify chữ ký → Tạo record trong KMS_Payments → Update Invoice
        // ============================================================

        public async Task<BaseResponseDTO> ProcessCallbackAsync(IQueryCollection queryParams)
        {
            try
            {
                // Tách secureHash ra khỏi params
                var vnpParams = new SortedDictionary<string, string>();
                foreach (var (key, value) in queryParams)
                {
                    if (key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                        vnpParams[key] = value.ToString();
                }

                var secureHash = queryParams["vnp_SecureHash"].ToString();

                // Verify chữ ký
                var queryString = BuildQueryString(vnpParams);
                var checkHash = HmacSHA512(_settings.HashSecret, queryString);

                if (!checkHash.Equals(secureHash, StringComparison.OrdinalIgnoreCase))
                    return new BaseResponseDTO { Success = false, Message = "Chữ ký không hợp lệ" };

                // Lấy thông tin từ callback
                var orderId = queryParams["vnp_TxnRef"].ToString();           // INV{invoiceId}_{timestamp}
                var responseCode = queryParams["vnp_ResponseCode"].ToString(); // "00" = thành công
                var vnpTransactionNo = queryParams["vnp_TransactionNo"].ToString();
                var bankCode = queryParams["vnp_BankCode"].ToString();
                var amountStr = queryParams["vnp_Amount"].ToString();

                // Parse invoiceId từ orderId: INV1_1234567890 → 1
                var invoiceId = ParseInvoiceId(orderId);
                if (invoiceId == 0)
                    return new BaseResponseDTO { Success = false, Message = "OrderId không hợp lệ" };

                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                var isSuccess = responseCode == "00";

                if (isSuccess && invoice.Status != "Paid")
                {
                    // Tạo Payment record trong KMS_Payments
                    var paymentNumber = $"VNPAY-{DateTime.Now:yyyyMMdd}-{vnpTransactionNo}";
                    var paidAmount = decimal.Parse(amountStr) / 100; // VNPay gửi x100

                    var payment = new KmsPayment
                    {
                        InvoiceId = invoiceId,
                        PaymentNumber = paymentNumber,
                        PaidAmount = paidAmount,
                        PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                        PaymentMethod = "VNPay",
                        TransactionReference = vnpTransactionNo, // Mã GD từ VNPay
                        Notes = $"VNPay | OrderId: {orderId} | Bank: {bankCode}",
                        ReceivedBy = invoice.CreatedBy ?? 1,
                        CreatedAt = DateTime.Now
                    };

                    var createdPayment = await _paymentRepository.CreateAsync(payment);

                    // Cập nhật Invoice → Paid
                    invoice.Status = "Paid";
                    invoice.UpdatedAt = DateTime.Now;
                    await _invoiceRepository.UpdateAsync(invoice);

                    // Lấy invoice kèm đầy đủ thông tin
                    var invoiceDetail = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId);

                    if (invoiceDetail != null)
                    {
                        // Lấy primary contact parent, fallback về contact đầu tiên
                        var primaryParent = invoiceDetail.Student.KmsStudentParents
                            .FirstOrDefault(sp => sp.IsPrimaryContact == true)
                            ?? invoiceDetail.Student.KmsStudentParents.FirstOrDefault();

                        var parentEmail = primaryParent?.Parent?.User?.Email;
                        var parentName = primaryParent?.Parent?.User?.FullName ?? "Quý phụ huynh";

                        if (!string.IsNullOrEmpty(parentEmail))
                        {
                            var invoiceItems = invoiceDetail.KmsInvoiceItems
                                .Select(x => (x.ItemName, x.Amount))
                                .ToList();

                            await _emailService.SendInvoiceEmailAsync(
                                toEmail: parentEmail,
                                parentName: parentName,
                                studentName: invoiceDetail.Student.FullName,
                                invoiceNumber: invoiceDetail.InvoiceNumber,
                                monthYear: invoiceDetail.MonthYear.ToDateTime(TimeOnly.MinValue),
                                items: invoiceItems,
                                totalAmount: invoiceDetail.TotalAmount,
                                discountAmount: invoiceDetail.DiscountAmount ?? 0,
                                finalAmount: invoiceDetail.FinalAmount,
                                vnpTransactionNo: vnpTransactionNo,
                                orderId: orderId,
                                bankCode: bankCode,
                                paymentDate: DateTime.Now
                            );
                        }
                    }

                    return new BaseResponseDTO
                    {
                        Success = true,
                        Message = "Thanh toán thành công",
                        Data = new VNPayCallbackResultDTO
                        {
                            Success = true,
                            Message = "Thanh toán thành công",
                            VnpTransactionNo = vnpTransactionNo,
                            OrderId = orderId,
                            Amount = paidAmount,
                            BankCode = bankCode,
                            InvoiceId = invoiceId,
                            InvoiceStatus = "Paid",
                            PaymentId = createdPayment.PaymentId
                        }
                    };
                }

                // Thanh toán thất bại
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Thanh toán thất bại (Code: {responseCode})",
                    Data = new VNPayCallbackResultDTO
                    {
                        Success = false,
                        Message = $"Thanh toán thất bại (Code: {responseCode})",
                        VnpTransactionNo = vnpTransactionNo,
                        OrderId = orderId,
                        InvoiceId = invoiceId,
                        InvoiceStatus = invoice.Status
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private static int ParseInvoiceId(string orderId)
        {
            // Format: INV{invoiceId}_{timestamp} → lấy phần giữa INV và _
            try
            {
                var withoutPrefix = orderId.Replace("INV", "");
                var parts = withoutPrefix.Split('_');
                return int.TryParse(parts[0], out var id) ? id : 0;
            }
            catch { return 0; }
        }

        private static string HmacSHA512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private static string BuildQueryString(SortedDictionary<string, string> parameters)
        {
            var query = new StringBuilder();
            foreach (var (key, value) in parameters)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (query.Length > 0) query.Append('&');
                    query.Append(WebUtility.UrlEncode(key));
                    query.Append('=');
                    query.Append(WebUtility.UrlEncode(value));
                }
            }
            return query.ToString();
        }
    }
}