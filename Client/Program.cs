using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast;
using Blazored.LocalStorage;
using Client.Interfaces;
using Client.Authentication;
using Client.Services;
using Client.Helpers;
using Microsoft.AspNetCore.Components;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredToast();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IRegistrationService, RegistrationService>();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<SpinnerService>();
            builder.Services.AddScoped<SpinnerHandler>();
            builder.Services.AddScoped(s =>
            {
                SpinnerHandler spinHandler = s.GetRequiredService<SpinnerHandler>();
                spinHandler.InnerHandler = new HttpClientHandler();
                NavigationManager navManager = s.GetRequiredService<NavigationManager>();
                return new HttpClient(spinHandler)
                {
                    BaseAddress = new Uri(navManager.BaseUri)
                };
            });

            await builder.Build().RunAsync();
        }
    }
}
