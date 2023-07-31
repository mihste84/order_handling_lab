using FluentValidation.Results;

namespace Common.Models
{
    public record ValidationError
    {
        public IEnumerable<ValidationErrorDto> Errors { get; init; }
        public string? Message { get; init; }
        public ValidationError(IEnumerable<ValidationFailure> failures, string? message = default)
        {
            Errors = failures.Select(x => new ValidationErrorDto(x.PropertyName, x.ErrorMessage));
            Message = message;
        }
    };
}