using Dx.Domain.Errors;

namespace AccountableDecisionSystem.Domain;

internal static class S0AccountabilitySubstrate
{
    internal static Dx.Domain.Result<T> Success<T>(T value) where T : notnull =>
        Dx.Domain.Dx.Result.Success(value);

    internal static Dx.Domain.Result<T> Failure<T>(string code, string message) where T : notnull =>
        Dx.Domain.Dx.Result.Failure<T>(DomainError.Create(code, message));
}
