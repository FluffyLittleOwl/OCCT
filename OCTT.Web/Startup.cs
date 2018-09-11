using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using OCTT.Domain.Abstract;
using OCTT.Domain.Concrete;
using OCTT.Domain.Helpers;

namespace OCTT.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            Configuration = (new ConfigurationBuilder()).BuildOCTT(basePath: hostingEnvironment.ContentRootPath);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /// thats how I did it once for a some task, will keep just in case, even here
            /// cd 'C:\Users\Kirill\Documents\Visual Studio 2015\Projects\ShopData\src\ShopData.Web'
            /// dotnet ef migrations add initial -c ApplicationDbContext

            services.AddDbContext<RSSDbContext>(options =>
                options.UseSqlServer(Configuration["Data:ConnectionString"], b => b.MigrationsAssembly("OCTT.Web")));

            services.AddScoped<IRSSRecordRepository, RSSRecordRepository>();
            services.AddScoped<IRSSFeedRepository, RSSFeedRepository>();

            services.AddMvc().AddJsonOptions(opts =>
            {
                // https://chsakell.com/2016/06/23/rest-apis-using-asp-net-core-and-entity-framework-core/
                // Force Camel Case to JSON, which I did not use it looks like
                opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc();

            // app.Run(async (context) =>
            // {
            //     await context.Response.WriteAsync("Hello World!");
            // });
        }
    }
}
