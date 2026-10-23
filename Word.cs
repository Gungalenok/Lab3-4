using System.Xml.Serialization;

namespace Lab3;

public class Word : Token
{
    [XmlIgnore]
    public int LineNumber { get; private set; }

    public Word() { }

    public Word(string value, int lineNumber = 1) : base(value)
    {
        LineNumber = lineNumber;
    }

    public bool StartsWithConsonant()
    {
        if (string.IsNullOrEmpty(Value) || !char.IsLetter(Value[0]))
            return false;

        const string vowels = "aeiouyаеёиоуыэюя";
        return !vowels.Contains(char.ToLowerInvariant(Value[0]));
    }
}
