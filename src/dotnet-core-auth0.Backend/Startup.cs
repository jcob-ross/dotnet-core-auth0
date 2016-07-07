namespace dotnet_core_auth0.Backend
{
  using System.Security.Claims;
  using Infrastructure;
  using Infrastructure.Attributes;
  using Infrastructure.Authentication;
  using Infrastructure.Deployment;
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;

  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
        .AddEnvironmentVariables();

      if (env.IsDevelopment())
        builder.AddUserSecrets();

      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors();
      services.AddMemoryCache();
      services.AddOptions();
      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

      services.AddMvc(options =>
      {
        // exeptions for requests with Content-Type: application/json should return json
        options.Filters.Add(new ApiExceptionFilterAttribute());
      });
      
      services.AddAuthorization(options =>
      {
        // see SampleApiController - OwnerOnly()
        options.AddPolicy("OwnerOnly", policy =>
        {
          policy.RequireClaim(ClaimTypes.NameIdentifier, Configuration.GetValue<string>("OwnerId"));
        });
      });

      services.Configure<Auth0Settings>(Configuration.GetSection("Auth0Settings"));
      
      // for deployment information, see bottom of _Layout.cshtml
      services.AddSingleton<IDeploymentEnvironment, DeploymentEnvironment>();

      // ISSUE: ClaimsTransformation can run multiple times
      // https://github.com/aspnet/Security/issues/284
      services.AddScoped<IClaimsTransformer, AuthClaimsTransformer>();
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

      app.UseAuth0JwtAuthentication("http");

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
  }
}