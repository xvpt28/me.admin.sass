namespace me.admin.api.Utils;

public static class VerificationUtils
{
	public static string GenerateVerificationCode(int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var random = new Random();
		var code = new char[length];

		for (var i = 0; i < length; i++)
		{
			code[i] = chars[random.Next(chars.Length)];
		}

		return new string(code);
	}
}