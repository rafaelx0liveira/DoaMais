using DoaMais.API.Middlewares;
using DoaMais.CrossCutting.DependencyInjection;
using Elastic.Clients.Elasticsearch;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using VaultService.Interface;

/// <summary>
/// Classe principal respons�vel por configurar e iniciar a API DoaMais.
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// Adiciona as configura��es de infraestrutura e inje��o de depend�ncias
builder.Services.AddInfrastructure(builder.Configuration);

// Configura��o dos controladores e serializa��o JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configura��o do CORS para permitir qualquer origem, m�todo e cabe�alho
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Configura��o do Serilog para logging estruturado com o Elasticsearch

// Registrar ElasticsearchClient dinamicamente usando DI
builder.Services.AddSingleton(provider =>
{
    var vaultClient = provider.GetRequiredService<IVaultClient>(); // Resolve o IVaultClient

    var elasticUrlSecret = builder.Configuration["KeyVaultSecrets:Elasticsearch:Url"] ?? throw new ArgumentNullException("Elasticsearch URL is missing in Vault");
    var elasticPasswordSecret = builder.Configuration["KeyVaultSecrets:Elasticsearch:Password"] ?? throw new ArgumentNullException("Elasticsearch Password is missing in Vault");
    var elasticUsernameSecret = builder.Configuration["KeyVaultSecrets:Elasticsearch:Username"] ?? throw new ArgumentNullException("Elasticsearch Username is missing in Vault");

    var elasticUrl = vaultClient.GetSecret(elasticUrlSecret);
    var elasticUsername = vaultClient.GetSecret(elasticUsernameSecret);
    var elasticPassword = vaultClient.GetSecret(elasticPasswordSecret); 

    var settings = new ElasticsearchClientSettings(new Uri(elasticUrl))
        .Authentication(new BasicAuthentication(elasticUsername, elasticPassword))
        .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);

    return new ElasticsearchClient(settings);
});

builder.Host.UseSerilog((context, config) =>
{
    var elasticClient = builder.Services.BuildServiceProvider().GetRequiredService<ElasticsearchClient>();

    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/api_log.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(elasticClient.Transport));
});

// Configura��o do Swagger para documenta��o da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DoaMais API",
        Version = "v1",
        Description = "API para gerenciamento de banco de sangue",
        Contact = new OpenApiContact
        {
            Name = "Rafael Oliveira",
            Email = "rafaelaparecido.oliveirasilva@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/rafael-aparecido-silva-oliveira/")
        }
    });

    // Configura��o do esquema de autentica��o JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token!"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
    });
});

// Configura��o da autentica��o JWT usando segredo armazenado no Vault
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IVaultClient>((options, vaultService) =>
    {
        var jwtSecretKey = builder.Configuration["KeyVaultSecrets:JwtSecret"]
            ?? throw new ArgumentNullException("JwtSecret is missing in Vault");
        var jwtSecret = vaultService.GetSecret(jwtSecretKey);

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Adiciona servi�os de autentica��o e autoriza��o
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configura��o do ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DoaMais API v1");
        c.RoutePrefix = string.Empty;
    });
}

// Adiciona middleware para logging de requisi��es usando Serilog
app.UseSerilogRequestLogging();

// Middleware de seguran�a e CORS
app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Middleware de autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestTimingMiddleware>();

// Mapeia os controladores da API
app.MapControllers();

// Inicia a aplica��o
app.Run();

