using FlightSchedule.Api.Flights.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;
using Swashbuckle.AspNetCore.Annotations;

namespace FlightSchedule.Api.Flights.Features;

[Route("api/v{version:apiVersion}/flights")]
[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Produces("application/json")]
public class FlightsController : ControllerBase
{
    private readonly ILogger<FlightsController> _logger;
    private readonly IMediator _mediator;

    public FlightsController(ILogger<FlightsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [RequiredScope("Read")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<FlightViewModel>>> GetAll([FromQuery] string? flightNumber, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetFlights.Query(flightNumber), cancellationToken));
    }
    [HttpGet("{flightNumber}", Name = "GetFlightByNumber")]
    [RequiredScope("Read")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string flightNumber,
        [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions,
            CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFlightByNumber.Query(flightNumber), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }
    [HttpPost]
    [RequiredScope("Write")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create new flight", Description = "Create new flight")]
    public async Task<ActionResult<FlightViewModel>> Create([FromBody] UpdateFlightModel? model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateFlight.Command(model!), cancellationToken);
        return CreatedAtRoute("GetFlightByNumber", new { flightNumber = result.FlightNumber }, result);
    }
    //[HttpPut("{id:guid}")]
    //[RequiredScope("Write")]
    //[ProducesResponseType(StatusCodes.Status202Accepted)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[SwaggerOperation(Summary = "Update airport", Description = "Update airport")]
    //public async Task<IActionResult> Replace(Guid id, [FromBody] UpdateAirportModel model, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(new UpdateAirport.Command(id, model), cancellationToken);
    //    return AcceptedAtRoute("GetAirportById", new { id }, result);
    //}
    //[HttpPatch("{id:guid}")]
    //[RequiredScope("Write")]
    //[ProducesResponseType(StatusCodes.Status202Accepted)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodeResult))]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[Produces("application/json")]
    //[SwaggerOperation(Summary = "Path airport", Description = "Path airport")]
    //public async Task<ActionResult<UpdateAirportModel>> Path(Guid id, [FromBody] JsonPatchDocument<UpdateAirportModel> path, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(new PatchAirport.Command(id, path), cancellationToken);
    //    return AcceptedAtRoute("GetAirportById", new { id }, result);
    //}

    //private (bool isError, string Resutl, Exception? Ex) unc()
    //{
    //    List<(string a, string b)> listAb = new() { ("a", "b") };
    //    listAb[0] = ("c", "d");

    //    return (false, "", null);
    //}

    //private IActionResult Pop([FromServices] IServiceScopeFactory serviceScopeFactory)
    //{
    //    Task.Run(async () =>
    //    {
    //        await using var scope = serviceScopeFactory.CreateAsyncScope();
    //        var dbContext = scope.ServiceProvider.GetRequiredService<FlightDbContext>();
    //        await dbContext.SaveChangesAsync();

    //    }
    //        );
    //    return Accepted();

    //}

}
