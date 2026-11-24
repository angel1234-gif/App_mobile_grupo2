using CommunityToolkit.Mvvm.Input;

namespace ReservasBarberia.ViewModels;

public partial class ShellViewModel
{
    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Cerrar sesión",
            "¿Seguro que quieres cerrar sesión?",
            "Sí",
            "No");

        if (!confirmar)
            return;

        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//LoginPage");
    }
}