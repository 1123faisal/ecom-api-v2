namespace EcomApi.Domain.Exceptions;

/// <summary>
/// Thrown when a business invariant or domain rule is violated (e.g. insufficient stock).
/// Maps to HTTP 422 Unprocessable Entity.
/// </summary>
public sealed class BusinessRuleException(string message) : Exception(message);
