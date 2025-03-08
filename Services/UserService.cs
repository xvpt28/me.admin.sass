using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using me.admin.api.Utils;
using Serilog;

namespace me.admin.api.Services;

public class UserService(UserRepository userRepository)
{
    readonly UserRepository _userRepository = userRepository;

    public async Task<BaseResponse<bool>> Initialization()
    {
        try
        {
            var user = new CreateUserDto
            {
                Email = "xvpt28@gmail.com",
                FullName = "Pu Tong",
                Role = "SuperAdmin",
                Password = "123456",
            };
            await CreateUser(user);
            return new BaseResponse<bool> { Success = true, Data = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error initializing user");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> CreateUser(CreateUserDto request)
    {
        try
        {
            var resp = await _userRepository.GetUserByEmail(request.Email.ToLower());
            if (resp != null)
            {
                throw new Exception("Email already exists");
            }

            var activationCode = VerificationUtils.GenerateVerificationCode(6);
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var activateTokenExpiresAt = currentTimestamp + 30000; // 5 minutes
            var entity = new User
            {
                UserId = Ulid.NewUlid().ToString(),
                Email = request.Email.ToLower(),
                PasswordHash = EncryptionUtils.HashPassword(request.Password),
                FullName = request.FullName,
                Role = request.Role,
                Phone = request.Phone,
                CreatedAt = currentTimestamp,
                UpdatedAt = currentTimestamp,
                ActivateToken = activationCode,
                ActivateTokenGeneratedAt = currentTimestamp,
                ActivateTokenExpiresAt = activateTokenExpiresAt,
                IsActive = true,
            };
            await _userRepository.Insert(entity);
            return new BaseResponse<bool> { Success = true, Data = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating user");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> UpdateUser(string userId, UpdateUserDto request)
    {
        try
        {
            var resp = await _userRepository.GetById(userId);
            if (resp == null)
            {
                throw new Exception("User not found");
            }
            resp.FullName = request.FullName ?? resp.FullName;
            resp.Role = request.Role ?? resp.Role;
            resp.IsActive = request.IsActive ?? resp.IsActive;
            resp.Email = request.Email ?? resp.Email;
            resp.Phone = request.Phone ?? resp.Phone;
            resp.UpdatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await _userRepository.Update(resp);
            return new BaseResponse<bool> { Success = true, Data = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating user");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> UpdateMyPassword(
        string userId,
        UpdateUserPasswordDto request
    )
    {
        try
        {
            var resp = await _userRepository.GetById(userId);
            if (resp == null)
                throw new Exception("User not found");

            if (!EncryptionUtils.VerifyPassword(request.CurrentPassword, resp.PasswordHash))
                throw new Exception("Invalid username or password");

            resp.PasswordHash = EncryptionUtils.HashPassword(request.Password);
            resp.UpdatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await _userRepository.Update(resp);
            return new BaseResponse<bool> { Success = true, Data = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating user password");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<List<GetUserDto>>> GetAllUsers()
    {
        try
        {
            var resp = await _userRepository.GetAll();
            var users = resp.Select(x => new GetUserDto
            {
                UserId = x.UserId,
                Email = x.Email,
                FullName = x.FullName,
                Role = x.Role,
                Phone = x.Phone,
            });
            return new BaseResponse<List<GetUserDto>> { Success = true, Data = users.ToList() };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error getting all users");
            return new BaseResponse<List<GetUserDto>> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> DeleteUser(string userId)
    {
        try
        {
            await _userRepository.Delete(userId);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error deleting user");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<GetUserDto>> GetUserById(string userId)
    {
        try
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var resp = new GetUserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Phone = user.Phone,
            };
            return new BaseResponse<GetUserDto>(resp) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error getting user by id");
            return new BaseResponse<GetUserDto> { Success = false, Message = e.Message };
        }
    }
}
