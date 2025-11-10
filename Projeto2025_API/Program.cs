using Dominio.Dtos;
using FluentValidation;
using InfraEstrutura.Data;
using InfraEstrutura.Repositorio;
using Interface.Repositorio;
using Interface.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Projeto2025_API.Mapping;
using Projeto2025_API.Validation;
using Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar serialização de DateTime para usar formato local
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Manter datas no formato ISO 8601 mas sem conversão automática de timezone
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insira 'Bearer' + espa�o + token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});


//configurar JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        
                        ClockSkew = TimeSpan.Zero // Remove a tolerância de tempo
                    };
                });
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);




//configurar o contexto
builder.Services.AddDbContext<EmpresaContexto>
    (p=>p.UseSqlServer(
        builder.Configuration
        .GetConnectionString("Default")));

//configurar o mapping

builder.Services.AddAutoMapper(
    p => p.AddProfile<MappingProfile>());

//configurar inje��o de dependencia
builder.Services.AddScoped<ICategoriaRepositorio,
    CategoriaRepositorio>();
builder.Services.AddScoped<ICategoriaService,
    CategoriaService>();
builder.Services.AddScoped<IValidator<CategoriaDto>,
        CategoriaValidation>();

// Repositórios
builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IVendaRepositorio, VendaRepositorio>();
builder.Services.AddScoped<IItemVendaRepositorio, ItemVendaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Serviços
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Validações
builder.Services.AddScoped<IValidator<ProdutoDto>, ProdutoValidation>();
builder.Services.AddScoped<IValidator<ClienteDto>, ClienteValidation>();
builder.Services.AddScoped<IValidator<VendaDto>, VendaValidation>();
builder.Services.AddScoped<IValidator<CreateUsuarioDto>, CreateUsuarioValidation>();
builder.Services.AddScoped<IValidator<UpdateUsuarioDto>, UpdateUsuarioValidation>();
builder.Services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordValidation>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
