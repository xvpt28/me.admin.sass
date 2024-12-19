namespace me.admin.api.DTOs;

public class BaseResponse<T>
{
	public bool Success { get; set; }
	public string? Message { get; set; }
	public T? Data { get; set; }

	public BaseResponse(T data)
	{
		Data = data;
		Success = true;
	}

	public BaseResponse()
	{
	}
}