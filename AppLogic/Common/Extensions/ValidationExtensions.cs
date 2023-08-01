using FluentValidation.Results;

namespace Common.Extensions;

public static class ValidationExtensions
{
    public static IEnumerable<ValidationErrorDto> GetValidationErrors(this IEnumerable<ValidationFailure> failures)
        => failures.Select(_ => new ValidationErrorDto(_.PropertyName, _.ErrorMessage));
}