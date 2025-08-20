using ApprovalSystem.Application;
using ApprovalSystem.Infrastructure;
using ApprovalSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using SendGrid.Helpers.Errors.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure();

// Application service
builder.Services.AddScoped<RequestService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HeaderCurrentUser>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.SwaggerDoc("v1", new OpenApiInfo { Title = "Approval System API", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Approval System API v1");
        c.RoutePrefix = "swagger";
    });
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler("/error");

app.MapControllers();
app.Map("/error", (HttpContext http) =>
{
    var ex = http.Features.Get<IExceptionHandlerFeature>()?.Error;

    var (status, title) = ex switch
    {
        ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
        NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
        InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
        _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
    };

    return Results.Problem(
        title: title,
        statusCode: status,
        detail: ex?.Message);
})
.WithDisplayName("Global Error Handler")
.ExcludeFromDescription(); // it doesn't appear in Swagger

// Seed initial data
await app.Services.SeedAsync();

app.Run();
