var builder = WebApplication.CreateBuilder(args);

// OpenAPI / Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Health endpoint
app.MapGet("/health", () =>
{
    return Results.Ok(new HealthResponse("healthy"));
})
.WithName("HealthCheck")
.WithTags("Health")
.Produces<HealthResponse>(StatusCodes.Status200OK);

app.Run();

record HealthResponse(string Status);