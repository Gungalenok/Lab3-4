using System.Text;
using System.Xml.Serialization;
using Lab3;

internal class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string baseDirectory = AppContext.BaseDirectory;
        string inputPath = Path.Combine(baseDirectory, "input.txt");

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Не найден файл: {inputPath}");
            return;
        }

        Text text = TextParser.Parse(File.ReadAllText(inputPath));

        // lab4
        Console.WriteLine("\n=== Concordance ===");
        foreach (var item in text.BuildConcordance()
            .OrderBy(item => item.Key, StringComparer.CurrentCulture))
        {
            Console.WriteLine(
                $"{item.Key.PadRight(35, '.')}{item.Value.Count}: " +
                string.Join(" ", item.Value.Lines));
        }
        // -------

        Console.WriteLine("\n=== ExportToXml ===");
        string xmlPath = Path.Combine(baseDirectory, "text.xml");
        var serializer = new XmlSerializer(typeof(Text));

        using (var writer = new StreamWriter(xmlPath, false, new UTF8Encoding(false)))
            serializer.Serialize(writer, text);

        Console.WriteLine($"XML сохранён: {xmlPath}");

        Console.WriteLine("\n=== SortByWordCount ===");
        foreach (Sentence sentence in text.SortByWordCount())
            Console.WriteLine(sentence);

        Console.WriteLine("\n=== SortByLength ===");
        foreach (Sentence sentence in text.SortByLength())
            Console.WriteLine(sentence);

        Console.WriteLine("\n=== FindWordsInQuestions(4) ===");
        foreach (string word in text.FindWordsInQuestions(4))
            Console.WriteLine(word);

        Console.WriteLine("\n=== RemoveWords(4) ===");
        var sentencesBeforeRemoval = text.Sentences
            .ToDictionary(sentence => sentence, sentence => sentence.ToString());

        text.RemoveWords(4);

        foreach (Sentence sentence in sentencesBeforeRemoval.Keys)
        {
            string before = sentencesBeforeRemoval[sentence];
            string after = sentence.ToString();

            if (before != after)
            {
                Console.WriteLine($"До: {before}");
                Console.WriteLine($"После: {after}");
            }
        }

        Console.WriteLine("\n=== ReplaceWords(0, 3, \"xxx\") ===");
        if (text.Sentences.Count > 0)
        {
            text.ReplaceWords(0, 3, "xxx");
            Console.WriteLine(text.Sentences[0]);
        }
        else
        {
            Console.WriteLine("В тексте нет предложений.");
        }

        Console.WriteLine("\n=== RemoveStopWords ===");
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        LoadStopWords(Path.Combine(baseDirectory, "stopwords_ru.txt"), stopWords);
        LoadStopWords(Path.Combine(baseDirectory, "stopwords_en.txt"), stopWords);

        Text example = TextParser.Parse(
            "Я и мой друг были в парке. We are happy because we are together.");

        foreach (Sentence sentence in example.Sentences)
            Console.WriteLine($"До: {sentence}");

        example.RemoveStopWords(stopWords);

        foreach (Sentence sentence in example.Sentences)
            Console.WriteLine($"После: {sentence}");
    }

    private static void LoadStopWords(string path, HashSet<string> stopWords)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Файл стоп-слов не найден: {path}");
            return;
        }

        foreach (string line in File.ReadLines(path))
        {
            string word = line.Trim();
            if (word.Length > 0)
                stopWords.Add(word);
        }
    }
}
