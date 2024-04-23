namespace DiscussionFleet.HttpApi.Utils;

public class ApiResponse
{
    public required bool Success { get; set; }
    public required int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; } = null;
}

public class ApiResponse<TData>
{
    public required bool Success { get; set; }
    public required int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public TData? Data { get; set; }
}