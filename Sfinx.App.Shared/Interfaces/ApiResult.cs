namespace Sfinx.App.Shared.Interfaces;

public struct ApiResult<T>
{
    public bool Success { get; set; }
    public T Response { get; set; }
    public int HttpStatus { get; set; }
    public string ErrorMessage { get; set; }
}