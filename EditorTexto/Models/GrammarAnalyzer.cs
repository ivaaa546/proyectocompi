using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorTexto.Models;

public class GrammarAnalyzer
{
    public GrammarContext ParseInput(string input)
    {
        var ctx = new GrammarContext();
        if (string.IsNullOrWhiteSpace(input)) return ctx;

        var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parts = line.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) continue;

            var nt = parts[0].Trim();
            if (string.IsNullOrEmpty(ctx.InitialSymbol))
            {
                ctx.InitialSymbol = nt;
            }
            if (!ctx.NonTerminals.Contains(nt))
            {
                ctx.NonTerminals.Add(nt);
            }

            if (!ctx.Productions.ContainsKey(nt))
            {
                ctx.Productions[nt] = new List<List<string>>();
            }

            var prods = parts[1].Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in prods)
            {
                var tokens = TokenizeProduction(p.Trim());
                ctx.Productions[nt].Add(tokens);

                // Add to terminals if it's not a non-terminal
                foreach (var token in tokens)
                {
                    if (IsTerminal(token) && !ctx.Terminals.Contains(token))
                    {
                        ctx.Terminals.Add(token);
                    }
                }
            }
        }

        return ctx;
    }

    private List<string> TokenizeProduction(string p)
    {
        var tokens = new List<string>();
        int i = 0;
        
        bool isQuote(char c) => c == '\'' || c == '‘' || c == '’' || c == '´' || c == '`';

        while (i < p.Length)
        {
            if (char.IsWhiteSpace(p[i]))
            {
                i++;
                continue;
            }

            if (isQuote(p[i]))
            {
                i++;
                var start = i;
                while (i < p.Length && !isQuote(p[i])) i++;
                var token = p.Substring(start, i - start);
                tokens.Add(token);
                if (i < p.Length) i++; // skip closing quote
            }
            else if (char.IsUpper(p[i]))
            {
                var nt = p[i].ToString();
                i++;
                if (i < p.Length && p[i] == '!')
                {
                    nt += "!"; // Normalize to !
                    i++;
                }
                tokens.Add(nt);
            }
            else
            {
                // Unquoted terminal like 'e' or 'a1' (if they didn't use quotes)
                var start = i;
                while (i < p.Length && !char.IsWhiteSpace(p[i]) && !isQuote(p[i]) && !char.IsUpper(p[i]) && p[i] != '|')
                {
                    i++;
                }
                var token = p.Substring(start, i - start);
                if (!string.IsNullOrEmpty(token))
                {
                    tokens.Add(token);
                }
            }
        }

        // If a token is "e" and stands alone, it's epsilon.
        return tokens;
    }

    private bool IsTerminal(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        // Non-terminals start with uppercase letter
        if (char.IsUpper(token[0])) return false;
        return true;
    }

    public GrammarContext EliminateLeftRecursion(GrammarContext input)
    {
        var output = new GrammarContext
        {
            InitialSymbol = input.InitialSymbol,
            Terminals = new List<string>()
        };
        
        // El profesor pone 'e' de primero en el vector T de salida en su maqueta
        output.Terminals.Add("e");
        foreach (var t in input.Terminals)
        {
            if (t != "e" && !output.Terminals.Contains(t))
            {
                output.Terminals.Add(t);
            }
        }

        foreach (var A in input.NonTerminals)
        {
            output.NonTerminals.Add(A);
            if (!input.Productions.ContainsKey(A)) continue;

            var prods = input.Productions[A];
            var alpha = new List<List<string>>();
            var beta = new List<List<string>>();

            foreach (var prod in prods)
            {
                if (prod.Count > 0 && prod[0] == A)
                {
                    alpha.Add(prod.Skip(1).ToList());
                }
                else
                {
                    beta.Add(new List<string>(prod));
                }
            }

            if (alpha.Count > 0)
            {
                var A_prime = A + "!";
                output.NonTerminals.Add(A_prime); // Añadir A! justo después de A

                output.Productions[A] = new List<List<string>>();
                if (beta.Count == 0)
                {
                    output.Productions[A].Add(new List<string> { A_prime });
                }
                else
                {
                    foreach (var b in beta)
                    {
                        var newProd = new List<string>(b);
                        if (newProd.Count == 1 && newProd[0] == "e")
                        {
                            newProd.Clear();
                        }
                        newProd.Add(A_prime);
                        output.Productions[A].Add(newProd);
                    }
                }

                output.Productions[A_prime] = new List<List<string>>();
                foreach (var a in alpha)
                {
                    var newProd = new List<string>(a);
                    newProd.Add(A_prime);
                    output.Productions[A_prime].Add(newProd);
                }
                output.Productions[A_prime].Add(new List<string> { "e" });
            }
            else
            {
                output.Productions[A] = new List<List<string>>();
                foreach (var p in prods)
                {
                    output.Productions[A].Add(new List<string>(p));
                }
            }
        }

        return output;
    }

    public Dictionary<string, HashSet<string>> ComputeFirst(GrammarContext ctx)
    {
        var first = new Dictionary<string, HashSet<string>>();
        foreach (var t in ctx.Terminals) first[t] = new HashSet<string> { t };
        foreach (var nt in ctx.NonTerminals) first[nt] = new HashSet<string>();

        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var nt in ctx.NonTerminals)
            {
                if (!ctx.Productions.ContainsKey(nt)) continue;

                foreach (var prod in ctx.Productions[nt])
                {
                    if (prod.Count == 0 || (prod.Count == 1 && prod[0] == "e"))
                    {
                        if (first[nt].Add("e")) changed = true;
                        continue;
                    }

                    bool allEpsilon = true;
                    foreach (var symbol in prod)
                    {
                        var symbolFirst = first.ContainsKey(symbol) ? first[symbol] : new HashSet<string>();
                        foreach (var f in symbolFirst)
                        {
                            if (f != "e")
                            {
                                if (first[nt].Add(f)) changed = true;
                            }
                        }
                        if (!symbolFirst.Contains("e"))
                        {
                            allEpsilon = false;
                            break;
                        }
                    }

                    if (allEpsilon)
                    {
                        if (first[nt].Add("e")) changed = true;
                    }
                }
            }
        }
        return first;
    }

    public Dictionary<string, HashSet<string>> ComputeFollow(GrammarContext ctx, Dictionary<string, HashSet<string>> first)
    {
        var follow = new Dictionary<string, HashSet<string>>();
        foreach (var nt in ctx.NonTerminals) follow[nt] = new HashSet<string>();

        if (!string.IsNullOrEmpty(ctx.InitialSymbol))
        {
            follow[ctx.InitialSymbol].Add("$");
        }

        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var nt in ctx.NonTerminals)
            {
                if (!ctx.Productions.ContainsKey(nt)) continue;

                foreach (var prod in ctx.Productions[nt])
                {
                    for (int i = 0; i < prod.Count; i++)
                    {
                        var symbol = prod[i];
                        if (!ctx.NonTerminals.Contains(symbol)) continue;

                        bool nextHasEpsilon = true;
                        for (int j = i + 1; j < prod.Count; j++)
                        {
                            var nextSymbol = prod[j];
                            var nextFirst = first.ContainsKey(nextSymbol) ? first[nextSymbol] : new HashSet<string>();

                            foreach (var f in nextFirst)
                            {
                                if (f != "e")
                                {
                                    if (follow[symbol].Add(f)) changed = true;
                                }
                            }

                            if (!nextFirst.Contains("e"))
                            {
                                nextHasEpsilon = false;
                                break;
                            }
                        }

                        if (nextHasEpsilon)
                        {
                            foreach (var f in follow[nt])
                            {
                                if (follow[symbol].Add(f)) changed = true;
                            }
                        }
                    }
                }
            }
        }
        return follow;
    }
}
