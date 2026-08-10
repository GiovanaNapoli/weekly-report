using System.Reflection;
using System.Text.Json.Serialization;
using Application;
using Infrastructure;
using Microsoft.OpenApi;

namespace Api
{
    public static class DependencyInjection
    {
        private const string DefaultCorsPolicy = "AllowSpecificOrigins";

        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
        {

            // Adding controllers
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Configuring CORS POLICY
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicy, policy =>
                {
                    var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
                if (File.Exists(apiXmlPath))
                    c.IncludeXmlComments(apiXmlPath);

                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, "Application.xml");
                if (File.Exists(applicationXmlPath))
                    c.IncludeXmlComments(applicationXmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Access token JWT. Informe apenas o token, sem o prefixo \"Bearer \".",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer"),
                        new List<string>()
                    }
                });
            });

            services.AddApplication();
            services.AddInfrastructure(configuration);

            return services;
        }

        public static WebApplication UseApiSettings(this WebApplication app)
        {
            app.UseCors(DefaultCorsPolicy);

            var swaggerEnabled = app.Configuration
                .GetValue<bool?>("Swagger:Enabled") ?? app.Environment.IsDevelopment();

            if (swaggerEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication(); // ← JWT lê o token
            app.UseAuthorization();  // ← valida permissões

            app.MapControllers();

            return app;
        }
    }
}