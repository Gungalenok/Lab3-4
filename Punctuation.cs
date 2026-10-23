namespace Lab3;

public class Punctuation : Token
{
    public Punctuation() { }

    public Punctuation(string value) : base(value) { }

    public Punctuation(char value) : base(value.ToString()) { }
}