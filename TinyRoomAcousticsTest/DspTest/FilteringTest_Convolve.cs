using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using TinyRoomAcoustics;
using TinyRoomAcoustics.Dsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class FilteringTest_Convolve
    {
        [DataTestMethod]
        [DataRow(100, 8)]
        [DataRow(123, 16)]
        [DataRow(25, 32)]
        [DataRow(100, 7)]
        [DataRow(123, 12)]
        [DataRow(25, 57)]
        [DataRow(10, 1)]
        public void CheckWithDelayFilter(int sourceLength, int firLength)
        {
            var random = new Random(57);
            var source = Enumerable.Range(0, sourceLength).Select(t => random.NextDouble()).ToArray();

            var delayFilter = new double[firLength];
            delayFilter[delayFilter.Length - 1] = 1.0;

            var expected = Enumerable.Repeat(0.0, firLength - 1).Concat(source).Take(sourceLength).ToArray();
            var actual = Filtering.Convolve(source, delayFilter);
            for (var t = 0; t < actual.Length; t++)
            {
                Assert.AreEqual(expected[t], actual[t], 1.0E-6);
            }
        }

        [DataTestMethod]
        [DataRow(100, 8)]
        [DataRow(123, 16)]
        [DataRow(25, 32)]
        [DataRow(100, 7)]
        [DataRow(123, 12)]
        [DataRow(25, 57)]
        [DataRow(10, 1)]
        public void CheckWithNaiveImplementation(int sourceLength, int firLength)
        {
            var random = new Random(57);
            var source = Enumerable.Range(0, sourceLength).Select(t => random.NextDouble()).ToArray();
            var fir = Enumerable.Range(0, firLength).Select(t => random.NextDouble()).ToArray();

            var expected = Convolve_Naive(source, fir);
            var actual = Filtering.Convolve(source, fir);
            for (var t = 0; t < actual.Length; t++)
            {
                Assert.AreEqual(expected[t], actual[t], 1.0E-6);
            }
        }

        private double[] Convolve_Naive(double[] source, double[] fir)
        {
            var destination = new double[source.Length];
            for (var dt = 0; dt < source.Length; dt++)
            {
                var sum = 0.0;
                var du = dt;
                for (var ft = 0; ft < fir.Length; ft++)
                {
                    if (du >= 0)
                    {
                        sum += fir[ft] * source[du];
                        du--;
                    }
                }
                destination[dt] = sum;
            }
            return destination;
        }
    }
}
