using CommunityToolkit.Maui;
using Midieval.Services;
using Midieval.ViewModels;
using Midieval.Views;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace Midieval;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        // BLE
        builder.Services.AddSingleton<IBluetoothLE>(CrossBluetoothLE.Current);
        builder.Services.AddSingleton<IAdapter>(CrossBluetoothLE.Current.Adapter);

        // Services
        builder.Services.AddSingleton<IBleService, BleService>();
        builder.Services.AddSingleton<IMidiService, MidiService>();

        // ViewModels
        builder.Services.AddTransient<DeviceScanViewModel>();
        builder.Services.AddTransient<MidiControlViewModel>();

        // Views
        builder.Services.AddTransient<DeviceScanPage>();
        builder.Services.AddTransient<MidiControlPage>();

        return builder.Build();
    }
}
