using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EditorTexto.Models;

namespace EditorTexto.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly TextAnalyzer _textAnalyzer = new();

    [ObservableProperty]
    private string textoUsuario = string.Empty;

    [ObservableProperty]
    private int totalPalabras;

    [ObservableProperty]
    private int totalVocalesUnicas;

    [ObservableProperty]
    private bool mostrarResultados;

    public ObservableCollection<VocalInfo> Vocales { get; } = new();

    [RelayCommand]
    private void EjecutarAnalisis()
    {
        var analysis = _textAnalyzer.AnalyzeText(TextoUsuario);

        TotalPalabras = analysis.TotalPalabras;
        TotalVocalesUnicas = analysis.TotalVocalesUnicas;

        Vocales.Clear();
        foreach (var vowel in analysis.Vocales)
        {
            Vocales.Add(vowel);
        }

        MostrarResultados = true;
    }
}
