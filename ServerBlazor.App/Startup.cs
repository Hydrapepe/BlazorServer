using Blazor.Fluxor;
using Blazor.Fluxor.ReduxDevTools;
using Blazor.Fluxor.Routing;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerBlazor.Authorization;
using ServerBlazor.Data;

namespace ServerBlazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("DataSource=app.db"));
            
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy => 
                    policy.AddRequirements(new EmailRequirement("pepega.com")));
            });

            services.AddSingleton<IAuthorizationHandler, EmailAuthHandler>();
            
            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

            services.AddBlazoredLocalStorage();
            services.AddBlazoredSessionStorage();

            services.AddFluxor(options =>
            {
                options.UseDependencyInjection(typeof(Startup).Assembly);
                options.AddMiddleware<ReduxDevToolsMiddleware>();
                options.AddMiddleware<RoutingMiddleware>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
