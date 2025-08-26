using InfraEstrutura.Data;
using InfraEstrutura.Repositorio;
using Interface.Repositorio;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Projeto2025_API.Mapping;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//configurar o contexto
builder.Services.AddDbContext<EmpresaContexto>
    (p=>p.UseSqlServer(
        builder.Configuration
        .GetConnectionString("default")));

//configurar o mapping

builder.Services.AddAutoMapper(
    p => p.AddProfile<MappingProfile>());

//configurar injeção de dependencia
builder.Services.AddScoped<ICategoriaRepositorio,
    CategoriaRepositorio>();
builder.Services.AddScoped<ICategoriaService,
    CategoriaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
