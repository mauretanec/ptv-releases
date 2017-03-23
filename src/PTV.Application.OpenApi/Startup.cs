﻿/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess;
using PTV.Framework;
using PTV.Framework.Extensions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using PTV.Database.DataAccess.EntityCloners;

namespace PTV.Application.OpenApi
{
    class ProtocolValidator : OpenIdConnectProtocolValidator
    {
        protected override void ValidateState(OpenIdConnectProtocolValidationContext validationContext)
        {
        }
        protected override void ValidateAtHash(OpenIdConnectProtocolValidationContext validationContext)
        {

        }
    }

    /// <summary>
    /// Initilizer of app
    /// </summary>
    public class Startup
    {
        private readonly IHostingEnvironment appEnv;

        /// <summary>
        /// Constructor of initializer
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            this.appEnv = env;

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            PtvConfiguration = new ApplicationConfiguration(Configuration);
        }

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }

        /// <summary>
        /// PTV type of configuration
        /// </summary>
        public ApplicationConfiguration PtvConfiguration { get; set; }

        /// <summary>
        /// Configure and register all services needed for application
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(
                c => c.Conventions.Add(new ControllerModelRemoveVersionPrefixConvention())
                );
            services.AddMvcCore().AddJsonFormatters();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));

            services.AddSingleton(PtvConfiguration);
            services.Configure<CookieAuthenticationOptions>(options => Configuration.GetSection("Data:CookieAuthenticationConfigurations").Bind(options));
            services.Configure<OpenIdConnectOptions>(options => Configuration.GetSection("Data:OpenIDConnectConfigurations").Bind(options));
            services.Configure<IdentityServerAuthenticationOptions>(options => Configuration.GetSection("Data:IdentityServerAuthenticationConfigurations").Bind(options));
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("Data:ProxyServerSettings").Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<AnnotationServiceConfiguration>(options => Configuration.GetSection("Data:AnnotationService").Bind(options));
            services.Configure<MapServiceConfiguration>(options => Configuration.GetSection("Data:MapService").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterServiceManager.RegisterFromAllAssemblies(services);
            BaseEntityCloners.RegisterBaseEntityCloners(services);
            RegisterDataProviderServices.RegisterFromAssembly(services);
            FrameworksInitializer.RegisterEntityFramework(services, Configuration["Data:DefaultConnection:ConnectionString"]);
            services.AddSwaggerGen(
                c => {
                    c.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = "PTV Open Api",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });
                    c.SwaggerDoc("v2", new Info
                    {
                        Version = "v2",
                        Title = "PTV Open Api version 2",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });
                    c.SwaggerDoc("v3", new Info
                    {
                        Version = "v3",
                        Title = "PTV Open Api version 3",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });
                    c.SwaggerDoc("v4", new Info
                    {
                        Version = "v4",
                        Title = "PTV Open Api version 4",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });

                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                    c.DocInclusionPredicate((version, apiDescription) => ResolveVersionSupportByRouteConstraint(apiDescription, version));

                    //Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                    c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = $"{Configuration["STS"]}/connect/authorize",
                        Scopes = new Dictionary<string, string>
                        {
                            { "dataEventRecords", "Access event records" }
                        }
                    });
                });

            var path = appEnv.IsDevelopment() ? string.Format(@"{0}\bin\Debug\netcoreapp1.1", appEnv.ContentRootPath) : appEnv.ContentRootPath;
            var xmlCommentPath = Path.Combine(path, @"PTV.Application.OpenApi.xml");
            var modelsXmlPath = Path.Combine(path, @"PTV.Domain.Model.xml");
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.IncludeXmlComments(xmlCommentPath);
                if (File.Exists(modelsXmlPath))
                {
                    options.IncludeXmlComments(modelsXmlPath);
                }
            });
        }

        private static bool ResolveVersionSupportByRouteConstraint(ApiDescription apiDesc, string targetApiVersion)
        {
            var path = apiDesc.RelativePath;
            if (targetApiVersion == "v1")
            {
                return path.Contains("api/v") ? false : true;
            }

            return path.Contains(targetApiVersion) ? true : (path.Contains("api/v{version}") ? true : false);
        }

        /// <summary>
        /// Security requirement operation filter
        /// </summary>
        /// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" />
        public class SecurityRequirementsOperationFilter : IOperationFilter
        {
            /// <summary>
            /// Applies filter to the specified operation.
            /// </summary>
            /// <param name="operation">The operation.</param>
            /// <param name="context">The context.</param>
            public void Apply(Operation operation, OperationFilterContext context)
            {
                // Policy names map to scopes
                var controllerScopes = context.ApiDescription.ControllerAttributes()
                    .OfType<AuthorizeAttribute>()
                    .Select(attr => attr.Policy);

                var actionScopes = context.ApiDescription.ActionAttributes()
                    .OfType<AuthorizeAttribute>()
                    .Select(attr => attr.Policy);

                var anonymousAccess = context.ApiDescription.ActionAttributes()
                    .OfType<AllowAnonymousAttribute>().Any();

                var requiredScopes = controllerScopes.Union(actionScopes).Distinct();

                if (requiredScopes.Any() && !anonymousAccess)
                {
                    operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                    operation.Security.Add(new Dictionary<string, IEnumerable<string>>
                    {
                        { "oauth2", new List<string>() { "dataEventRecords" } }
                    });
                }
            }
        }

        /// <summary>
        /// Convetion for model prefix
        /// </summary>
        /// <seealso cref="Microsoft.AspNetCore.Mvc.ApplicationModels.IControllerModelConvention" />
        public class ControllerModelRemoveVersionPrefixConvention : Microsoft.AspNetCore.Mvc.ApplicationModels.IControllerModelConvention
        {
            /// <summary>
            /// Called to apply the convention to the <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.
            /// </summary>
            /// <param name="controller">The <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.</param>
            public void Apply(Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel controller)
            {
                controller.ControllerName = Regex.Replace(controller.ControllerName, "V[0-9]+", string.Empty);
            }
        }

        /// <summary>
        /// Configure additional runtime pipeline middlewares
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();
            var switchRequestsToHttpsString = Configuration["Data:SwitchRequestsToHttps"];
            bool switchRequestsToHttps = false;
            bool.TryParse(switchRequestsToHttpsString, out switchRequestsToHttps);
            if (switchRequestsToHttps)
            {
                app.UseHttpsSwitch();
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var cookieOptions = app.ApplicationServices.GetRequiredService<IOptions<CookieAuthenticationOptions>>().Value;
            var openIdOptions = app.ApplicationServices.GetRequiredService<IOptions<OpenIdConnectOptions>>().Value;
            var identityServerOptions = app.ApplicationServices.GetRequiredService<IOptions<IdentityServerAuthenticationOptions>>().Value;
            app.UseCookieAuthentication(cookieOptions);
            openIdOptions.Authority = Configuration["STS"];
            openIdOptions.ProtocolValidator = new ProtocolValidator() { RequireState = true };
            openIdOptions.TokenValidationParameters.ValidateIssuerSigningKey = false;
            app.UseOpenIdConnectAuthentication(openIdOptions);
            identityServerOptions.Authority = Configuration["STS"];
            identityServerOptions.AutomaticChallenge = true;
            identityServerOptions.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Jwt;
            identityServerOptions.AdditionalScopes = new List<string>() { "email", "role", "openid", "dataEventRecords" };
            app.UseIdentityServerAuthentication(identityServerOptions);
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v4/swagger.json", "Version 4");
                c.SwaggerEndpoint("/swagger/v3/swagger.json", "Version 3");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Version 2");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1");

                c.EnabledValidator(null);
                c.DocExpansion("none");
                c.RoutePrefix = "swagger/ui";
                c.ConfigureOAuth2(openIdOptions.ClientId, openIdOptions.ClientSecret, "swagger-ui-realm", openIdOptions.ClientId);
            });
            app.ApplicationServices.GetService<IPrefilteringManager>().Initialize();
        }
    }
}
