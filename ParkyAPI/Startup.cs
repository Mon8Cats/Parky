using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParkyAPI.Data;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using AutoMapper;
using ParkyAPI.ParkyMapper;
using System.Reflection;
using System.IO;

namespace ParkyAPI
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
            services.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddAutoMapper(typeof(ParkyMappings));
            services.AddSwaggerGen(options=>
            {
                // add swagger document
                // uri for document
                // http://localhost:59182/swagger/ParkyOpenApiSpec/swagger.json
                options.SwaggerDoc("ParkyOpenApiSpec",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Parky API",
                        Version = "1",
                        Description = "My API Project",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "steve@test.com",
                            Name = "steve test",
                            Url = new Uri("http://www.dummy.com")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                        {
                            Name = "My Test License",
                            Url = new Uri("http://www.dummylicense.org")
                        }

                    });

                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                options.IncludeXmlComments(cmlCommentsFullPath);
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            // swagger end point -- uri and name then we can use the following URI
            // http://localhost:59182/swagger/index.html
            app.UseSwaggerUI(options=>
            {
                options.SwaggerEndpoint("/swagger/ParkyOpenApiSpec/swagger.json", "Parky API");
                options.RoutePrefix = ""; // then change lauchsettings remove lauchURl
                // then http://localhost:59182/index.html works fine
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
