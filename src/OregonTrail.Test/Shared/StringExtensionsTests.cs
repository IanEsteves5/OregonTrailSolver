using Microsoft.VisualStudio.TestTools.UnitTesting;
using OregonTrail.Shared;

namespace OregonTrail.Test.Shared
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void RemoveNumbersTest()
        {
            VerifyRemoveNumbers(string.Empty, string.Empty);
            VerifyRemoveNumbers("abc", "abc");
            VerifyRemoveNumbers("1abc", "{0}abc", "1");
            VerifyRemoveNumbers("a5bc", "a{0}bc", "5");
            VerifyRemoveNumbers("abc9", "abc{0}", "9");
            VerifyRemoveNumbers("12a34b56c789", "{0}a{1}b{2}c{3}", "12", "34", "56", "789");
        }

        private void VerifyRemoveNumbers(string s, string expected, params string[] expectedNumbers)
        {
            string[] actualNumbers;
            var actual = s.RemoveNumbers(out actualNumbers);

            Assert.AreEqual(expected, actual, s);
            CollectionAssert.AreEqual(expectedNumbers, actualNumbers, s);
            Assert.AreEqual(s, string.Format(actual, actualNumbers));
        }
    }
}
