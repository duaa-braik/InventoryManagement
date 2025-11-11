using FlashSaleDB;
using InventoryManagement.API.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;          
using Utils = InventoryManagement.API.Utils.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });
    c.AddServer(new OpenApiServer { Url = "/inventory" });  
});

builder.Services.AddDbContext<FlashSaleDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

builder.Services.AddServices(builder.Configuration);
builder.Services.AddVersioning();
builder.Services.AddRabbitMq(builder.Configuration);

var app = builder.Build();

app.UsePathBase("/inventory");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/inventory/swagger/v1/swagger.json", "Inventory API v1");
        c.RoutePrefix = "swagger";  
    });
}

await Utils.LoadInventory(app);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
