namespace DataAccessLibrary.Models;

public class ServiceResponseModel<T>
{
    public bool Success { get; set; } = true;
    public T Data { get; set; }
    public string Message { get; set; } = "";
}
