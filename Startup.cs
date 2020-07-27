using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMOServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Http;
using MMOServer.Security;
using System;

// https://www.youtube.com/watch?v=qJmEI2LtXIY
// https://www.youtube.com/watch?v=xMktEpPmadI&list=PL6n9fhu94yhVkdrusLaQsfERmL_Jh4XmU&index=48

// SQL Server TUtorials:
// https://www.youtube.com/watch?v=ETepOVi7Xk8&list=PL08903FB7ACA1C2FB

// DataBase Migration:
// https://www.youtube.com/watch?v=MhvOKHUWgiY
// Add-Migration Initial -IgnoreChanges
// Add-Migration <name>
// Update-Database

// Part 117
// https://www.youtube.com/watch?v=fOQjWUokhn8&list=PL6n9fhu94yhVkdrusLaQsfERmL_Jh4XmU&index=117

namespace MMOServer
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
      // Create a Pool, reuse instances in Context.
      services.AddDbContextPool<MMODbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MMODBConnection")));

      // Account Creation and options
      services.AddIdentity<Account, IdentityRole>(options =>
      {
        options.SignIn.RequireConfirmedEmail = true;
        options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
      })
      .AddEntityFrameworkStores<MMODbContext>()
      .AddDefaultTokenProviders()
      .AddTokenProvider<CustomEmailConfirmationTokenProvider<Account> >("CustomEmailConfirmation");

      // Changes token lifespan of all token types to 5 hours
      services.Configure<DataProtectionTokenProviderOptions>(o =>
              o.TokenLifespan = TimeSpan.FromHours(5));

      // Changes token lifespan of just the Email Confirmation Token type to 3 days
      services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
              o.TokenLifespan = TimeSpan.FromDays(3));

      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequireNonAlphanumeric = false;
      });



      services.AddMvc(options =>
      {
        options.EnableEndpointRouting = false;

        var policy = new AuthorizationPolicyBuilder()
                         .RequireAuthenticatedUser()
                         .Build();
        options.Filters.Add(new AuthorizeFilter(policy));

      }).AddXmlSerializerFormatters();


      services.AddAuthentication()
        .AddGoogle(options =>
        {
          // https://console.developers.google.com
          options.ClientId = "forbidden";
          options.ClientSecret = "forbidden";
        });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("DeleteRolePolicy",
          policy => policy.RequireClaim("Delete Role"));

        options.AddPolicy("EditRolePolicy",
          policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

        options.AddPolicy("AdminRolePolicy",
          policy => policy.RequireRole("Admin"));
      });


      // use Repository pattern
      services.AddScoped<IMMORepository, SqlMMORepository>();

      services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
      services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
      services.AddSingleton<DataProtectionPurposeStrings>();

      services.ConfigureApplicationCookie(options =>
      {
        options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
      });

      services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // re-execute the pipeline and return the original status code (see ErrorController.cs)
        app.UseExceptionHandler("/Error");
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
      }


      // must be before static files, use default.htlm
      //app.UseDefaultFiles();
      app.UseStaticFiles();
      // app.UseFileServer() combines both

      app.UseAuthentication();

      //app.UseMvcWithDefaultRoute();
      app.UseMvc(routes =>
      {
        routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
      });
      //app.UseMvc();

      //app.UseHttpsRedirection();
      //
      //app.UseRouting();
      //
      //app.UseAuthorization();
      //
      //app.UseEndpoints(endpoints =>
      //{
      //  endpoints.MapControllers();
      //});
    }
  }
}
