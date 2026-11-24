using ReservasBarberia.ViewModels;

namespace ReservasBarberia;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = new ShellViewModel();
    }
}
