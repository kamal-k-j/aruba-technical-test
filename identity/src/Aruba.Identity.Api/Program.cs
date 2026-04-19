using Aruba.Identity.Api.Common.Middlewares;
using Aruba.Identity.Api.DependencyInjection;
using Aruba.Identity.Application.DependencyInjection;
using Aruba.Identity.Infrastructure.DependencyInjection;

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

builder.Services.AddSwaggerGen();

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