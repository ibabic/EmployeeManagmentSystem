using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Constants = ServerLibrary.Helpers.Constants;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext, ILogger<UserAccountRepository> logger) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var methodName = nameof(CreateAsync);
            logger.LogInformation($"[{methodName}] Attempting to create a new user...");
            if (user is null)
            {  
                logger.LogError($"[{methodName}] Model is empty");
                return new GeneralResponse(false, "Model is empty");
            } 

            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                var errorMessage = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                logger.LogError($"[{methodName}] Validation failed: {errorMessage}");
                return new GeneralResponse(false, $"Validation failed: {errorMessage}");
            }

            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser is not null) return new GeneralResponse(false, "User registered already");

            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                FullName = user.FullName,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
                return new GeneralResponse(true, "Account Created!");
            }

            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
            }
            logger.LogInformation($"[{methodName}] New user created");
            return new GeneralResponse(true, "Account Created!");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            var methodName = nameof(SignInAsync);
            logger.LogInformation($"[{methodName}] Attempting to sign in user...");

            if (user is null)
            {
                logger.LogError($"[{methodName}] Model is empty");
                return new LoginResponse(false, "Model is empty");
            }

            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                var errorMessage = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                logger.LogError($"[{methodName}] Validation failed: {errorMessage}");
                return new LoginResponse(false, $"Validation failed: {errorMessage}");
            }

            var applicationUser = await FindUserByEmail(user.Email);
            if (applicationUser is null)
            {
                logger.LogError($"[{methodName}] User not found");
                return new LoginResponse(false, "User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
            {
                logger.LogError($"[{methodName}] Email/Password not valid");
                return new LoginResponse(false, "Email/Password not valid");
            }

            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null)
            {
                logger.LogError($"[{methodName}] User role not found");
                return new LoginResponse(false, "User role not found");
            }

            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName is null)
            {
                logger.LogError($"[{methodName}] User role not found");
                return new LoginResponse(false, "User role not found");
            }

            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name);
            string refreshToken = GenerateRefreshToken();

            var findUser = await appDbContext.RefreshTokens.FirstOrDefaultAsync(u => u.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                findUser!.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken, UserId = applicationUser.Id });
            }

            logger.LogInformation($"[{methodName}] User successfully signed in");
            return new LoginResponse(true, "Login successfully", jwtToken, refreshToken);
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role!)
            };

            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserRole> FindUserRole(int userId) => await appDbContext.UserRoles.FirstOrDefaultAsync(r => r.UserId == userId);

        private async Task<SystemRole> FindRoleName(int roleId) => await appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Id == roleId);

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<ApplicationUser?> FindUserByEmail(string email)
        {
            if (email == null)
                return null;

            var normalizedEmail = email.ToLower();
            return await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
        }

        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            var methodName = nameof(RefreshTokenAsync);
            logger.LogInformation($"[{methodName}] Attempting to refresh token...");

            if (token is null)
            {
                logger.LogError($"[{methodName}] Model is empty");
                return new LoginResponse(false, "Model is empty");
            }

            var findToken = await appDbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token!.Equals(token.Token));
            if (findToken is null)
            {
                logger.LogError($"[{methodName}] Refresh token is required");
                return new LoginResponse(false, "Refresh token is required");
            }

            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == findToken.UserId);
            if (user is null)
            {
                logger.LogError($"[{methodName}] Refresh token could not be generated because user not found");
                return new LoginResponse(false, "Refresh token could not be generated because user not found");
            }

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.RoleId);
            string jwtToken = GenerateToken(user, roleName!.Name);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDbContext.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (updateRefreshToken is null)
            {
                logger.LogError($"[{methodName}] Refresh token could not be generated because user has not signed in");
                return new LoginResponse(false, "Refresh token could not be generated because user has not signed in");
            }

            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();

            logger.LogInformation($"[{methodName}] Token refreshed successfully");
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
        }

        public async Task<List<ManageUser>> GetUsers()
        {
            var methodName = nameof(GetUsers);
            logger.LogInformation($"[{methodName}] Attempting to retrieve all users");

            var allUsers = await GetApplicationUsers();
            var allUserRoles = await UserRoles();
            var allRoles = await SystemRoles();

            if (allUsers.Count == 0 || allRoles.Count == 0)
            {
                logger.LogWarning($"[{methodName}] No users or roles found");
                return null!;
            }

            var users = new List<ManageUser>();
            foreach (var user in allUsers)
            {
                var userRole = allUserRoles.FirstOrDefault(u => u.UserId == user.Id);
                var roleName = allRoles.FirstOrDefault(r => r.Id == userRole!.RoleId);
                users.Add(new ManageUser() { UserId = user.Id, Name = user.FullName, Email = user.Email, Role = roleName!.Name });
            }

            logger.LogInformation($"[{methodName}] Successfully retrieved all users");
            return users;
        }

        public async Task<UserProfile> GetUserProfileAsync(int id)
        {
            var methodName = nameof(GetUserProfileAsync);
            logger.LogInformation($"[{methodName}] Retrieving user profile for ID: {id}");

            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                logger.LogError($"[{methodName}] User with ID {id} not found");
                return null!;
            }

            var userRole = await FindUserRole(id);
            if (userRole == null)
            {
                logger.LogWarning($"[{methodName}] No roles found for user with ID {id}");
                return null!;
            }

            var roleName = await FindRoleName(userRole.RoleId);

            var userProfile = new UserProfile
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = roleName!.Name
            };

            logger.LogInformation($"[{methodName}] User profile retrieved successfully for ID: {id}");
            return userProfile;
        }

        public async Task<GeneralResponse> UpdateUserProfile(UserProfile user)
        {
            var methodName = nameof(UpdateUserProfile);
            logger.LogInformation($"[{methodName}] Attempting to update user profile...");

            if (user == null)
            {
                logger.LogError($"[{methodName}] Model is empty");
                return new GeneralResponse(false, "Model is empty");
            }

            var userEntity = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (userEntity == null)
            {
                logger.LogError($"[{methodName}] User not found");
                return new GeneralResponse(false, "User not found");
            }

            userEntity.FullName = user.FullName;
            userEntity.Email = user.Email;

            if (!string.IsNullOrEmpty(user.Password))
            {
                userEntity.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            await appDbContext.SaveChangesAsync();

            logger.LogInformation($"[{methodName}] User profile updated successfully");
            return new GeneralResponse(true, "User profile updated successfully");
        }

        public async Task<GeneralResponse> UpdateUser(ManageUser user)
        {
            var methodName = nameof(UpdateUser);
            logger.LogInformation($"[{methodName}] Attempting to update user role...");

            if (user is null)
            {
                logger.LogError($"[{methodName}] Model is empty");
                return new GeneralResponse(false, "Model is empty");
            }

            var getRole = (await SystemRoles()).FirstOrDefault(r => r.Name!.Equals(user.Role));
            if (getRole is null)
            {
                logger.LogError($"[{methodName}] Role '{user.Role}' not found");
                return new GeneralResponse(false, $"Role '{user.Role}' not found");
            }

            var userRole = await appDbContext.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (userRole is null)
            {
                logger.LogError($"[{methodName}] User role not found for user ID: {user.UserId}");
                return new GeneralResponse(false, $"User role not found for user ID: {user.UserId}");
            }

            userRole.RoleId = getRole.Id;
            await appDbContext.SaveChangesAsync();

            logger.LogInformation($"[{methodName}] User role updated successfully");
            return new GeneralResponse(true, "User role updated successfully");
        }

        public async Task<List<SystemRole>> GetRoles() => await SystemRoles();

        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var methodName = nameof(DeleteUser);
            logger.LogInformation($"[{methodName}] Attempting to delete user with ID: {id}");

            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                logger.LogError($"[{methodName}] User with ID {id} not found");
                return new GeneralResponse(false, $"User with ID {id} not found");
            }

            appDbContext.ApplicationUsers.Remove(user);
            await appDbContext.SaveChangesAsync();

            logger.LogInformation($"[{methodName}] User with ID {id} successfully deleted");
            return new GeneralResponse(true, "User successfully deleted");
        }

        public async Task<List<SystemRole>> SystemRoles() => await appDbContext.SystemRoles.AsNoTracking().ToListAsync();
        public async Task<List<UserRole>> UserRoles() => await appDbContext.UserRoles.AsNoTracking().ToListAsync();
        public async Task<List<ApplicationUser>> GetApplicationUsers() => await appDbContext.ApplicationUsers.AsNoTracking().ToListAsync();
    }
}
