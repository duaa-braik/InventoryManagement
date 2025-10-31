using FlashSaleDB;
using InventoryManagement.API.Extensions;
using Microsoft.EntityFrameworkCore;
using Utils = InventoryManagement.API.Utils.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FlashSaleDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

builder.Services.AddServices(builder.Configuration);

builder.Services.AddVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await Utils.LoadInventory(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();