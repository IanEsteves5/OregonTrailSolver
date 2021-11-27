using Microsoft.VisualStudio.TestTools.UnitTesting;
using OregonTrail.Shared;

namespace OregonTrail.Test.Shared
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void MatrixConstructorTest()
        {
            var m = new Matrix(0, 0, new double[] { });

            Assert.AreEqual(0, m.Height);
            Assert.AreEqual(0, m.Width);

            m = new Matrix(1, 1, new double[] { 1.2 });

            Assert.AreEqual(1, m.Height);
            Assert.AreEqual(1, m.Width);
            Assert.AreEqual(1.2, m[0, 0]);

            m = new Matrix(2, 3, new double[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7 });

            Assert.AreEqual(2, m.Height);
            Assert.AreEqual(3, m.Width);
            Assert.AreEqual(1.2, m[0, 0]);
            Assert.AreEqual(2.3, m[0, 1]);
            Assert.AreEqual(3.4, m[0, 2]);
            Assert.AreEqual(4.5, m[1, 0]);
            Assert.AreEqual(5.6, m[1, 1]);
            Assert.AreEqual(6.7, m[1, 2]);
        }

        [TestMethod]
        public void MatrixAddConstantTest()
        {
            var m = new Matrix(2, 3, new double[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7 });
            var r = m + 10.0;

            Assert.AreEqual(2, r.Height);
            Assert.AreEqual(3, r.Width);
            Assert.AreEqual(11.2, r[0, 0]);
            Assert.AreEqual(12.3, r[0, 1]);
            Assert.AreEqual(13.4, r[0, 2]);
            Assert.AreEqual(14.5, r[1, 0]);
            Assert.AreEqual(15.6, r[1, 1]);
            Assert.AreEqual(16.7, r[1, 2]);
        }

        [TestMethod]
        public void MatrixMultiplyConstantTest()
        {
            var m = new Matrix(2, 3, new double[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7 });
            var r = m * 10.0;

            Assert.AreEqual(2, r.Height);
            Assert.AreEqual(3, r.Width);
            Assert.AreEqual(12.0, r[0, 0]);
            Assert.AreEqual(23.0, r[0, 1]);
            Assert.AreEqual(34.0, r[0, 2]);
            Assert.AreEqual(45.0, r[1, 0]);
            Assert.AreEqual(56.0, r[1, 1]);
            Assert.AreEqual(67.0, r[1, 2]);
        }

        [TestMethod]
        public void MatrixAdditionTest()
        {
            var m = new Matrix(2, 3, new double[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7 });
            var n = new Matrix(2, 3, new double[] { 10.0, 20.0, 30.0, 40.0, 50.0, 60.0 });
            var r = m + n;

            Assert.AreEqual(2, r.Height);
            Assert.AreEqual(3, r.Width);
            Assert.AreEqual(11.2, r[0, 0]);
            Assert.AreEqual(22.3, r[0, 1]);
            Assert.AreEqual(33.4, r[0, 2]);
            Assert.AreEqual(44.5, r[1, 0]);
            Assert.AreEqual(55.6, r[1, 1]);
            Assert.AreEqual(66.7, r[1, 2]);
        }

        [TestMethod]
        public void MatrixMultiplicationTest()
        {
            var m = new Matrix(1, 1, new double[] { 1.2 });
            var n = new Matrix(1, 1, new double[] { 10.0 });
            var r = m * n;

            Assert.AreEqual(1, r.Height);
            Assert.AreEqual(1, r.Width);
            Assert.AreEqual(12.0, r[0, 0]);

            m = new Matrix(1, 3, new double[] { 1.2, 2.3, 3.4 });
            n = new Matrix(3, 1, new double[] { 10.0, 20.0, 30.0 });
            r = m * n;

            Assert.AreEqual(1, r.Height);
            Assert.AreEqual(1, r.Width);
            Assert.AreEqual(160.0, r[0, 0]);

            m = new Matrix(3, 1, new double[] { 1.2, 2.3, 3.4 });
            n = new Matrix(1, 3, new double[] { 10.0, 20.0, 30.0 });
            r = m * n;

            Assert.AreEqual(3, r.Height);
            Assert.AreEqual(3, r.Width);
            Assert.AreEqual(12.0, r[0, 0]);
            Assert.AreEqual(24.0, r[0, 1]);
            Assert.AreEqual(36.0, r[0, 2]);
            Assert.AreEqual(23.0, r[1, 0]);
            Assert.AreEqual(46.0, r[1, 1]);
            Assert.AreEqual(69.0, r[1, 2]);
            Assert.AreEqual(34.0, r[2, 0]);
            Assert.AreEqual(68.0, r[2, 1]);
            Assert.AreEqual(102.0, r[2, 2]);

            m = new Matrix(2, 3, new double[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7 });
            n = new Matrix(3, 2, new double[] { 10.0, 20.0, 30.0, 40.0, 50.0, 60.0 });
            r = m * n;

            Assert.AreEqual(2, r.Height);
            Assert.AreEqual(2, r.Width);
            Assert.AreEqual(251.0, r[0, 0]);
            Assert.AreEqual(320.0, r[0, 1]);
            Assert.AreEqual(548.0, r[1, 0]);
            Assert.AreEqual(716.0, r[1, 1]);
        }
    }
}
