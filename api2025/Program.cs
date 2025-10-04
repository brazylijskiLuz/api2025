using System.Text.RegularExpressions;
using api2025.Services;
using EntityArchitect.CRUD;
using EntityArchitect.CRUD.Actions;
using EntityArchitect.CRUD.Authorization;
using EntityArchitect.CRUD.Entities;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddEntityArchitect(typeof(Program).Assembly, connectionString ?? "");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseActions(typeof(Program).Assembly);

builder.Services.AddControllers();

builder.Services.AddScoped<IXlsxService, XlsxService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.MapEntityArchitectCrud(typeof(Program).Assembly);

app.Run();
