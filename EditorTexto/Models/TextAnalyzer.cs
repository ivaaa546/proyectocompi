using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EditorTexto.Models;

public sealed class TextAnalyzer
{
    private static readonly Regex ValidWordRegex = new("^[A-Za-z]+$", RegexOptions.Compiled);
    private static readonly char[] TokenTrimChars =
    [
        '.', ',', ';', ':', '!', '?', '"', '\'', '(', ')', '[', ']', '{', '}',
        '¡', '¿', '-', '_', '/', '\\'
    ];

    public AnalysisResult AnalyzeText(string text)
    {
        var safeText = text ?? string.Empty;

        var wordsCount = 0;
        var tokens = safeText.Split((char[]?)null, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            var cleanedToken = token.Trim(TokenTrimChars);
            if (cleanedToken.Length == 0)
            {
                continue;
            }

            if (ValidWordRegex.IsMatch(cleanedToken))
            {
                wordsCount++;
            }
        }

        var vowelCounts = new Dictionary<char, int>
        {
            ['a'] = 0,
            ['e'] = 0,
            ['i'] = 0,
            ['o'] = 0,
            ['u'] = 0,
        };

        foreach (var character in safeText.ToLowerInvariant())
        {
            if (vowelCounts.ContainsKey(character))
            {
                vowelCounts[character]++;
            }
        }

        var resultVowels = new List<VocalInfo>();
        foreach (var vowel in new[] { 'a', 'e', 'i', 'o', 'u' })
        {
            var count = vowelCounts[vowel];
            if (count > 0)
            {
                resultVowels.Add(new VocalInfo(vowel, count));
            }
        }

        return new AnalysisResult(
            wordsCount,
            resultVowels.Count,
            resultVowels);
    }
}

public sealed record VocalInfo(char Vocal, int Cantidad);

public sealed record AnalysisResult(int TotalPalabras, int TotalVocalesUnicas, List<VocalInfo> Vocales);
