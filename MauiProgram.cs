using Microsoft.Extensions.Logging;
using ReservasBarberia.Services;
using ReservasBarberia.Views;
using ReservasBarberia.ViewModels;

namespace ReservasBarberia;

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
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        
        builder.Services.AddSingleton<DataService>();

        
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<PrincipalPage>();
        builder.Services.AddTransient<ListadoReservasPage>();
        builder.Services.AddTransient<FormularioReservaPage>();
        builder.Services.AddTransient<BarberosPage>();
        builder.Services.AddTransient<ServiciosPage>();
        builder.Services.AddTransient<ReservaViewModel>();
        builder.Services.AddTransient<BarberoViewModel>();
        builder.Services.AddTransient<ServicioViewModel>();
        builder.Services.AddTransient<ClientesPage>();
        return builder.Build();
	}
}



