using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs
{
    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UserRoleDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
    }

    public class AssignRoleRequestDTO
    {
        public int RoleId { get; set; }
    }

    public class AssignRoleByNameRequestDTO
    {
        public string RoleName { get; set; } = string.Empty;
    }

    public class BaseResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
