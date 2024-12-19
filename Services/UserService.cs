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
				Fullname = "Pu Tong",
				Role = "SuperAdmin",
				Password = "123456"
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
			var resp = await _userRepository.GetUserByEmail(request.Email);
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
				Email = request.Email,
				PasswordHash = EncryptionUtils.HashPassword(request.Password),
				Fullname = request.Fullname,
				Role = request.Role,
				CreatedAt = currentTimestamp,
				UpdatedAt = currentTimestamp,
				ActivateToken = activationCode,
				ActivateTokenGeneratedAt = currentTimestamp,
				ActivateTokenExpiresAt = activateTokenExpiresAt,
				IsActive = true
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
				Fullname = user.Fullname,
				Role = user.Role
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