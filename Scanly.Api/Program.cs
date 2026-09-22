using Scanly.Api.Endpoints;
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



// Invoice endpoints
app.MapInvoiceEndpoints();
app.MapHealthEndpoints();

app.Run();


public partial class Program { }