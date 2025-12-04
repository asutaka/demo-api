using DemoApi;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Tắt camelCase cho khỏe
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.PropertyNamingPolicy = null);

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Pass"] ?? "guest");
        });

        cfg.ReceiveEndpoint("submit-order-queue", e =>
        {
            e.ConfigureConsumer<SubmitOrderConsumer>(context);
        });
    });
});

var app = builder.Build();

// Tạo bảng nếu chưa có
string pgConn = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=demo";

await using (var conn = new NpgsqlConnection(pgConn))
{
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(@"
        CREATE TABLE IF NOT EXISTS messages (
            id SERIAL PRIMARY KEY,
            content TEXT NOT NULL
        )", conn);
    await cmd.ExecuteNonQueryAsync();
}

app.MapGet("/", () => "MassTransit đang chạy ngon lành!");

app.MapPost("/order", async ([FromBody] SubmitOrder order, IPublishEndpoint publish) =>
{
    await publish.Publish(order);
    return Results.Ok(order);
});

app.MapGet("/messages", async () =>
{
    var result = new List<string>();
    await using var conn = new NpgsqlConnection(pgConn);
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand("SELECT content FROM messages ORDER BY id", conn);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
        result.Add(reader.GetString(0));
    return Results.Ok(result);
});

app.Run();