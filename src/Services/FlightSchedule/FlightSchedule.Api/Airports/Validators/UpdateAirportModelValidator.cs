using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.Validators;
using FluentValidation;

namespace FlightSchedule.Api.Airports.Validators;

public class UpdateAirportModelValidator : AbstractValidator<AirportUpdateModel>
{
    public UpdateAirportModelValidator()
    {
        RuleFor(t=>t.IataCode).NotNull().IataLocationCodeMustBeValid().WithName("Iata");
        RuleFor(t => t.Name).NotEmpty();
    }
}
