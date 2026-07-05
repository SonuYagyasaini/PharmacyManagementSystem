namespace PharmacyManagement.Api.Infrastructure.Exceptions;

public sealed record ApiErrorResponse(
    int StatusCode,
    string Message,
    string TraceId);
