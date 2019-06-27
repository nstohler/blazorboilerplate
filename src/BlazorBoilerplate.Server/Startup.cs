using BlazorBoilerplate.Server.Data;
using BlazorBoilerplate.Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BlazorBoilerplate.Server.Services;
using BlazorBoilerplate.Server.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using BlazorBoilerplate.Startup;

namespace BlazorBoilerplate.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private IContainer _applicationContainer;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Filename=data.db"));  // Sql Lite / file database
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))); //SQL Server Database

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
                AdditionalUserClaimsPrincipalFactory>();

            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            //Add Policies / Claims / Authorization - https://stormpath.com/blog/tutorial-policy-based-authorization-asp-net-core
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireElevatedRights", policy => policy.RequireRole("SuperAdmin", "Admin"));
                options.AddPolicy("ReadOnly", policy => policy.RequireClaim("ReadOnly", "true"));
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                //options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;
                //options.SignIn.RequireConfirmedEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;

                // Suppress redirect on API URLs in ASP.NET Core -> https://stackoverflow.com/a/56384729/54159
                options.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = (int) (HttpStatusCode.Unauthorized);
                        }

                        return Task.CompletedTask;
                    },
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version     = "v1";
                    document.Info.Title       = "Blazor Boilerplate";
                    document.Info.Description = "Blazor Boilerplate / Starter Template using the  (ASP.NET Core Hosted) (dotnet new blazorhosted) model. Hosted by an ASP.NET Core server";
                };
            });

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();

            // https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html#quick-start-without-configurecontainer

            //// Create the container builder.
            //var builder = new ContainerBuilder();

            //// Register dependencies, populate the services from
            //// the collection, and build the container.
            ////
            //// Note that Populate is basically a foreach to add things
            //// into Autofac that are in the collection. If you register
            //// things in Autofac BEFORE Populate then the stuff in the
            //// ServiceCollection can override those things; if you register
            //// AFTER Populate those registrations can override things
            //// in the ServiceCollection. Mix and match as needed.
            //builder.Populate(services);
            ////builder.RegisterType<MyType>().As<IMyType>();
            
            //// autofac registrations go here
            //// TODO: use Bootstrapper
            
            //builder.RegisterModule(new AutofacBootstrapModule());

            //this.ApplicationContainer = builder.Build();

            var assemblies = AutofacBootstrapModule.GetAssemblies().ToList();
            services.AddAutoMapper(assemblies);

            _applicationContainer = Bootstrapper.Initialize(builder =>
            {
                builder.Populate(services);

                
            });

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(_applicationContainer);
        }

        //// ConfigureContainer is where you can register things directly
        //// with Autofac. This runs after ConfigureServices so the things
        //// here will override registrations made in ConfigureServices.
        //// Don't build the container; that gets done for you. If you
        //// need a reference to the container, you need to use the
        //// "Without ConfigureContainer" mechanism shown later.
        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    builder.RegisterModule(new AutofacBootstrapModule());
            
        //    builder.RegisterModule(new AutofacModule());
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
            }

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseClientSideBlazorFiles<Client.Startup>();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // NSwag
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");
            });
        }
    }
}
