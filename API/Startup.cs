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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Quartz.Impl;
using Swashbuckle.AspNetCore.Filters;
using System.IO;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Clear();
            Log.Clear("ef_log.txt");
#if DEBUG
            Log.Write("Debug");
            services.AddDbContext<APIContext>(options => options.UseSqlServer(Configuration.GetConnectionString("APIContext")).EnableSensitiveDataLogging());
#else
                        Log.Write("Release");
                        try
                        {
                            Log.Write(Config.DbConnectionString);
                            services.AddDbContext<APIContext>(options => options.UseSqlServer(Config.DbConnectionString));
                        }
                        catch(Exception ex)
                        {
                            Log.Write(ex.ToString());
                        }
#endif
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = AuthOptions.ISSUER,

                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = AuthOptions.AUDIENCE,
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,

                            // установка ключа безопасности
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });

            Log.Write("Connection complete");
            services.AddControllers();
            services.AddSwaggerExamples();
            services.AddSwaggerGen(options =>
            {
                options.ExampleFilters();

                //options.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request");

                //options.OperationFilter<AddResponseHeadersFilter>();

                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "API.xml");
                //options.IncludeXmlComments(filePath);

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.EnableAnnotations();
                options.OperationFilter<SecurityRequirementsOperationFilter>();

                options.IgnoreObsoleteProperties();
            });
            
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //только для разработки
            services.AddCors(options =>
            {
                options.AddPolicy("DevCorsPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, APIContext apiContext)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("DevCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseMiddleware<TokenMiddleware>();
            Log.Write("TokenMiddleware complete");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
