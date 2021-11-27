using Microsoft.VisualStudio.TestTools.UnitTesting;
using OregonTrail.Shared;
using OregonTrail.Solver;

namespace OregonTrail.Test.Solver
{
    [TestClass]
    public class DecisionEngineTests
    {
        [TestMethod]
        public void DecideTest()
        {
            var l1 = new DecisionLayer(
                new Matrix(2, 3, 1.2, 2.3, 3.4, 4.5, 5.6, 6.7),
                new Matrix(2, 1, 10.0, 20.0));

            var l2 = new DecisionLayer(
                new Matrix(4, 2, 10.11, 11.12, 12.13, 13.14, 14.15, 15.16, 16.17, 17.18, 18.19),
                new Matrix(4, 1, 30.0, 40.0, 50.0, 60.0));

            var d = new DecisionEngine(l1, l2);

            var r = d.Decide(new double[] { 100.0, 200.0, 300.0 });

            Assert.AreEqual(3, l1.InputSize);
            Assert.AreEqual(2, l1.OutputSize);

            Assert.AreEqual(2, l2.InputSize);
            Assert.AreEqual(4, l2.OutputSize);

            Assert.AreEqual(3, d.InputSize);
            Assert.AreEqual(4, d.OutputSize);

            Assert.AreEqual(4, r.Length);

            foreach (var x in r)
                Assert.IsTrue(x >= 0 && x <= 1);
        }
    }
}
