using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;
using API.BusinessLogic;
using API.Contexts;
using API.Contexts.Interfaces;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly string _version;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Enable CORS
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Configure jsonOptions for Controllers
            var jsonOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include
            };

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = jsonOptions.ContractResolver;
                options.SerializerSettings.DefaultValueHandling = jsonOptions.DefaultValueHandling;
                options.SerializerSettings.NullValueHandling = jsonOptions.NullValueHandling;
            });

            services.AddSingleton<JsonSerializerSettings>(jsonOptions);

            // Add Configuration Context
            services.Configure<ConfigurationContext>(_configuration.GetSection("ConfigurationContext"));
            var configContext = _configuration.GetSection(nameof(ConfigurationContext)).Get<ConfigurationContext>();
            services.AddSingleton<IConfigurationContext>(configContext);

            // Add MongoDb
            services.AddSingleton(configContext.MongoDbConfiguration);
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            // Add Swagger UI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_version, new OpenApiInfo
                {
                    Contact = new OpenApiContact
                    {
                        Email = "mike.avgeros@gmail.com",
                        Name = "Mike Avgeros"
                    },
                    Description = "Provides access to the API",
                    License = null,
                    TermsOfService = null,
                    Title = Assembly.GetExecutingAssembly().GetName().Name,
                    Version = _version
                });

            });

            // Add Business Logic
            services.AddTransient<UploadLogic>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"./{_version}/swagger.json", "Synth Logic");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
