using System.Xml.Linq;
using System.Xml.Serialization;
using Xunit;
using Lab3;

namespace Lab3.Tests;

public class Lab3Tests
{
    private static Text CreateSample()
    {
        return TextParser.Parse(
            "Кто-то don't code_test isn’t. Слово слово?\n" +
            "Слово. Код CODE?\n" +
            "Другое слово. код снова?");
    }

    [Fact]
    public void Parse_RecognizesHyphensApostrophesAndUnderscores()
    {
        Text text = CreateSample();

        string[] words = text.Sentences[0].Tokens
            .OfType<Word>()
            .Select(word => word.Value)
            .ToArray();

        Assert.Equal(new[] { "Кто-то", "don't", "code_test", "isn’t" }, words);
    }

    [Fact]
    public void Parse_CreatesSentencesAndTracksLineNumbers()
    {
        Text text = CreateSample();

        Assert.Equal(6, text.Sentences.Count);
        Assert.True(text.Sentences[1].IsQuestion);
        Assert.Equal(2, text.Sentences[2].Tokens.OfType<Word>().First().LineNumber);
        Assert.Equal(3, text.Sentences[4].Tokens.OfType<Word>().First().LineNumber);
    }

    [Fact]
    public void SortByWordCount_ReturnsAscendingOrder()
    {
        Text text = CreateSample();

        Assert.Equal(
            new[] { 1, 2, 2, 2, 2, 4 },
            text.SortByWordCount().Select(sentence => sentence.WordCount));
    }

    [Fact]
    public void SortByLength_ReturnsAscendingOrder()
    {
        Text text = CreateSample();

        Assert.Equal(
            new[] { 6, 9, 10, 12, 13, 29 },
            text.SortByLength().Select(sentence => sentence.Length));
    }

    [Fact]
    public void FindWordsInQuestions_ReturnsUniqueWordsIgnoringCase()
    {
        Text text = CreateSample();

        Assert.Equal(new[] { "Код" }, text.FindWordsInQuestions(3));
    }

    // lab4
    [Fact]
    public void BuildConcordance_CountsOccurrencesAndKeepsUniqueSortedLineNumbers()
    {
        Text text = TextParser.Parse("Piper piper picked.\nPIPER pickled piper.");

        Dictionary<string, (int Count, SortedSet<int> Lines)> concordance =
            text.BuildConcordance();

        Assert.Equal(new[] { "picked", "pickled", "piper" },
            concordance.Keys.OrderBy(word => word, StringComparer.CurrentCulture));

        var piper = concordance["piper"];
        Assert.Equal(4, piper.Count);
        Assert.Equal(new[] { 1, 2 }, piper.Lines);

        var picked = concordance["picked"];
        Assert.Equal(1, picked.Count);
        Assert.Equal(new[] { 1 }, picked.Lines);
    }

    // lab4
    [Fact]
    public void BuildConcordance_ReturnsEmptySetForTextWithoutWords()
    {
        Text text = TextParser.Parse("... !!!");

        Assert.Empty(text.BuildConcordance());
    }

    [Fact]
    public void RemoveWords_RemovesMatchingConsonantWords()
    {
        Text text = TextParser.Parse("Бобр ария тест. Дом работает.");

        text.RemoveWords(4);

        Assert.Equal("ария.", text.Sentences[0].ToString());
        Assert.Equal("Дом работает.", text.Sentences[1].ToString());
    }

    [Fact]
    public void ReplaceWords_ChangesOnlySelectedSentence()
    {
        Text text = TextParser.Parse("Дом кот. Мир жив.");

        text.ReplaceWords(0, 3, "xxx");

        Assert.Equal("xxx xxx.", text.Sentences[0].ToString());
        Assert.Equal("Мир жив.", text.Sentences[1].ToString());
    }

    [Fact]
    public void RemoveStopWords_IsCaseInsensitive()
    {
        Text text = TextParser.Parse("Я и код.");

        text.RemoveStopWords(new[] { "я", "и" });

        Assert.Equal("код.", text.Sentences[0].ToString());
    }

    [Fact]
    public void SerializeToXml_WritesWordsAndPunctuation()
    {
        Text text = TextParser.Parse("Кто-то don't code_test isn’t.");
        using var writer = new StringWriter();
        new XmlSerializer(typeof(Text)).Serialize(writer, text);
        XDocument xml = XDocument.Parse(writer.ToString());

        Assert.Equal("text", xml.Root!.Name.LocalName);

        string[] tokens = xml.Root.Element("sentence")!
            .Elements()
            .Select(element => $"{element.Name.LocalName}:{element.Value}")
            .ToArray();

        Assert.Equal(
            new[]
            {
                "word:Кто-то", "word:don't", "word:code_test",
                "word:isn’t", "punctuation:."
            },
            tokens);

        Assert.DoesNotContain("LineNumber", writer.ToString());
    }
}
