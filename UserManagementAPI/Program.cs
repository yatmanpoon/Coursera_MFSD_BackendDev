var builder = WebApplication.CreateBuilder(args);
var validToken = builder.Configuration["Authentication:Token"];

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware to handle unhandled exceptions and return a consistent error response
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Unhandled exception while processing {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        if (context.Response.HasStarted)
        {
            throw;
        }

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Internal server error."
        });
    }
});

// Middleware to validate the presence and correctness of the Bearer token in the Authorization header
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
    {
        app.Logger.LogWarning("Unauthorized request: missing Authorization header for {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unauthorized."
        });
        return;
    }

    const string bearerPrefix = "Bearer ";
    var authorizationValue = authorizationHeader.ToString();

    if (!authorizationValue.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
    {
        app.Logger.LogWarning("Unauthorized request: invalid Authorization scheme for {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unauthorized."
        });
        return;
    }

    var token = authorizationValue[bearerPrefix.Length..].Trim();
    var isTokenValid = !string.IsNullOrWhiteSpace(validToken)
        && string.Equals(token, validToken, StringComparison.Ordinal);

    if (!isTokenValid)
    {
        app.Logger.LogWarning("Unauthorized request: invalid token for {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unauthorized."
        });
        return;
    }

    await next();
});

// Middleware to log incoming requests and their corresponding responses
app.Use(async (context, next) =>
{
    await next();

    app.Logger.LogInformation(
        "HTTP {Method} {Path} responded {StatusCode}",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode);
});

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new
{
    Name = "User Management API",
    Status = "Running",
    TimestampUtc = DateTime.UtcNow
}));

app.Run();
