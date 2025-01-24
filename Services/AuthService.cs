using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace me.admin.api.Services;

public class AuthService(
	UserRepository userRepository,
	IOptions<AuthorizationSetting> authorizationSettings,
	IMapper mapper,
	IHttpContextAccessor httpContextAccessor
)
{
	readonly AuthorizationSetting _authorizationSettings = authorizationSettings.Value;
	readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
	readonly IMapper _mapper = mapper;
	readonly UserRepository _userRepository = userRepository;

	public string? GetUserId()
	{
		if (
			_httpContextAccessor.HttpContext != null
			&& _httpContextAccessor.HttpContext.User.Identity != null
			)
		{
			var userId = _httpContextAccessor
				.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")
				?.Value;
			return userId;
		}

		throw new Exception("User not authenticated");
	}

	public async Task<BaseResponse<LoginTokenDto>> Login(LoginRequestDto loginRequest)
	{
		try
		{
			var user = await _userRepository.GetUserByEmail(loginRequest.Email);

			if (user == null)
				throw new Exception("Invalid username or password");
			if (!EncryptionUtils.VerifyPassword(loginRequest.Password, user.PasswordHash))
				throw new Exception("Invalid username or password");

			if (user.IsActive == false)
				throw new Exception("User is not active");

			var userInfo = _mapper.Map<UserInfoForTokenDto>(user);
			var loginToken = GenerateJwtToken(userInfo);
			await _userRepository.UpdateLastLogin(user.UserId);
			return new BaseResponse<LoginTokenDto>(loginToken) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e, "Error logging in user");
			return new BaseResponse<LoginTokenDto> { Success = false, Message = e.Message };
		}
	}

	LoginTokenDto GenerateJwtToken(UserInfoForTokenDto userInfo)
	{
		// generate token that is valid for 7 days
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_authorizationSettings.JwtSecret);

		var subject = new ClaimsIdentity(
			new[]
			{
				new Claim("UserId", userInfo.UserId),
				new Claim(ClaimTypes.Name, userInfo.FullName),
				new Claim(ClaimTypes.Email, userInfo.Email),
				new Claim(ClaimTypes.Role, userInfo.Role)
			}
		);

		var tokenExpiry = DateTimeOffset.UtcNow.AddMinutes(
			_authorizationSettings.JwtExpirationInMinutes
		);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = subject,
			Expires = tokenExpiry.UtcDateTime,
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature
			)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);

		return new LoginTokenDto
		{
			AccessToken = tokenHandler.WriteToken(token),
			Expiry = tokenExpiry.ToUnixTimeMilliseconds()
		};
	}
}