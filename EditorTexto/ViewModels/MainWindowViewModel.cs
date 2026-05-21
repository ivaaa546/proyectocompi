using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EditorTexto.Models;

namespace EditorTexto.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly GrammarAnalyzer _analyzer = new();

    [ObservableProperty]
    private string inputText = string.Empty;

    [ObservableProperty]
    private string outputText = string.Empty;

    [ObservableProperty]
    private bool showResults;

    public ObservableCollection<VectorRecord> InputVectors { get; } = new();
    public ObservableCollection<ProductionRecord> InputProductions { get; } = new();

    public ObservableCollection<VectorRecord> OutputVectors { get; } = new();
    public ObservableCollection<ProductionRecord> OutputProductions { get; } = new();

    public ObservableCollection<FirstFollowRecord> FirstSets { get; } = new();
    public ObservableCollection<FirstFollowRecord> FollowSets { get; } = new();

    [RelayCommand]
    private void EjecutarAnalisis()
    {
        // 1. Parse Input
        var inputCtx = _analyzer.ParseInput(InputText);

        // Populate Input UI
        InputVectors.Clear();
        var inputV = inputCtx.NonTerminals;
        var inputT = inputCtx.Terminals;
        int maxInputLen = System.Math.Max(inputV.Count, inputT.Count);
        for (int i = 0; i < maxInputLen; i++)
        {
            string vStr = i < inputV.Count ? inputV[i] : "";
            string tStr = i < inputT.Count ? inputT[i] : "";
            InputVectors.Add(new VectorRecord(vStr, tStr));
        }

        InputProductions.Clear();
        foreach (var nt in inputCtx.NonTerminals)
        {
            if (inputCtx.Productions.TryGetValue(nt, out var prods))
            {
                foreach (var prod in prods)
                {
                    string pStr = prod.Count == 0 ? "e" : string.Join(" ", prod);
                    InputProductions.Add(new ProductionRecord(nt, pStr));
                }
            }
        }

        // 2. Eliminate Left Recursion
        var outputCtx = _analyzer.EliminateLeftRecursion(inputCtx);

        // Construct Output Text
        var sb = new StringBuilder();
        foreach (var nt in outputCtx.NonTerminals)
        {
            if (outputCtx.Productions.TryGetValue(nt, out var prods))
            {
                var prodStrings = prods.Select(p => p.Count == 0 ? "e" : string.Join(" ", p));
                sb.AppendLine($"{nt}:: {string.Join(" | ", prodStrings)}");
            }
        }
        OutputText = sb.ToString().TrimEnd();

        // Populate Output UI
        OutputVectors.Clear();
        var outV = outputCtx.NonTerminals;
        var outT = outputCtx.Terminals;
        int maxOutLen = System.Math.Max(outV.Count, outT.Count);
        for (int i = 0; i < maxOutLen; i++)
        {
            string vStr = i < outV.Count ? outV[i] : "";
            string tStr = i < outT.Count ? outT[i] : "";
            OutputVectors.Add(new VectorRecord(vStr, tStr));
        }

        OutputProductions.Clear();
        foreach (var nt in outputCtx.NonTerminals)
        {
            if (outputCtx.Productions.TryGetValue(nt, out var prods))
            {
                foreach (var prod in prods)
                {
                    string pStr = prod.Count == 0 ? "e" : string.Join(" ", prod);
                    OutputProductions.Add(new ProductionRecord(nt, pStr));
                }
            }
        }

        // 3. Compute First and Follow
        var first = _analyzer.ComputeFirst(outputCtx);
        var follow = _analyzer.ComputeFollow(outputCtx, first);

        FirstSets.Clear();
        foreach (var nt in outputCtx.NonTerminals)
        {
            var terms = first.ContainsKey(nt) ? string.Join(", ", first[nt]) : "";
            FirstSets.Add(new FirstFollowRecord(nt, terms));
        }

        FollowSets.Clear();
        foreach (var nt in outputCtx.NonTerminals)
        {
            var terms = follow.ContainsKey(nt) ? string.Join(", ", follow[nt]) : "";
            FollowSets.Add(new FirstFollowRecord(nt, terms));
        }

        ShowResults = true;
    }
}
