using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Infra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations.Rules;

namespace API
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddSwaggerGen(o=>
            {
                o.SwaggerDoc("v1.0", new OpenApiInfo {

                    Version="v1.0",
                    Title = "ToDo API",
                    Description = "Asp.Net Core Web API & Swashbuckle Implementation",
                    TermsOfService = new Uri("http://localhost"),
                    Contact=new OpenApiContact
                    { 
                        Name="Test Name",
                        Email=string.Empty,
                        Url=new Uri("http://localhost")
                    },
                    License=new OpenApiLicense
                    { 
                        Name="Use Under MIT",
                        Url=new Uri("http://localhost")
                    }
                    
                }) ;

                o.SwaggerDoc("v2.0", new OpenApiInfo
                {

                    Title = "Api Versioning using Swashbuckle",
                    Version = "v2.0"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                o.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(o=> {
                o.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1");

                });
        }
    }
}
