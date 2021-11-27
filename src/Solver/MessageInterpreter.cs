using OregonTrail.Shared;
using System.Linq;

namespace OregonTrail.Solver
{
    public class MessageInterpreter
    {
        private static readonly char[] LineEnd = "\r\n".ToCharArray();
        private static readonly char[] IgnoredChars = "\t\",()/\\:-".ToCharArray();
        
        public InterpretedMessage Interpret(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
                return new InterpretedMessage();

            var lines = msg
                .Split(LineEnd)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(RemoveIgnoredChars)
                .Select(s => s.Replace("  ", " ").Trim())
                .ToArray();

            var msgInfo = GetInfo(string.Join(" ", lines));
            var firstLineInfo = GetInfo(lines.FirstOrDefault() ?? string.Empty);
            var lastLineInfo = GetInfo(lines.LastOrDefault() ?? string.Empty);

            return new InterpretedMessage(msgInfo, firstLineInfo, lastLineInfo);
        }

        private string RemoveIgnoredChars(string s)
        {
            foreach (var c in IgnoredChars)
                s = s.Replace(c, ' ');
            return s;
        }
        
        private TextInfo GetInfo(string text)
        {
            string[] numbers;
            var clean = text.RemoveNumbers(out numbers);
            var words = clean.GetUniqueWords().OrderBy(s => s).ToArray();

            return new TextInfo(clean, numbers, words);
        }
    }

    public class InterpretedMessage
    {
        public TextInfo Message { get; }
        public TextInfo FirstLine { get; }
        public TextInfo LastLine { get; }
        
        public InterpretedMessage()
            : this(new TextInfo(), new TextInfo(), new TextInfo())
        {
        }

        public InterpretedMessage(TextInfo message, TextInfo firstLine, TextInfo lastLine)
        {
            Message = message;
            FirstLine = firstLine;
            LastLine = lastLine;
        }

        public override string ToString()
        {
            return Message.ToString();
        }
    }

    public class TextInfo
    {
        public string Value { get; }
        public string[] Numbers { get; }
        public string[] Words { get; }

        public TextInfo()
            : this(string.Empty, new string[] { }, new string[] { })
        {
        }

        public TextInfo(string value, string[] numbers, string[] words)
        {
            Value = value;
            Numbers = numbers;
            Words = words;
        }

        public override string ToString()
        {
            return string.Format(Value, Numbers);
        }
    }
}
