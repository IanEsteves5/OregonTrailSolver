using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace OregonTrail.Shared
{
    public class Matrix
    {
        public int Height { get; }
        public int Width { get; }

        public int Size { get; }
        public double[] Values { get; }

        public double this[int i, int j]
        {
            get
            {
                Contract.Requires(i >= 0);
                Contract.Requires(i < Height);
                Contract.Requires(j >= 0);
                Contract.Requires(j < Width);

                return Values[i * Width + j];
            }
        }

        public Matrix(int height, int width, params double[] values)
        {
            Contract.Requires(height > 0);
            Contract.Requires(width > 0);
            Contract.Requires(values != null);

            Size = height * width;
            Contract.Requires(values.Length == Size);

            Height = height;
            Width = width;
            Values = values;
        }

        public Matrix Select(Func<double, double> f)
        {
            return new Matrix(Height, Width, Values.Select(f).ToArray());
        }

        public Matrix Zip(Matrix m, Func<double, double, double> f)
        {
            Contract.Requires(Height == m.Height);
            Contract.Requires(Width == m.Width);

            return new Matrix(Height, Width, Values.Zip(m.Values, f).ToArray());
        }

        public Matrix Multiply(Matrix m)
        {
            Contract.Requires(Width == m.Height);

            var s = Height * m.Width;
            var r = new double[s];

            for (var i = 0; i < Height; i++)
                for (var j = 0; j < m.Width; j++)
                {
                    var d = 0.0;
                    for (var k = 0; k < Width; k++)
                        d += this[i, k] * m[k, j];
                    r[i * m.Width + j] = d;
                }

            return new Matrix(Height, m.Width, r);
        }

        public override string ToString()
        {
            return "[" + Height + ", " + Width + "]";
        }

        public static Matrix operator +(Matrix m, double d)
        {
            return m.Select(x => x + d);
        }

        public static Matrix operator *(Matrix m, double d)
        {
            return m.Select(x => x * d);
        }

        public static Matrix operator +(Matrix m, Matrix n)
        {
            return m.Zip(n, (x, y) => x + y);
        }

        public static Matrix operator *(Matrix m, Matrix n)
        {
            return m.Multiply(n);
        }
    }
}
