using Microsoft.OpenApi.Models;

namespace AuthServer.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        _ = services.AddSwaggerGen(static c =>
        {
            c.DescribeAllParametersInCamelCase();

            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Auth Server API",
                    Description =
                        "An authentication server that supports JWT token generation and redirect-based authentication for SPAs",
                    Contact = new OpenApiContact { Name = "Auth Team", Email = "auth@vantaca.com" },
                }
            );

            // Include XML comments in Swagger documentation
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Define the security scheme
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                }
            );

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                }
            );
        });

        return services;
    }
}
