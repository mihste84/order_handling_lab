
using FluentValidation.Results;

namespace AppLogic.Common.Exceptions;

public class AppValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; set; } = new List<ValidationError>();

    public AppValidationException(IEnumerable<ValidationFailure> failures) : base()
    {
        Errors = GetValidationErrors(failures);
    }

    private static IEnumerable<ValidationError> GetValidationErrors(IEnumerable<ValidationFailure> failures)
        => failures
        .GroupBy(_ => new { _.PropertyName, _.ErrorMessage })
        .Select(_ => new ValidationError(_.Key.PropertyName, _.Key.ErrorMessage));
}