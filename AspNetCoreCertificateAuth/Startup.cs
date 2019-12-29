using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreCertificateAuth
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
            services.AddHttpClient();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            var clientCertificateIntermediate = new X509Certificate2("../Certs/client_intermediate_localhost.pfx", "1234");
            var handlerClientCertificateIntermediate = new HttpClientHandler();
            handlerClientCertificateIntermediate.ClientCertificates.Add(clientCertificateIntermediate);

            services.AddHttpClient("client_intermediate_localhost", c => {})
                .ConfigurePrimaryHttpMessageHandler(() => handlerClientCertificateIntermediate);

            var certificateIntermediate = new X509Certificate2("../Certs/intermediate_localhost.pfx", "1234");
            var handlerCertificateIntermediate = new HttpClientHandler();
            handlerCertificateIntermediate.ClientCertificates.Add(certificateIntermediate);

            services.AddHttpClient("intermediate_localhost", c => { })
                .ConfigurePrimaryHttpMessageHandler(() => handlerCertificateIntermediate);

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
