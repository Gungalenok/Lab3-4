using System.Text;
using System.Xml.Serialization;

namespace Lab3;

public class Sentence
{
    [XmlElement("word", typeof(Word))]
    [XmlElement("punctuation", typeof(Punctuation))]
    public List<Token> Tokens { get; set; } = new();

    [XmlIgnore]
    public int WordCount => Tokens.Count(token => token is Word);

    [XmlIgnore]
    public int Length => ToString().Length;

    [XmlIgnore]
    public bool IsQuestion
    {
        get
        {
            Punctuation? ending = Tokens
                .OfType<Punctuation>()
                .LastOrDefault(punctuation =>
                    punctuation.Value.Any(c => c is '.' or '!' or '?' or '…'));

            return ending?.Value.Contains('?') == true;
        }
    }

    public void AddToken(Token token)
    {
        ArgumentNullException.ThrowIfNull(token);
        Tokens.Add(token);
    }

    public void RemoveWords(int length)
    {
        Tokens.RemoveAll(token =>
            token is Word word &&
            word.Value.Length == length &&
            word.StartsWithConsonant());
    }

    public void RemoveStopWords(ISet<string> stopWords)
    {
        Tokens.RemoveAll(token =>
            token is Word word && stopWords.Contains(word.Value));
    }

    public void ReplaceWords(int length, string replacement)
    {
        ArgumentNullException.ThrowIfNull(replacement);

        foreach (Word word in Tokens.OfType<Word>())
        {
            if (word.Value.Length == length)
                word.Value = replacement;
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder();

        foreach (Token token in Tokens)
        {
            if (token is Word && NeedsSpaceBeforeWord(result))
                result.Append(' ');

            result.Append(token.Value);
        }

        return result.ToString();
    }

    private static bool NeedsSpaceBeforeWord(StringBuilder result)
    {
        if (result.Length == 0)
            return false;

        char previous = result[^1];

        return !char.IsWhiteSpace(previous) &&
               previous is not '(' and not '[' and not '{' and
               not '«' and not '„' and not '“';
    }
}
