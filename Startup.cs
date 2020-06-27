using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Icomm.NewsCrawl.Website.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Icomm.NewsCrawl.Website
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
            services.AddControllers()
            .AddNewtonsoftJson(ops =>
             {
                 ops.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                 ops.SerializerSettings.ContractResolver = null;// new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
             });
            services.AddControllersWithViews();
            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
            });

#pragma warning disable CS0618 // Type or member is obsolete
            services.AddDbContext<news_crawlContext>(opt => opt.UseSqlServer(this.Configuration.GetConnectionString("news_crawl"), opt => opt.UseRowNumberForPaging()));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Crawl}/{id?}");
            });
            app.UseMvc();
        }
    }
}
