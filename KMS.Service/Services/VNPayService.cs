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

        // TẠO URL THANH TOÁN

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

                var orderId = $"INV{dto.InvoiceId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                var amount = (long)(invoice.FinalAmount * 100);
                var payDate = DateTime.Now.ToString("yyyyMMddHHmmss");

                var returnUrl = string.IsNullOrWhiteSpace(dto.ReturnUrl)
                    ? _settings.ReturnUrl
                    : dto.ReturnUrl.Trim();

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
                    { "vnp_ReturnUrl",  returnUrl },
                    { "vnp_TmnCode",    _settings.TmnCode },
                    { "vnp_TxnRef",     orderId },
                    { "vnp_Version",    _settings.Version },
                };

                if (!string.IsNullOrEmpty(dto.BankCode))
                    vnpParams.Add("vnp_BankCode", dto.BankCode);

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

        // XỬ LÝ CALLBACK TỪ VNPAY

        public async Task<BaseResponseDTO> ProcessCallbackAsync(IQueryCollection queryParams)
        {
            try
            {
                if (!queryParams.ContainsKey("vnp_TxnRef") || !queryParams.ContainsKey("vnp_ResponseCode"))
                    return new BaseResponseDTO { Success = false, Message = "Missing required VNPAY parameters" };

                if (!queryParams.ContainsKey("vnp_SecureHash"))
                    return new BaseResponseDTO { Success = false, Message = "Missing secure hash" };

                var vnpParams = new SortedDictionary<string, string>();
                foreach (var (key, value) in queryParams)
                {
                    if (key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                        vnpParams[key] = value.ToString();
                }

                var secureHash = queryParams["vnp_SecureHash"].ToString();
                var queryString = BuildQueryString(vnpParams);
                var checkHash = HmacSHA512(_settings.HashSecret, queryString);

                if (!checkHash.Equals(secureHash, StringComparison.OrdinalIgnoreCase))
                    return new BaseResponseDTO { Success = false, Message = "Chữ ký không hợp lệ" };

                var orderId = queryParams["vnp_TxnRef"].ToString();
                var responseCode = queryParams["vnp_ResponseCode"].ToString();
                var vnpTransactionNo = queryParams.ContainsKey("vnp_TransactionNo") ? queryParams["vnp_TransactionNo"].ToString() : "";
                var bankCode = queryParams.ContainsKey("vnp_BankCode") ? queryParams["vnp_BankCode"].ToString() : "";
                var amountStr = queryParams.ContainsKey("vnp_Amount") ? queryParams["vnp_Amount"].ToString() : "0";

                var invoiceId = ParseInvoiceId(orderId);
                if (invoiceId == 0)
                    return new BaseResponseDTO { Success = false, Message = "OrderId không hợp lệ" };

                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                var isSuccess = responseCode == "00";

                if (isSuccess && invoice.Status != "Paid")
                {
                    var paymentNumber = $"VNPAY-{DateTime.Now:yyyyMMdd}-{vnpTransactionNo}";
                    var paidAmount = decimal.Parse(amountStr) / 100;

                    var payment = new KmsPayment
                    {
                        InvoiceId = invoiceId,
                        PaymentNumber = paymentNumber,
                        PaidAmount = paidAmount,
                        PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                        PaymentMethod = "VNPay",
                        TransactionReference = vnpTransactionNo,
                        Notes = $"VNPay | OrderId: {orderId} | Bank: {bankCode}",
                        ReceivedBy = invoice.CreatedBy ?? 1,
                        CreatedAt = DateTime.Now
                    };

                    var createdPayment = await _paymentRepository.CreateAsync(payment);

                    invoice.Status = "Paid";
                    invoice.UpdatedAt = DateTime.Now;
                    await _invoiceRepository.UpdateAsync(invoice);

                    var invoiceDetail = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId);

                    if (invoiceDetail != null)
                    {
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

        // HELPERS

        private static int ParseInvoiceId(string orderId)
        {
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