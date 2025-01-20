using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using PTFGym.Authentication;
using PTFGym.Models;
using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fontello.ttf", "Icons");
                    fonts.AddFont("Montserrat-Bold.ttf", "MontserratBold");
                    fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
                    fonts.AddFont("Montserrat-Medium.ttf", "MontserratMedium");
                    fonts.AddFont("Montserrat-Light.ttf", "MontserratLight");
                    fonts.AddFont("Rubik-Light.ttf", "RubikLight");
                    fonts.AddFont("Rubik-Bold.ttf", "RubikBold");
                    fonts.AddFont("Rubik-Regular.ttf", "RubikRegular");
                });


            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<AuthorizationService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddAuthorizationCore(); 

            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ClansRoleViewModel>();
            builder.Services.AddTransient<TerminViewModel>();
            builder.Services.AddTransient<TerminDetailViewModel>();

            builder.Services.AddTransient<RezervacijaDisplay>();

            builder.Services.AddTransient<Views.MainPage>();
            builder.Services.AddTransient<AppShell>(); 
            builder.Services.AddSingleton<Models.UserSession>();

            builder.Services.AddTransient<Views.Admin.DashboardPage>();
            builder.Services.AddTransient<Views.ClansRolePage>();
            builder.Services.AddSingleton<Views.Register.RegisterPage>();
            builder.Services.AddTransient<Views.Napredak.NapredakListPage>();
            builder.Services.AddTransient<Views.Clanarina.ClanarinaListPage>();
            builder.Services.AddTransient<Views.Termin.TerminListPage>();
            builder.Services.AddTransient<Views.Trener.TrenerListPage>();
            builder.Services.AddTransient<Views.Clan.ClanListPage>();
            builder.Services.AddTransient<Views.Rezervacija.RezervacijaListPage>();
            builder.Services.AddTransient<Views.RestrictedPage>();
            builder.Services.AddTransient<Views.ProfilePage>();
            builder.Services.AddTransient<Views.Clanarina.ClanarinaClan>();
            builder.Services.AddTransient<Views.Napredak.NapredakClan>();
            builder.Services.AddTransient<Views.Napredak.NapredakTrener>();
            builder.Services.AddTransient<Views.Rezervacija.RezervacijaClan>();
            builder.Services.AddTransient<Views.Rezervacija.RezervacijaTrener>();
            builder.Services.AddTransient<Views.Termin.TerminClan>();
            builder.Services.AddTransient<Views.Termin.TerminTrener>();
            builder.Services.AddTransient<Views.Termin.TerminDetail>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
