using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using TinyRoomAcoustics.Dsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class FramingTest_OverlapAdd_Complex
    {
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
        public void GetFrame_CheckWithNaiveImplementation(int destinationLength, int windowLength, int position)
        {
            var random = new Random(57);
            var destination = Enumerable.Range(0, destinationLength).Select(t => random.NextDouble()).ToArray();
            var frame = Enumerable.Range(0, windowLength).Select(t => (Complex)random.NextDouble()).ToArray();
            var window = WindowFunctions.Hann(windowLength);

            var expected = destination.ToArray();
            OverlapAdd_Naive(expected, frame, window, position);

            var actual = destination.ToArray();
            Framing.OverlapAdd(actual, frame, window, position);

            for (var t = 0; t < actual.Length; t++)
            {
                Assert.AreEqual(expected[t], actual[t], 1.0E-9);
            }
        }

        public static void OverlapAdd_Naive(double[] destination, Complex[] frame, double[] window, int position)
        {
            for (var ft = 0; ft < frame.Length; ft++)
            {
                var dt = position + ft;
                if (0 <= dt && dt < destination.Length)
                {
                    destination[dt] += window[ft] * frame[ft].Real;
                }
            }
        }
    }
}
