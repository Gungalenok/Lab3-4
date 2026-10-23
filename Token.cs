using System.Xml.Serialization;

namespace Lab3;

[XmlInclude(typeof(Word))]
[XmlInclude(typeof(Punctuation))]
public abstract class Token
{
    [XmlText]
    public string Value { get; set; } = "";

    protected Token() { }

    protected Token(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;
}
