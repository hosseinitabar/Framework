using Holism.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Holism.Api
{
    public class Startup
    {
        public static List<Action<IServiceCollection>> ServiceRegistrars = new List<Action<IServiceCollection>>();

        public static Action<IApplicationBuilder> IdentityMiddlewareRegistrar;

        private static List<Assembly> AssembliesToSearchForControllers { get; set; }

        public static bool NeedsUploadingBigFiles { get; set; }

        static Startup()
        {
            AssembliesToSearchForControllers = new List<Assembly>();
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static void AddControllerSearchAssembly(Assembly assembly)
        {
            if (AssembliesToSearchForControllers.Any(i => i.FullName == assembly.FullName))
            {
                return;
            }
            AssembliesToSearchForControllers.Add(assembly);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            var mvcBuilder = services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new ListParametersModelBinderProvider());
                options.EnableEndpointRouting = false;
                //options.Filters.Add(typeof(ProtectedIdResultFilter));
            });
            foreach (var controllerAssembly in AssembliesToSearchForControllers)
            {
                try
                {
                    mvcBuilder.AddApplicationPart(controllerAssembly).AddControllersAsServices();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    Logger.LogError($"Error adding application part for {controllerAssembly.GetName().Name}");
                }
            }
            AddMvcService(services);
            // if (Config.CorsOriginsSpecified)
            // {
            //     services.AddCors();
            // }
            services.AddCors(o => o.AddPolicy("AllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            if (NeedsUploadingBigFiles)
            {
                services.Configure<FormOptions>(x =>
                {
                    x.ValueLengthLimit = int.MaxValue;
                    x.MultipartBodyLengthLimit = int.MaxValue;
                });
            }
            foreach (var serviceRegistrar in ServiceRegistrars)
            {
                serviceRegistrar.Invoke(services);
            }
        }

        private static void AddMvcService(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new ReferenceTypeBodyJsonBindingConvention());
                options.Filters.Add(new ModelChecker());
            });
            //.AddJsonOptions(JsonHelper.Options);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MiddlewareExtensions.UseExceptionHandler(app);

            //app.UseExceptionHandler();

            //app.UseServiceLengthner();

            if (Config.RedirectAllHttpRequestsToHttps)
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseRouter(router =>
            {
                router.MapGet(".well-known/acme-challenge/{id}", async (request, response, routeData) =>
                {
                    var id = routeData.Values["id"] as string;
                    var file = Path.Combine(AppContext.BaseDirectory, ".well-known", "acme-challenge", id);
                    await response.SendFileAsync(file);
                });
            });

            IdentityMiddlewareRegistrar?.Invoke(app);

            if (Config.DelayApiResponsesGlobally)
            {
                app.UseApiDelayedResponse();
            }

            // app.UseCors(options =>
            // {
            //     options.SetIsOriginAllowedToAllowWildcardSubdomains();
            //     foreach (var item in Config.CorsOrigins)
            //     {
            //         options.WithOrigins(Config.CorsOrigins).SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //     }
            // });

            app.UseCors("AllOrigins");

            DisableCacheEntirely(app);

            HttpContextHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            app.UseMvc(options =>
            {
                options.MapRoute("Default", "{controller=Default}/{action=Index}/{id?}");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(name: "Default", pattern: "{controller=Default}/{action=Index}/{id?}");
            //});
        }

        public void DisableCacheEntirely(IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
                return next.Invoke();
            });
        }
    }
}