using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Filed.PaymentProcess.API.Data.EFCore;
using Filed.PaymentProcess.API.Data;
using Filed.PaymentProcess.API.Model;
using Filed.PaymentProcess.API.Model.BLL;

namespace Filed.PaymentProcess.API
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
            services.AddControllers();

            services.AddDbContext<PaymentDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("PaymentDbContext")));

            RegisterDependencies(services);
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
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient<IRepository<PayDetails>, PayDetailRepository>();
            services.AddTransient<IRepository<PaymentStatus>, PaymentStatusRepository>();
            services.AddTransient<IPaymentHandler, PaymentHandler>();
            services.AddTransient<ICheapPaymentGateway, CheapPaymentGateway>();
            services.AddTransient<IExpensivePaymentGateway, ExpensivePaymentGateway>();
            services.AddTransient<IPremiumPaymentGateway, PremiumPaymentGateway>();
        }
    }
}