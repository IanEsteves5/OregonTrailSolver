using Microsoft.VisualStudio.TestTools.UnitTesting;
using OregonTrail.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OregonTrail.Test.Solver
{
    [TestClass]
    public class MessageIndexerTests
    {
        [TestMethod]
        public void GetMessageIndexTest()
        {
            var indexer = new MessageIndexer();

            Assert.AreEqual(-1, indexer.GetMessageIndex(string.Empty));
            Assert.AreEqual(0, indexer.GetMessageIndex("abc"));
            Assert.AreEqual(1, indexer.GetMessageIndex("def"));
            Assert.AreEqual(2, indexer.GetMessageIndex("ghi"));
            Assert.AreEqual(0, indexer.GetMessageIndex("abc"));
            Assert.AreEqual(1, indexer.GetMessageIndex("DEF"));
            Assert.AreEqual(2, indexer.GetMessageIndex("  ghi "));
        }
    }
}
