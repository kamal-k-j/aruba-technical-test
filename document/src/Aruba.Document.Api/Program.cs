using Aruba.Document.Api.Common.Middlewares;
using Aruba.Document.Api.DependencyInjection;
using Aruba.Document.Application.DependencyInjection;
using Aruba.Document.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services
    .AddApi()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddAuthentication(builder.Configuration)
    .AddSwaggerGenWithBearerAuth();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();