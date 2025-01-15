using System.Reflection;
using Transactions_Api.Application.Handlers;
using Transactions_Api.Application.Services;
using Transactions_Api.Domain.Validators;
using Transactions_Api.Infrastructure.Infrastructure;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using Transactions_Api.Infrastructure.Repositories;
using Transactions_Api.Shared.Utils;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Transactions_Api.Controllers;
using Transactions_Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Configurar o Swagger para gerar o arquivo JSON
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transactions API",
        Version = "v1",
        Description = "API para transações financeiras",
        Contact = new OpenApiContact
        {
            Name = "Transactions-Maurício Pereira",
            Email = "mauricio.pvieira1@gmail.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));


    // Configurar o esquema de segurança para API Key
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key necessária para acessar endpoints protegidos. Insira a chave no campo abaixo.",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY", // Nome do cabeçalho onde a API Key será enviada
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new List<string>()
        }
    });
});


string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 25)),
        options => options.EnableRetryOnFailure()).EnableDetailedErrors().EnableSensitiveDataLogging());

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<TransacaoCreateValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<TransacaoUpdateDTOValidator>();
    });

builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<ITxidGenerator, Helper>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<ICachingService, CachingService>();

builder.Services.AddStackExchangeRedisCache(o => {
    o.InstanceName = "Instance";
    o.Configuration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(Program));

// Adicionar serviços necessários para HATEOAS
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IUrlHelper>(factory =>
{
    var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
    return new UrlHelper(actionContext);
});

// Adicionar serviços necessários para logging
builder.Services.AddLogging();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(TransacoesController).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateTransactionHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(DeleteTransactionHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateTransactionHandler).Assembly);
});




var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker") || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Transactions API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();


app.MapControllers();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();