namespace Bot.Application.MCPTools;

public readonly record struct ToolResult<T>
{
    public T? Data { get; }
    
    public string? Error { get; }

    private ToolResult(T? data, string? error)
    {
        Data = data;
        Error = error;
    }
    
    public static implicit operator ToolResult<T>(T data) => new(data, null);
    public static implicit operator ToolResult<T>(string error) => new(default, error);
}