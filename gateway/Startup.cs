using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Gateway.Messaging;
using Microsoft.OpenApi.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Gateway
{
    public class Startup
    {

        string _title;
        string _version;
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }



        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;

            _title = Configuration.GetSection("ASPNETCORE_TITLE").Value;
            _version = Configuration.GetSection("ASPNETCORE_VERSION").Value;

        }


        // This method gets called by the runtime. Add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //add jsend error responses
            //TODO - future implementation

            //add configuration
            services.AddSingleton(Configuration);


            //add endpoint controllers
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            //add needed singletons for consumers
            MessageRepository messageRepository = new MessageRepository();
            services.AddSingleton<MRabbitMQ>();
            services.AddSingleton(messageRepository);

            //add consumers
            //TODO add more consumers if too much load
            services.AddHostedService<FitModelConsumer>();
            services.AddHostedService<ForecastConsumer>();

            //Add swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_version, new OpenApiInfo { Title = _title, Version = _version });

            });


        }

        // This method gets called by the runtime. Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Enable Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/" + _version + "/swagger.json", _title);
                //c.RoutePrefix = string.Empty;
            });

            //add routes
            app.UseRouting();

            //add endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
