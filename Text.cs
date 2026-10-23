using System.Xml.Serialization;

namespace Lab3;

[XmlRoot("text")]
public class Text
{
    [XmlElement("sentence")]
    public List<Sentence> Sentences { get; set; } = new();

    public void AddSentence(Sentence sentence)
    {
        ArgumentNullException.ThrowIfNull(sentence);
        Sentences.Add(sentence);
    }

    public List<Sentence> SortByWordCount()
    {
        return Sentences
            .OrderBy(sentence => sentence.WordCount)
            .ToList();
    }

    public List<Sentence> SortByLength()
    {
        return Sentences
            .OrderBy(sentence => sentence.Length)
            .ToList();
    }

    public List<string> FindWordsInQuestions(int length)
    {
        return Sentences
            .Where(sentence => sentence.IsQuestion)
            .SelectMany(sentence => sentence.Tokens.OfType<Word>())
            .Where(word => word.Value.Length == length)
            .Select(word => word.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    // lab4
    public Dictionary<string, (int Count, SortedSet<int> Lines)> BuildConcordance()
    {
        Dictionary<string, (int Count, SortedSet<int> Lines)> concordance = new();

        foreach (Word word in Sentences.SelectMany(sentence => sentence.Tokens.OfType<Word>()))
        {
            string value = word.Value.ToLowerInvariant();

            if (!concordance.TryGetValue(value, out var entry))
                entry = (0, new SortedSet<int>());

            entry.Count++;
            entry.Lines.Add(word.LineNumber);

            concordance[value] = entry;
        }

        return concordance;
    }
    // -------

    public void RemoveWords(int length)
    {
        foreach (Sentence sentence in Sentences)
            sentence.RemoveWords(length);
    }

    public void RemoveStopWords(IEnumerable<string> words)
    {
        var stopWords = new HashSet<string>(
            words,
            StringComparer.OrdinalIgnoreCase);

        foreach (Sentence sentence in Sentences)
            sentence.RemoveStopWords(stopWords);
    }

    public void ReplaceWords(int sentenceIndex, int length, string replacement)
    {
        if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sentenceIndex),
                "Предложения с таким индексом нет.");
        }

        Sentences[sentenceIndex].ReplaceWords(length, replacement);
    }
}
