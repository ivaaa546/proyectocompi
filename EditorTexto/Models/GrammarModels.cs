using System.Collections.Generic;

namespace EditorTexto.Models;

public record ProductionRecord(string V, string Producciones);
public record VectorRecord(string V, string T);
public record FirstFollowRecord(string V, string Terminales);

public class GrammarContext
{
    public string InitialSymbol { get; set; } = string.Empty;
    public List<string> NonTerminals { get; set; } = new();
    public List<string> Terminals { get; set; } = new();
    public Dictionary<string, List<List<string>>> Productions { get; set; } = new();

    public GrammarContext() { }

    public GrammarContext Clone()
    {
        var clone = new GrammarContext
        {
            InitialSymbol = this.InitialSymbol,
            NonTerminals = new List<string>(this.NonTerminals),
            Terminals = new List<string>(this.Terminals)
        };
        foreach (var kvp in this.Productions)
        {
            var list = new List<List<string>>();
            foreach (var prod in kvp.Value)
            {
                list.Add(new List<string>(prod));
            }
            clone.Productions[kvp.Key] = list;
        }
        return clone;
    }
}
