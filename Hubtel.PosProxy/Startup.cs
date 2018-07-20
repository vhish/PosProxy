using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Hubtel.PosProxy.Lib;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Dtos;
using Hubtel.PosProxy.Services;
using Hubtel.PosProxyData.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using Hubtel.PosProxyData.Repositories;

namespace Hubtel.PosProxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Redis = ConnectionMultiplexer.Connect(Configuration["Redis:ConnectionString"]);
        }

        public IConfiguration Configuration { get; }
        //public static ConnectionMultiplexer Redis;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddCors();

            services.AddDistributedMemoryCache();
            /*services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["Redis:ConnectionString"];
                option.InstanceName = Configuration["Redis:InstanceName"];
            });*/

            string migrationsAssembly = "Hubtel.PosProxyData";
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(migrationsAssembly)).UseInternalServiceProvider(serviceProvider));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DtoToEntityMappingProfile());
                cfg.CreateMissingTypeMaps = true;
                cfg.AddCollectionMappers();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper();

            services.AddSingleton<IMerchantAccountConfiguration>(Configuration.GetSection("MerchantAccountConfiguration")
                .Get<MerchantAccountConfiguration>());
            services.AddSingleton<IUnifiedSalesConfiguration>(Configuration.GetSection("UnifiedSalesConfiguration")
                .Get<UnifiedSalesConfiguration>());
            services.AddSingleton<IPaymentTypeConfiguration>(Configuration.GetSection("PaymentTypeConfiguration")
                .Get<PaymentTypeConfiguration>());

            services.AddSingleton<IProxyHttpClient, ProxyHttpClient>();
            services.AddSingleton<IMerchantAccountHttpClient, MerchantAccountHttpClient>();
            services.AddSingleton<IUnifiedSalesHttpClient, UnifiedSalesHttpClient>();
            services.AddTransient<ICashPaymentService, CashPaymentService>();
            services.AddTransient<ICardPaymentService, CardPaymentService>();
            services.AddTransient<IMomoPaymentService, MomoPaymentService>();
            //services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IUnifiedSalesService, UnifiedSalesService>();
            services.AddTransient<IMerchantAccountService, MerchantAccountService>();
            services.AddTransient<IPaymentRequestRepository, PaymentRequestRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
            }); ;

            services.AddMvcCore().AddApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "POS Proxy API", Version = "v1" });
                c.OperationFilter<AuthorizationInputOperationFilter>();
            });

            var issuer = Configuration.GetValue<string>("HubtelAuth:Issuer");
            var audience = Configuration.GetValue<string>("HubtelAuth:Audience");
            var key = Configuration.GetValue<string>("HubtelAuth:Key");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "POS Proxy API V1");
            });
        }
    }
}
