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
    public class FramingTest_GetFrameComplex_SingleChannel
    {
        [DataTestMethod]
        [DataRow(100, 30, 50)]
        [DataRow(50, 50, 0)]
        [DataRow(100, 10, 0)]
        [DataRow(100, 10, 90)]
        public void CheckWithLinq(int sourceLength, int windowLength, int position)
        {
            var random = new Random(57);
            var source = Enumerable.Range(0, sourceLength).Select(t => random.NextDouble()).ToArray();
            var window = WindowFunctions.Hann(windowLength);

            var expected = source.Skip(position).Take(windowLength).Select(value => (Complex)value).ToArray();
            for (var t = 0; t < expected.Length; t++)
            {
                expected[t] *= window[t];
            }

            var actual = Framing.GetFrameComplex(source, window, position);

            for (var t = 0; t < actual.Length; t++)
            {
                Assert.AreEqual(expected[t].Real, actual[t].Real, 1.0E-9);
                Assert.AreEqual(expected[t].Imaginary, actual[t].Imaginary, 1.0E-9);
            }
        }

        [DataTestMethod]
        [DataRow(100, 30, 50)] // Normal
        [DataRow(50, 50, 0)] // Normal
        [DataRow(100, 10, 0)] // Normal
        [DataRow(100, 10, 90)] // Normal
        [DataRow(100, 30, -1)] // Left exceedance, partial overlap
        [DataRow(100, 30, -10)] // Left exceedance, partial overlap
        [DataRow(100, 30, -29)] // Left exceedance, partial overlap
        [DataRow(100, 30, -30)] // Left exceedance, no overlap
        [DataRow(100, 30, -50)] // Left exceedance, no overlap
        [DataRow(100, 30, 71)] // Right exceedance, partial overlap
        [DataRow(100, 30, 85)] // Right exceedance, partial overlap
        [DataRow(100, 30, 99)] // Right exceedance, partial overlap
        [DataRow(100, 30, 100)] // Right exceedance, no overlap
        [DataRow(100, 30, 150)] // Right exceedance, no overlap
        [DataRow(10, 30, -10)] // Source is shorter than frame
        [DataRow(10, 30, 0)] // Source is shorter than frame
        [DataRow(10, 30, -20)] // Source is shorter than frame
        public void CheckWithNaiveImplementation(int sourceLength, int windowLength, int position)
        {
            var random = new Random(57);
            var source = Enumerable.Range(0, sourceLength).Select(t => random.NextDouble()).ToArray();
            var window = WindowFunctions.Hann(windowLength);

            var expected = GetFrameComplex_Naive(source, window, position);

            var actual = Framing.GetFrameComplex(source, window, position);

            for (var t = 0; t < actual.Length; t++)
            {
                Assert.AreEqual(expected[t].Real, actual[t].Real, 1.0E-9);
                Assert.AreEqual(expected[t].Imaginary, actual[t].Imaginary, 1.0E-9);
            }
        }

        private static Complex[] GetFrameComplex_Naive(double[] source, double[] window, int position)
        {
            var frame = new Complex[window.Length];
            for (var ft = 0; ft < frame.Length; ft++)
            {
                var st = position + ft;
                if (0 <= st && st < source.Length)
                {
                    frame[ft] = window[ft] * source[st];
                }
            }
            return frame;
        }
    }
}
