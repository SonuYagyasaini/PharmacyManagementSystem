namespace PharmacyManagement.Api.Infrastructure.Exceptions;

public sealed class BadRequestException(string message) : Exception(message);
