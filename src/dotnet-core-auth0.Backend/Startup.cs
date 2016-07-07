namespace dotnet_core_auth0.Backend
{
  using System;
  using System.Security.Claims;
  using System.Threading.Tasks;
  using AspNet.Security.OpenIdConnect.Extensions;
  using Data.Context;
  using Data.Models;
  using Infrastructure;
  using Infrastructure.Attributes;
  using Infrastructure.Deployment;
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.AspNetCore.Authentication.Cookies;
  using Microsoft.AspNetCore.Authentication.JwtBearer;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
  using Microsoft.Data.Sqlite;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;

  public class Startup
  {
    private SqliteConnection _inMemorySqliteConnection;
    private readonly IHostingEnvironment _environment;
    public Startup(IHostingEnvironment env)
    {
      IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
        .AddEnvironmentVariables();

      if (env.IsDevelopment())
        builder.AddUserSecrets();

      _environment = env;

      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors();
      services.AddMemoryCache();
      services.AddOptions();
      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

      services.AddMvcCore(o =>
        {
          o.Filters.Add(new ApiExceptionFilterAttribute());
        })
        .AddApiExplorer()
        .AddAuthorization(o =>
          {
            o.AddPolicy("COOKIES", o.DefaultPolicy); 
            o.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireClaim(OpenIdConnectConstants.Claims.Scope, "api.rw")
                        .Build();

            o.AddPolicy("OwnerOnly", policy => policy.RequireClaim(ClaimTypes.Email, "admin@it.io"));
          })
        .AddDataAnnotations()
        .AddFormatterMappings()
        .AddJsonFormatters();

      #region authorization server

      services.AddDbContext<SpaDbContext>(options =>
      {
        if (_environment.IsDevelopment())
        {
          _inMemorySqliteConnection = new SqliteConnection("Data Source=:memory:");
          _inMemorySqliteConnection.Open();
          options.UseSqlite(_inMemorySqliteConnection);
        }
        else
        {
          options.UseSqlite(Configuration.GetConnectionString("SqliteDbFile"));
        }
      });

      // register Identity services
      services.AddIdentity<SpaUser, IdentityRole<Guid>>(setup =>
        {
          setup.Password.RequireDigit = false;
          setup.Password.RequireLowercase = false;
          setup.Password.RequireUppercase = false;
          setup.Password.RequireNonAlphanumeric = false;
          setup.Password.RequiredLength = 4;

          setup.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                                                   {
                                                     OnRedirectToLogin = OpenIddictRedirectFilter,
                                                     OnRedirectToAccessDenied = OpenIddictRedirectFilter
                                                   };

          setup.Cookies.ApplicationCookie.AccessDeniedPath = "/connect/login";
          setup.Cookies.ApplicationCookie.LoginPath = "/connect/login";
          setup.Cookies.ApplicationCookie.LogoutPath = "/connect/logout";
          setup.Cookies.ApplicationCookie.CookiePath = "/connect/";
        })
        .AddEntityFrameworkStores<SpaDbContext, Guid>()
        .AddDefaultTokenProviders();

      services.AddOpenIddict<SpaUser, IdentityRole<Guid>, SpaDbContext, Guid>()
        .SetTokenEndpointPath("/connect/token")
        .SetAuthorizationEndpointPath("/connect/authorize")
        .SetLogoutEndpointPath("/connect/logout")
        .SetErrorHandlingPath("/connect/error")
        .UseJsonWebTokens()
        .Configure(o =>
                   {
                     o.ApplicationCanDisplayErrors = true;
                     o.AllowInsecureHttp = true;
                     o.AutomaticAuthenticate = true;
                     o.AutomaticChallenge = true;
                   })
        // When using your own authorization controller instead of using the
        // MVC module, you need to configure the authorization/logout paths:
        //.SetLogoutEndpointPath("/connect/logout")

        // Note: if you don't explicitly register a signing key, one is automatically generated and
        // persisted on the disk. If the key cannot be persisted, an exception is thrown.

        // You can also store the certificate as an embedded .pfx resource
        // directly in this assembly or in a file published alongside this project:

        //.AddSigningCertificate(
        //  assembly: typeof(Startup).GetTypeInfo().Assembly,
        //  resource: "dotnet-core-auth0.Backend.Certificate.pfx",
        //  password: "OpenIddict")

        .DisableHttpsRequirement();

      #endregion authorization server
      
      // for deployment information, see bottom of _Layout.cshtml
      services.AddSingleton<IDeploymentEnvironment, DeploymentEnvironment>();

      services.AddTransient<IOpenIddictDbInitializer, OpenIddictDbInitializer>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      var deploymentEnv = app.ApplicationServices.GetService<IDeploymentEnvironment>();
      deploymentEnv.Initialize();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        
        app.UseCors(policy => {
          policy.AllowAnyHeader();
          policy.AllowAnyOrigin();
          policy.AllowAnyMethod();
          policy.AllowCredentials();
          policy.Build();
        });
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      #region auth validation



      #endregion auth validation

      #region auth server
      // https://github.com/openiddict/openiddict-core/blob/dev/samples/Mvc.Server/Startup.cs
      // todo: content security policy

      // Add a middleware used to validate access
      // tokens and protect the API endpoints.
      app.UseOAuthValidation();

      // Alternatively, you can also use the introspection middleware.
      // Using it is recommended if your resource server is in a
      // different application/separated from the authorization server.
      // 
      // app.UseOAuthIntrospection(options => {
      //     options.AutomaticAuthenticate = true;
      //     options.AutomaticChallenge = true;
      //     options.Authority = "http://localhost:8080/";
      //     options.Audience = "resource_server";
      //     options.ClientId = "resource_server";
      //     options.ClientSecret = "875sqd4s5d748z78z7ds1ff8zz8814ff88ed8ea4z4zzd";
      // });

      app.UseIdentity();
      // app.UseGoogleAuthentication(...)
      // app.UseTwitterAuthentication(...)

      app.UseOpenIddict();
      
      #endregion auth server

      app.UseClaimsTransformation(ctx =>
      {
        var transformer = ctx.Context.RequestServices.GetRequiredService<IClaimsTransformer>();
        return transformer.TransformAsync(ctx);
      });

      app.UseMvc(routes =>
                 {
                   routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

                   // for non-existing routes/assets:
                   // /some/route/foo.bar will return 404
                   // /some/route/foobar will map to spa (index page)
                   // https://github.com/aspnet/JavaScriptServices/blob/dev/samples/angular/MusicStore/Startup.cs#L84
                   routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
                 });
    }

    private static Task OpenIddictRedirectFilter(CookieRedirectContext ctx)
    {
      if (ctx.Request.Path.StartsWithSegments("/connect"))
      {
        ctx.Response.Redirect(ctx.RedirectUri);
      }
      return Task.FromResult(0);
    }
  }
}