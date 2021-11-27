using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OregonTrail.Shared
{
    public static class StringExtensions
    {
        private static readonly char[] Digits = "0123456789".ToCharArray();
        private static readonly char[] Separators = " \t\r\n".ToCharArray();

        public static string RemoveNumbers(this string s, out string[] numbers)
        {
            if (string.IsNullOrEmpty(s))
            {
                numbers = new string[] { };
                return string.Empty;
            }

            var sb = new StringBuilder();
            var numbersList = new List<string>();

            var nextIndex = 0;
            var lastPos = 0;
            var posStartNumber = 0;
            var posEndNumber = 0;

            do
            {
                lastPos = posEndNumber;

                posStartNumber = s.IndexOfAny(Digits, lastPos);
                if (posStartNumber < 0)
                {
                    sb.Append(s.Substring(lastPos));
                    break;
                }
                else if (posStartNumber > lastPos)
                    sb.Append(s.Substring(lastPos, posStartNumber - lastPos));

                sb.Append("{" + (nextIndex++) + "}");

                posEndNumber = posStartNumber;
                while (posEndNumber < s.Length && char.IsNumber(s[posEndNumber]))
                    posEndNumber++;

                if (posEndNumber >= s.Length)
                {
                    numbersList.Add(s.Substring(posStartNumber));
                    break;
                }
                else
                    numbersList.Add(s.Substring(posStartNumber, posEndNumber - posStartNumber));
            }
            while (posStartNumber >= 0 && posEndNumber < s.Length);

            numbers = numbersList.ToArray();
            return sb.ToString();
        }

        public static IEnumerable<string> GetUniqueWords(this string s)
        {
            return s
                .Split(Separators)
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(w => w.Trim())
                .Distinct();
        }
    }
}
