using Microsoft.VisualStudio.TestTools.UnitTesting;
using OregonTrail.Shared;
using OregonTrail.Solver;
using System.Linq;

namespace OregonTrail.Test.Solver
{
    [TestClass]
    public class DecisionEngineBuilderTests
    {
        [TestMethod]
        public void DecisionEngineBuilderSizeTest()
        {
            var b = new DecisionEngineBuilder(1, 1, new int[] { });
            Assert.AreEqual(2, b.Values.Length);

            b = new DecisionEngineBuilder(2, 3, new int[] { });
            Assert.AreEqual(9, b.Values.Length);

            b = new DecisionEngineBuilder(5, 7, new int[] { 15, 9 });
            Assert.AreEqual(304, b.Values.Length);
        }

        [TestMethod]
        public void DecisionEngineBuilderLoadTest()
        {
            var d = new DecisionEngine(
                new DecisionLayer(
                    new Matrix(1, 1, 1.2),
                    new Matrix(1, 1, 2.3)));

            var b = new DecisionEngineBuilder(d);

            Assert.AreEqual(1, b.InputSize);
            Assert.AreEqual(1, b.OutputSize);
            Assert.AreEqual(0, b.InnerLayers.Length);
            CollectionAssert.AreEqual(new double[] { 1.2, 2.3 }, b.Values);

            d = new DecisionEngine(
                new DecisionLayer(
                    new Matrix(3, 2, 1.2, 2.3, 3.4, 4.5, 5.6, 6.7),
                    new Matrix(3, 1, 7.8, 8.9, 9.1)),
                new DecisionLayer(
                    new Matrix(2, 3, 11.0, 22.0, 33.0, 44.0, 55.0, 66.0),
                    new Matrix(2, 2, 77.0, 88.0, 99.0, 111.0)),
                new DecisionLayer(
                    new Matrix(4, 2, 201.0, 202.0, 203.0, 204.0, 205.0, 206.0, 207.0, 208.0),
                    new Matrix(4, 1, 209.0, 210.0, 211.0, 212.0)));

            b = new DecisionEngineBuilder(d);

            Assert.AreEqual(2, b.InputSize);
            Assert.AreEqual(4, b.OutputSize);
            CollectionAssert.AreEqual(new int[] { 3, 2 }, b.InnerLayers);
            CollectionAssert.AreEqual(new double[]
            {
                1.2, 2.3, 3.4, 4.5, 5.6, 6.7,
                7.8, 8.9, 9.1,
                11.0, 22.0, 33.0, 44.0, 55.0, 66.0,
                77.0, 88.0, 99.0, 111.0,
                201.0, 202.0, 203.0, 204.0, 205.0, 206.0, 207.0, 208.0,
                209.0, 210.0, 211.0, 212.0
            },
                b.Values);
        }

        [TestMethod]
        public void DecisionEngineBuilderBuildTest()
        {
            var b = new DecisionEngineBuilder(1, 1, new int[] { });
            b.Values[0] = 1.2;
            b.Values[1] = 2.3;

            var d = b.Build();

            Assert.AreEqual(1, d.InputSize);
            Assert.AreEqual(1, d.OutputSize);
            Assert.AreEqual(1, d.Layers.Length);

            var l = d.Layers[0];

            Assert.AreEqual(1, l.InputSize);
            Assert.AreEqual(1, l.OutputSize);

            Assert.AreEqual(1, l.Weights.Height);
            Assert.AreEqual(1, l.Weights.Width);
            Assert.AreEqual(1.2, l.Weights[0, 0]);

            Assert.AreEqual(1, l.Offsets.Height);
            Assert.AreEqual(1, l.Offsets.Width);
            Assert.AreEqual(2.3, l.Offsets[0, 0]);

            b = new DecisionEngineBuilder(5, 2, new int[] { 3, 4 });

            for (var i = 0; i < b.Values.Length; i++)
                b.Values[i] = i * 10.0;

            d = b.Build();

            Assert.AreEqual(5, d.InputSize);
            Assert.AreEqual(2, d.OutputSize);
            Assert.AreEqual(3, d.Layers.Length);

            l = d.Layers[0];

            Assert.AreEqual(5, l.InputSize);
            Assert.AreEqual(3, l.OutputSize);

            CollectionAssert.AreEqual(Enumerable.Range(0, 15).Select(i => i * 10.0).ToArray(), l.Weights.Values);
            CollectionAssert.AreEqual(Enumerable.Range(15, 3).Select(i => i * 10.0).ToArray(), l.Offsets.Values);

            l = d.Layers[1];

            Assert.AreEqual(3, l.InputSize);
            Assert.AreEqual(4, l.OutputSize);

            CollectionAssert.AreEqual(Enumerable.Range(18, 12).Select(i => i * 10.0).ToArray(), l.Weights.Values);
            CollectionAssert.AreEqual(Enumerable.Range(30, 4).Select(i => i * 10.0).ToArray(), l.Offsets.Values);

            l = d.Layers[2];

            Assert.AreEqual(4, l.InputSize);
            Assert.AreEqual(2, l.OutputSize);

            CollectionAssert.AreEqual(Enumerable.Range(34, 8).Select(i => i * 10.0).ToArray(), l.Weights.Values);
            CollectionAssert.AreEqual(Enumerable.Range(42, 2).Select(i => i * 10.0).ToArray(), l.Offsets.Values);
        }
    }
}