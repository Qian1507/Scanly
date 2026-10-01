namespace Scanly.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () =>
        {
            return Results.Ok(new HealthResponse("healthy"));
        })
        .WithName("HealthCheck")
        .WithTags("Health")
        .Produces<HealthResponse>(StatusCodes.Status200OK);
    }
}

public record HealthResponse(string Status);