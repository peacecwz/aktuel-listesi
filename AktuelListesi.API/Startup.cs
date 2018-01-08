﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AktuelListesi.AppService;
using AktuelListesi.AppService.Interfaces;
using AktuelListesi.Crawler;
using AktuelListesi.Crawler.Interfaces;
using AktuelListesi.Repository;
using AktuelListesi.Service;
using AktuelListesi.Service.Implementations;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace AktuelListesi.API
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

            services.AddDbContext<AktuelDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("AktuelDbConnection"), opt => opt.MigrationsAssembly("AktuelListesi.API")), contextLifetime: ServiceLifetime.Singleton, optionsLifetime: ServiceLifetime.Singleton);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            var mapper = config.CreateMapper();
            services.AddSingleton<IMapper>(mapper);


            services.AddSingleton(typeof(IRepository<,>), typeof(Repository<,>));

            services.AddSingleton<IAktuelPageService, AktuelPageService>();
            services.AddSingleton<IAktuelService, AktuelService>();
            services.AddSingleton<ICompanyService, CompanyService>();

            services.AddSingleton<ICrawlerService, CrawlerService>();
            services.AddSingleton<IOneSignalService, OneSignalService>();
            services.AddSingleton<IUploadService, UploadService>();
            services.AddSingleton<IQueueService, QueueService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Aktuel Listesi API", Version = "v1" });
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aktuel Listesi V1");
            });

            app.UseMvc();
        }
    }
}
