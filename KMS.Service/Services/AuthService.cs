using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.Interfaces;
using KMS.Service.DTOs.Login;
using KMS.Service.DTOs.Register;
using KMS.Service.DTOs.User;

namespace KMS.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            try
            {
                // 1. Find user by username
                var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);

                if (user == null)
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // 2. Check if user is active
                if (user.IsActive != true)
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Your account has been deactivated"
                    };
                }

                // 3. Verify password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // 4. Get user roles
                var roles = await _unitOfWork.Users.GetUserRolesAsync(user.UserId);

                // 5. Generate JWT token
                var token = GenerateJwtToken(user, roles);

                // 6. Update last login
                await _unitOfWork.Users.UpdateLastLoginAsync(user.UserId);

                // 7. Return success response
                return new LoginResponseDTO
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new UserDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        Avatar = user.Avatar,
                        IsActive = user.IsActive == true,
                        Roles = roles.ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            try
            {
                // 1. Check if username exists
                if (await _unitOfWork.Users.UsernameExistsAsync(request.Username))
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                // 2. Check if email exists
                if (!string.IsNullOrEmpty(request.Email))
                {
                    if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                    {
                        return new LoginResponseDTO
                        {
                            Success = false,
                            Message = "Email already exists"
                        };
                    }
                }

                // 3. Hash password using BCrypt
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // 4. Create new user
                var newUser = new KmsUser
                {
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    IsActive = false,  // ← PENDING - Chờ Admin duyệt
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _unitOfWork.Users.CreateAsync(newUser);

                return new LoginResponseDTO
                {
                    Success = true,
                    Message = "Registration successful. Your account is pending approval by administrator."
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return null;

            var roles = await _unitOfWork.Users.GetUserRolesAsync(userId);

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Avatar = user.Avatar,
                IsActive = user.IsActive == true,
                Roles = roles.ToList()
            };
        }

        private string GenerateJwtToken(KmsUser user, IEnumerable<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "KMS_API";
            var audience = jwtSettings["Audience"] ?? "KMS_Client";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "1440");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}