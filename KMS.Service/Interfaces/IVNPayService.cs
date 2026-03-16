using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.VNPay;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IVNPayService
    {
        // Tạo URL thanh toán → Parent mở URL này để thanh toán
        Task<BaseResponseDTO> CreatePaymentUrlAsync(CreateVNPayPaymentDTO dto, int userId, string ipAddress);

        // VNPay callback sau khi thanh toán → tự động tạo Payment + cập nhật Invoice
        Task<BaseResponseDTO> ProcessCallbackAsync(IQueryCollection queryParams);
    }
}
