﻿using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace RSFBackup.Api.Extensions;

public static class ServicesRegistration
{
    private static string GetXmlPath()
    {
        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        return xmlPath;
    }



    /// <summary>
    /// Adiciona o serviço para swagger OpenAPI no container de serviços
    /// </summary>
    /// <param name="services">container services</param>
    /// <returns>container services com o swagger OpenAPI no container</returns>
    public static void AddSwaggerOpenAPI(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Amazon S3 Web API",
                Version = "v1",
                Description = "Demonstração dos recursos disponíveis na api.",
                Contact = new OpenApiContact
                {
                    Name = "Rafael Francisco",
                    Email = "broncasrafa@gmail.com",
                    Url = new Uri("https://github.com/broncasrafa")
                }
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Informe seu token bearer para acessar os recursos da API da seguinte forma: Bearer {your token here}",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });

            // Set the comments path for the Swagger JSON and UI.                
            c.IncludeXmlComments(GetXmlPath());
        });
    }


    /// <summary>
    /// Adiciona as configurações de controller e JSON no container de serviços
    /// </summary>
    /// <param name="services">container services</param>
    /// <returns>container services com as configurações de controller e JSON no container</returns>
    public static void AddControllerAndJsonConfigurations(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                //x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.None;
            });
    }

    /// <summary>
    /// Adiciona as configurações dos serviços no container de serviços
    /// </summary>
    /// <param name="services">container services</param>
    /// <param name="configuration">configuration properties</param>
    public static void AddSettingsConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<TwitterSettings>(configuration.GetSection("TwitterSettings"));
    }

    /// <summary>
    /// Adiciona as configurações dos serviços da Amazon AWS no container de serviços
    /// </summary>
    /// <param name="services">container services</param>
    /// <param name="configuration">configuration properties</param>
    public static void AddAWSServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAWSService<IAmazonS3>(configuration.GetAWSOptions());
    }
}
