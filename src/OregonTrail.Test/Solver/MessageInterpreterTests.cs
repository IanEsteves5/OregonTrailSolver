using Microsoft.VisualStudio.TestTools.UnitTesting;
using OregonTrail.Solver;

namespace OregonTrail.Test.Solver
{
    [TestClass]
    public class MessageInterpreterTests
    {
        [TestMethod]
        public void InterpretTest()
        {
            var interpreter = new MessageInterpreter();

            var result = interpreter.Interpret(string.Empty);
            ValidateTextInformation(result.Message, string.Empty, new string[] { });
            ValidateTextInformation(result.FirstLine, string.Empty, new string[] { });
            ValidateTextInformation(result.LastLine, string.Empty, new string[] { });

            result = interpreter.Interpret("abc");
            ValidateTextInformation(result.Message, "abc", new string[] { });
            ValidateTextInformation(result.FirstLine, "abc", new string[] { });
            ValidateTextInformation(result.LastLine, "abc", new string[] { });

            result = interpreter.Interpret("abc\r\ndef");
            ValidateTextInformation(result.Message, "abc def", new string[] { });
            ValidateTextInformation(result.FirstLine, "abc", new string[] { });
            ValidateTextInformation(result.LastLine, "def", new string[] { });

            result = interpreter.Interpret("\r\n    \r\n\r\n abc  \r\n \t def\t\r\n\t  \tghi \r\n   \r\n");
            ValidateTextInformation(result.Message, "abc def ghi", new string[] { });
            ValidateTextInformation(result.FirstLine, "abc", new string[] { });
            ValidateTextInformation(result.LastLine, "ghi", new string[] { });

            result = interpreter.Interpret("1ab2c\r\n3d4ef\r\ng5h6i7");
            ValidateTextInformation(result.Message, "{0}ab{1}c {2}d{3}ef g{4}h{5}i{6}", new string[] { "1", "2", "3", "4", "5", "6", "7" });
            ValidateTextInformation(result.FirstLine, "{0}ab{1}c", new string[] { "1", "2" });
            ValidateTextInformation(result.LastLine, "g{0}h{1}i{2}", new string[] { "5", "6", "7" });
        }

        private void ValidateTextInformation(TextInfo info, string value, string[] numbers)
        {
            Assert.AreEqual(value, info.Value);
            CollectionAssert.AreEqual(numbers, info.Numbers);
        }
    }
}
