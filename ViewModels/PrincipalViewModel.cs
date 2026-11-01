using CommunityToolkit.Mvvm.Input;

namespace ReservasBarberia.ViewModels;

public partial class PrincipalViewModel
{
    [RelayCommand]
    private async Task IrAReservasAsync()
    {
        await Shell.Current.GoToAsync("//ListadoReservasPage");
    }

    [RelayCommand]
    private async Task IrANuevaReservaAsync()
    {
        await Shell.Current.GoToAsync("//FormularioReservaPage");
    }
}