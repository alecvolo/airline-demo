using FlightSchedule.Domain.ValueObjects;
using FluentValidation;

namespace FlightSchedule.Api.Validators;

public static class DomainValidatorBuilders
{
    public static IRuleBuilderOptions<T, string?> IataLocationCodeMustBeValid<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(IataLocationCode.IsValid).WithMessage("{PropertyName} code must contain 3 letters");
    }
    public static IRuleBuilderOptions<T, string?> FlightNumberMustBeValid<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(FlightNumber.IsValid)
            //.WithMessage("{PropertyName} must contain 2 letter IATA flight codes and a number from 1 to 9999")
            .WithMessage(FlightNumber.FlightNumberInvalidMessage);
    }
}