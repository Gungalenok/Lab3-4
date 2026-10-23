using System.Text.RegularExpressions;

namespace Lab3;

public static class TextParser
{
    private static readonly Regex TokenPattern = new(
        @"[\p{L}\p{M}\p{N}_]+(?:[-'’][\p{L}\p{M}\p{N}_]+)*|[.!?…]+|[^\s]",
        RegexOptions.Compiled);

    private static readonly HashSet<string> ClosingMarks =
        new(StringComparer.Ordinal)
        {
            "\"", "'", "’", "”", "»", ")", "]", "}"
        };

    public static Text Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var text = new Text();
        var sentence = new Sentence();
        MatchCollection matches = TokenPattern.Matches(source);

        // lab4
        int lineNumber = 1;
        int position = 0;

        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];

            for (int j = position; j < match.Index; j++)
            {
                if (source[j] == '\n')
                    lineNumber++;
            }

            position = match.Index + match.Length;
            string value = match.Value;
        // ------

            if (IsWord(value))
            {
                sentence.AddToken(new Word(value, lineNumber));
                continue;
            }

            sentence.AddToken(new Punctuation(value));

            if (!IsSentenceEnding(value))
                continue;

            while (i + 1 < matches.Count && ClosingMarks.Contains(matches[i + 1].Value))
            {
                Match closingMark = matches[++i];
                sentence.AddToken(new Punctuation(closingMark.Value));
                position = closingMark.Index + closingMark.Length;
            }

            text.AddSentence(sentence);
            sentence = new Sentence();
        }

        if (sentence.Tokens.Count > 0)
            text.AddSentence(sentence);

        return text;
    }

    private static bool IsWord(string value)
    {
        return value.Length > 0 &&
               (char.IsLetterOrDigit(value[0]) || value[0] == '_');
    }

    private static bool IsSentenceEnding(string value)
    {
        return value.Any(character => character is '.' or '!' or '?' or '…');
    }
}
