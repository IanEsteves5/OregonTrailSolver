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
    public class GameChoiceMemoryTests
    {
        [TestMethod]
        public void GameChoiceMemoryUpdateTest()
        {
            var m = new GameChoiceMemory();

            Assert.AreEqual(-1, m.PreviousChoice);
            Assert.AreEqual(string.Empty, m.PreviousChoiceText);

            Assert.AreEqual(-1, m.MessageMemory.GetLastChoice(1));
            Assert.AreEqual(string.Empty, m.MessageMemory.GetLastChoiceText(1));

            Assert.AreEqual(-1, m.FirstLineMemory.GetLastChoice(2));
            Assert.AreEqual(string.Empty, m.FirstLineMemory.GetLastChoiceText(2));

            Assert.AreEqual(-1, m.LastLineMemory.GetLastChoice(3));
            Assert.AreEqual(string.Empty, m.LastLineMemory.GetLastChoiceText(3));

            m.Update(1, 2, 3, 4, "test");

            Assert.AreEqual(4, m.PreviousChoice);
            Assert.AreEqual("test", m.PreviousChoiceText);

            Assert.AreEqual(4, m.MessageMemory.GetLastChoice(1));
            Assert.AreEqual("test", m.MessageMemory.GetLastChoiceText(1));

            Assert.AreEqual(4, m.FirstLineMemory.GetLastChoice(2));
            Assert.AreEqual("test", m.FirstLineMemory.GetLastChoiceText(2));

            Assert.AreEqual(4, m.LastLineMemory.GetLastChoice(3));
            Assert.AreEqual("test", m.LastLineMemory.GetLastChoiceText(3));
        }
    }
}
