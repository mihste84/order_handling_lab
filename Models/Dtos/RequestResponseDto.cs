namespace Models.Dtos;


public class RequestResponseDto<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
}
