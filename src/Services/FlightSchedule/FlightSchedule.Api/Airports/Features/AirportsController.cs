using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain.EfCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Swashbuckle.AspNetCore.Annotations;

namespace FlightSchedule.Api.Airports.Features;

[Route("api/v{version:apiVersion}/airports")]
[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Produces("application/json")]
public class AirportsController : ControllerBase
{
    private readonly ILogger<AirportsController> _logger;
    private readonly IMediator _mediator;
    private readonly FlightDbContext _dbContext;

    public AirportsController(ILogger<AirportsController> logger,  IMediator  mediator, FlightDbContext dbContext)
    {
        _logger = logger;
        _mediator = mediator;
        _dbContext = dbContext;
    }

    [HttpGet]
    [RequiredScope("Read")]
    //[Authorize(Roles = "FlightApi.Writes")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<AirportViewModel>>> GetAll([FromQuery] string? iataCode, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetAirports.Query(iataCode), cancellationToken));
    }
    [HttpGet("{id:guid}", Name = "GetAirportById")]
    [RequiredScope("Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AirportViewModel>> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAirportById.Query(id), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }
    [HttpGet("{iataCode:alpha}", Name = "GetAirportByCode")]
    [RequiredScope("Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<AirportViewModel>>> Get([FromRoute]  string iataCode, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAirportByCode.Query(iataCode), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }
    [HttpPost]
    [RequiredScope("Write")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create new airport", Description = "Create new airport")]
    public async Task<IActionResult> Create([FromBody] AirportUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAirport.Command(model), cancellationToken);
        return CreatedAtRoute("GetAirportById", new { id = result.Id }, result);
    }
    [HttpPut("{id:guid}")]
    [RequiredScope("Write")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Update airport", Description = "Update airport")]
    public async Task<IActionResult> Replace(Guid id, [FromBody] AirportUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAirport.Command(id, model), cancellationToken);
        return AcceptedAtRoute("GetAirportById", new { id }, result);
    }
    [HttpPatch("{id:guid}")]
    [RequiredScope("Write")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodeResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    [SwaggerOperation(Summary = "Path airport", Description = "Path airport")]
    public async Task<ActionResult<AirportUpdateModel>> Path(Guid id, [FromBody] JsonPatchDocument<AirportUpdateModel> path, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new PatchAirport.Command(id, path), cancellationToken);
        return AcceptedAtRoute("GetAirportById", new { id }, result);
    }

    private (bool isError, string Resutl, Exception? Ex) unc()
    {
        List<(string a, string b)> listAb = new() { ("a", "b") };
        listAb[0] = ("c", "d");

        return (false, "", null);
    }

    private  IActionResult Pop([FromServices] IServiceScopeFactory serviceScopeFactory)
    {
        Task.Run(async () =>
            {
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FlightDbContext>();
                await dbContext.SaveChangesAsync();

            }
        );
        return Accepted();

    }

}