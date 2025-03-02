using FluentValidation.Results;

namespace Articles.Application.Commons.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("Wystąpił jeden lub więcej błędów walidacji.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
} 