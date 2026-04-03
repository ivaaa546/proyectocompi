using Avalonia.Controls;
using Avalonia.Input;
using EditorTexto.ViewModels;

namespace EditorTexto.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Capturar F5 en el nivel de la ventana, incluso cuando el TextBox tiene foco
        AddHandler(KeyDownEvent, OnWindowKeyDown, handledEventsToo: true);
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5 && DataContext is MainWindowViewModel vm)
        {
            vm.EjecutarAnalisisCommand.Execute(null);
            e.Handled = true;
        }
    }
}