using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.Airports.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace FlightSchedule.Api.Tests.Airports.Features
{
    public class UpdateAirportModelValidatorTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        [InlineData("q", false)]
        [InlineData("aa ", false)]
        [InlineData("aaaa", false)]
        [InlineData("12a", false)]
        [InlineData("_$%", false)]
        [InlineData("NYC", true)]
        [InlineData("nyc", true)]
        public void Should_Pass_Validation_For_IataCode(string iataCode, bool validInput)
        {
            
            var model = new AirportUpdateModel() {IataCode = iataCode};
            var validator = new UpdateAirportModelValidator();
            var validationResult = validator.TestValidate(model);
            if (validInput)
            {
                validationResult.ShouldNotHaveValidationErrorFor(p=>p.IataCode);
            }
            else
            {
                validationResult.ShouldHaveValidationErrorFor(p => p.IataCode);
            }
        }
        [Theory]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        [InlineData("name", true)]
        [InlineData("  b ", true)]
        public void Should_Catch_Error_For_Name(string name, bool validInput)
        {
            var model = new AirportUpdateModel() { Name = name };
            var validator = new UpdateAirportModelValidator();
            var validationResult = validator.TestValidate(model);
            if (validInput)
            {
                validationResult.ShouldNotHaveValidationErrorFor(m => m.Name);
            }
            else
            {
                validationResult.ShouldHaveValidationErrorFor(m => m.Name);
            }
        }

    }
}