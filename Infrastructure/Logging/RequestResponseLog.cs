namespace PharmacyManagement.Api.Infrastructure.Logging;

public sealed class RequestResponseLog
{
    public Guid Id { get; private set; }
    public string TraceId { get; private set; } = string.Empty;
    public string Method { get; private set; } = string.Empty;
    public string Path { get; private set; } = string.Empty;
    public string? QueryString { get; private set; }
    public string? RequestBody { get; private set; }
    public int StatusCode { get; private set; }
    public string? ResponseBody { get; private set; }
    public long ElapsedMilliseconds { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }

    private RequestResponseLog()
    {
    }

    public RequestResponseLog(
        string traceId,
        string method,
        string path,
        string? queryString,
        string? requestBody,
        int statusCode,
        string? responseBody,
        long elapsedMilliseconds,
        string? ipAddress,
        string? userAgent)
    {
        Id = Guid.NewGuid();
        TraceId = traceId;
        Method = method;
        Path = path;
        QueryString = queryString;
        RequestBody = requestBody;
        StatusCode = statusCode;
        ResponseBody = responseBody;
        ElapsedMilliseconds = elapsedMilliseconds;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        RequestedAtUtc = DateTime.UtcNow;
    }
}
