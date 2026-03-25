using Midieval.Views;

namespace Midieval;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MidiControlPage), typeof(MidiControlPage));
    }
}
