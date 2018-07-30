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
using Hubtel.PosProxy.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Serialization;
using FluentValidation.AspNetCore;
using Hubtel.PosProxy.BackgroundServices;
using Hubtel.PosProxy.Models.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Hubtel.PosProxy.Filters;
using Microsoft.AspNetCore.HttpOverrides;
using Gelf.Extensions.Logging;
using Hubtel.PosProxy.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Hubtel.PosProxy.Extensions;
using Hubtel.PosProxy.Helpers;

namespace Hubtel.PosProxy
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
            services.Configure<GelfLoggerOptions>(Configuration.GetSection("Graylog"));
            services.PostConfigure<GelfLoggerOptions>(options =>
                            options.AdditionalFields["machine_name"] = Environment.MachineName);

            services.AddLogging(builder => builder.AddConfiguration(Configuration.GetSection("Logging"))
                            .AddConsole()
                            .AddDebug()
                            .AddGelf());
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

            services.AddScoped<IpAttributeFilter>();
            services.AddHostedService<SetupHostedService>();
            services.AddScoped<IPaymentStatusCheckService, PaymentStatusCheckService>();

            services.AddSingleton<IMerchantAccountConfiguration>(Configuration.GetSection("MerchantAccountConfiguration")
                .Get<MerchantAccountConfiguration>());
            services.AddSingleton<IUnifiedSalesConfiguration>(Configuration.GetSection("UnifiedSalesConfiguration")
                .Get<UnifiedSalesConfiguration>());
            services.AddSingleton<IPaymentTypeConfiguration>(Configuration.GetSection("PaymentTypeConfiguration")
                .Get<PaymentTypeConfiguration>());

            services.AddSingleton<IProxyHttpClient, ProxyHttpClient>();
            services.AddSingleton<IMerchantAccountHttpClient, MerchantAccountHttpClient>();
            services.AddSingleton<IUnifiedSalesHttpClient, UnifiedSalesHttpClient>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICashPaymentService, CashPaymentService>();
            services.AddTransient<ICardPaymentService, CardPaymentService>();
            services.AddTransient<IMomoPaymentService, MomoPaymentService>();
            services.AddTransient<IHubtelMePaymentService, HubtelMePaymentService>();
            services.AddTransient<ISalesOrderZipFileService, SalesOrderZipFileService>();
            services.AddTransient<IUnifiedSalesService, UnifiedSalesService>();
            services.AddTransient<IMerchantAccountService, MerchantAccountService>();
            services.AddTransient<IPaymentRequestRepository, PaymentRequestRepository>();
            services.AddTransient<ISalesOrderZipFileRepository, SalesOrderZipFileRepository>();

            //services.AddTransient<IValidator<CreatePaymentRequestDto>, PaymentRequestValidator>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
            })/*.AddFluentValidation(fv => {
                fv.RegisterValidatorsFromAssemblyContaining<DtoToEntityMappingProfile>();
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                fv.ImplicitlyValidateChildProperties = true;
            })*/;

            services.AddMvcCore(options =>
            {
                // My custom auth All endpoints need authentication
                options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                    .Build()));
            })
            .AddAuthorization()
            .AddJsonFormatters(b => b.ContractResolver = new DefaultContractResolver())
            .AddApiExplorer();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "POS Proxy API", Version = "v1" });
                c.OperationFilter<AuthorizationInputOperationFilter>();
                c.OperationFilter<AddFileParamTypesOperationFilter>();
                c.IncludeXmlComments(SwaggerConfigHelper.XmlCommentsFilePath);
            });

            /*var issuer = Configuration.GetValue<string>("HubtelAuth:Issuer");
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
            });*/
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
            }).AddCustomAuth(options =>
            {
                // Configure single or multiple passwords for authentication
                options.AuthKey = "custom auth key";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseForwardedHeaders();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedFor
            });

            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                            context.Response.Headers.Add("Content-Type", "application/json");

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                var jsonErrorResponse = new
                                {
                                    Error = error.Error.Message
                                };
                                var serializeObject = JsonConvert.SerializeObject(jsonErrorResponse);
                                context.Response.AddApplicationError(serializeObject);
                                await context.Response.WriteAsync(serializeObject).ConfigureAwait(false);
                            }
                        });
                });

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
            //app.UseMiddleware<LoggingMiddleware>();
            app.UseMvc();

            app.UseSwagger(c=>
            {
                var basepath = Configuration["SwaggerBaseApiUrl"];
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.BasePath = basepath);
            });

            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "POS Proxy API V1");
            });
        }
    }
}
