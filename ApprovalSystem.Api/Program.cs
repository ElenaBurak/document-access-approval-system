using ApprovalSystem.Application;
using ApprovalSystem.Infrastructure;
using ApprovalSystem.Infrastructure.Identity;
using ApprovalSystem.Infrastructure.Notifications;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure();

// Application service
builder.Services.AddScoped<RequestService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HeaderCurrentUser>();
builder.Services.AddSingleton<INotificationBus, NoopNotificationBus>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(a => a.Run(async context =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var (status, title) = ex switch
    {
        KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
        UnauthorizedAccessException => (StatusCodes.Status403Forbidden, ex.Message),
        ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
        InvalidOperationException => (StatusCodes.Status409Conflict, ex.Message),
        _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
    };
    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = title });
}));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
await app.Services.SeedAsync();
app.Run();
