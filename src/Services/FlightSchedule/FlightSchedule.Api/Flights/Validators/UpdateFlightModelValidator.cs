using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Api.Validators;
using FluentValidation;

namespace FlightSchedule.Api.Flights.Validators
{
    public class UpdateFlightModelValidator: AbstractValidator<UpdateFlightModel>
    {
        public UpdateFlightModelValidator()
        {
            RuleFor(p=>p.FlightNumber).NotNull().FlightNumberMustBeValid().WithName("Flight number");
            RuleFor(p => p.DepartureAirport).NotNull().IataLocationCodeMustBeValid().WithName("Departure airport");
            RuleFor(p=>p.ArrivalAirport).NotNull().IataLocationCodeMustBeValid().WithName("Arrival airport");
            RuleFor(p => p.DepartureAt).NotEmpty().OverridePropertyName("departureAt");
            RuleFor(p => p.ArrivalAt).NotEmpty()
                .GreaterThanOrEqualTo(p => p.DepartureAt)
                //.WithMessage("{PropertyName} must be greater than or equal to '{ComparisonValue:yyyy-MM-ddTHH:mm:ss.FFFZ}'")
                .WithMessage("Arrival date must be greater than departure date")
                .LessThanOrEqualTo(p => p.DepartureAt.AddDays(1))
                //.WithMessage("{PropertyName} must be less than or equal to '{ComparisonValue:yyyy-MM-ddTHH:mm:ss.FFFZ}'")
                .WithMessage("Arrival date must be not a day greater than departure date")
                .WithName("Arrival date");
            RuleFor(x => x).Must((command) => true).WithMessage("Something terrible wrong");
        }
    }
}
