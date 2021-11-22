using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PostmanApi.Authentication;
using PostmanApi.Filters;
using PostmanApi.Middleware;
using PostmanApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using PostmanApi.Authentication.Jwt;
using PostmanApi.Interfaces;
using PostmanApi.Repositories;
using PostmanApi.Services;
using PostmanApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Versioning;
using PostmanApi.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;

//https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-parameter
//https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-2.0/#dbcontext-pooling
//https://dev.to/djeikyb/configure-entity-framework-with-dependency-injection-2726
//https://neelbhatt.com/2018/02/27/use-dbcontextpooling-to-improve-the-performance-net-core-2-1-feature/
//https://www.c-sharpcorner.com/blogs/net-core-mvc-with-entity-framework-core-using-dependency-injection-and-repository
//https://www.newtonsoft.com/json/help/html/NamingStrategyCamelCase.htm

// Add-Migration Initial
// Update-Database -Verbose
// docker pull mcr.microsoft.com/mssql/server:2019-latest
// docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssw0rd" -p 1433:1433 --name "LOCALDB" -d mcr.microsoft.com/mssql/server:2019-latest
// docker exec -it LOCALDB "bash"
// /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'P@ssw0rd'
// CREATE LOGIN postman_user WITH PASSWORD = 'P@ssw0rd';CREATE LOGIN postman_user WITH PASSWORD = 'P@ssw0rd';
// CREATE DATABASE Postman;
// GO
// USE Postman;
// CREATE USER postman_user;
// GO
// EXEC sp_addrolemember N'db_owner', N'postman_user';
// GO
// dotnet tool uninstall --global dotnet-user-secrets
// dotnet tool install --global dotnet-user-secrets
// dotnet user-secrets init
// dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=Contacts;User ID=sa;Password=tiger;Timeout=300;MultipleActiveResultsets=True;"
// dotnet user-secrets list
// Scaffold-DbContext "Server=.;Database=Contacts;Timeout=300;MultipleActiveResultsets=True;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -t Contacts -f
// dotnet tool install --global dotnet-ef
// dotnet tool install --global dotnet-dev-certs
// dotnet dev-certs
// dotnet dev-certs https

//https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
//https://github.com/dotnet/aspnet-api-versioning
//https://github.com/dotnet/aspnet-api-versioning/blob/master/samples/aspnetcore/SwaggerSample/Startup.cs
//https://stackoverflow.com/questions/59756374/configuring-apiversiondescriptions-in-netcore-3-0-without-using-build-service-p

namespace PostmanApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration);
                builder.AddDebug();
                builder.AddJsonConsole();
            });

            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            services.AddDbContextPool<DataContext>(
                options =>
                {
                    options.UseSqlServer(connectionString,
                            sqlServerOptions =>
                            {
                                sqlServerOptions.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
                                sqlServerOptions.EnableRetryOnFailure(2);
                            });
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                    options
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Name, DbLoggerCategory.Database.Name, DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
                    options.LogTo(message => Debug.WriteLine(message));
                });

            services.AddControllers(options =>
                {
                    options.Filters.Add<CustomExceptionFilter>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlSerializerFormatters()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                });

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.UseApiBehavior = true;

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader(),  //default "api-version"
                    new QueryStringApiVersionReader("v"),
                    new HeaderApiVersionReader("v"),
                    new HeaderApiVersionReader("api-version"),
                    new MediaTypeApiVersionReader(),    //default "v"
                    new MediaTypeApiVersionReader("api-version")
                    );
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.Configure<MvcOptions>(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureOptions>();
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<DefaultValues>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Description = "Provide a token in the form 'Bearer {key}' to access a resource. "
                });

                options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Basic",
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic auth may only be used for the AccessToken resource. "
                });

                // Applies the schemes defined above.
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Basic"
                            },
                            Scheme = "basic",
                            Name = "Basic",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.ResolveConflictingActions(c => c.Last());
            });

            services.AddCors();

            services.AddAuthentication("Basic")
                .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);

            services.AddSingleton<IConfiguration>(provider => Configuration);
            var settings = Configuration.GetSection("AuthenticationSettings");
            var typedSettings = settings.Get<AuthenticationSettings>();
            services.Configure<AuthenticationSettings>(settings);
            services.AddSingleton<IAuthenticationSettings>(sp =>
                sp.GetRequiredService<IOptions<AuthenticationSettings>>().Value);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddSingleton<IAppSettings>(sp =>
                sp.GetRequiredService<IOptions<AppSettings>>().Value);
            services.Configure<MongoDBSettings>(Configuration.GetSection("MongoDB"));
            services.AddSingleton<IMongoDBSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

            services.AddSingleton<IConfiguration>(provider => Configuration);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = typedSettings.Issuer,
                        ValidAudience = typedSettings.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(typedSettings.Secret))
                    };
                });

            services.AddTransient<IAuthenticationHandler, BasicAuthenticationHandler>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddHostedService<DataService>();

            services.AddHealthChecks().AddDbContextCheck<DataContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider versionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PostmanApi v1"));
            }

            app.UseCors(options =>
            {
                options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });

            app.UseHttpsRedirection();
            app.UseLoggerMiddleware();
            app.UseTimerMiddlewareAsync();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecks(path: "/howdoyoufeel");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var jsontext = System.IO.File.ReadAllText(@"Data\Contact.json");
            DataSeeder.Seed(jsontext, app.ApplicationServices);
            MongoSeeder.SeedData(app, Configuration);
        }
    }
}
