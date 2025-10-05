using System.Globalization;
using System.Text.RegularExpressions;
using api2025.Repositories;
using api2025.Services;
using api2025.Services.PdfServices;
using EntityArchitect.CRUD;
using EntityArchitect.CRUD.Actions;
using EntityArchitect.CRUD.Authorization;
using EntityArchitect.CRUD.Entities;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddEntityArchitect(typeof(Program).Assembly, connectionString ?? "");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseActions(typeof(Program).Assembly);

builder.Services.AddControllers();

builder.Services.AddScoped<IXlsxService, XlsxService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IPostCodeRepository, PostCodeRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (AppContext.TryGetSwitch("System.Globalization.Invariant", out var inv) && inv)
{
    Console.WriteLine("Globalization-Invariant = ON");
}

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.MapEntityArchitectCrud(typeof(Program).Assembly);

app.Run();
