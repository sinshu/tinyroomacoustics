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
    public class FramingTest_GetFrame_MultiChannel
    {
        [DataTestMethod]
        [DataRow(1, 100, 30, 50)]
        [DataRow(2, 50, 50, 0)]
        [DataRow(3, 100, 10, 0)]
        [DataRow(4, 100, 10, 90)]
        public void CheckWithLinq(int channelCount, int sourceLength, int windowLength, int position)
        {
            var random = new Random(57);
            var source = new double[channelCount][];
            for (var ch = 0; ch < channelCount; ch++)
            {
                source[ch] = Enumerable.Range(0, sourceLength).Select(t => random.NextDouble()).ToArray();
            }
            var window = WindowFunctions.Hann(windowLength);

            var expected = new double[channelCount][];
            for (var ch = 0; ch < channelCount; ch++)
            {
                expected[ch] = source[ch].Skip(position).Take(windowLength).ToArray();
                for (var t = 0; t < expected.Length; t++)
                {
                    expected[ch][t] *= window[t];
                }
            }

            var actual = Framing.GetFrame(source, window, position);

            for (var ch = 0; ch < channelCount; ch++)
            {
                for (var t = 0; t < actual.Length; t++)
                {
                    Assert.AreEqual(expected[ch][t], actual[ch][t], 1.0E-9);
                }
            }
        }

        [DataTestMethod]
        [DataRow(1, 100, 30, 50)] // Normal
        [DataRow(2, 50, 50, 0)] // Normal
        [DataRow(3, 100, 10, 0)] // Normal
        [DataRow(4, 100, 10, 90)] // Normal
        [DataRow(3, 100, 30, -1)] // Left exceedance, partial overlap
        [DataRow(2, 100, 30, -10)] // Left exceedance, partial overlap
        [DataRow(1, 100, 30, -29)] // Left exceedance, partial overlap
        [DataRow(2, 100, 30, -30)] // Left exceedance, no overlap
        [DataRow(3, 100, 30, -50)] // Left exceedance, no overlap
        [DataRow(4, 100, 30, 71)] // Right exceedance, partial overlap
        [DataRow(3, 100, 30, 85)] // Right exceedance, partial overlap
        [DataRow(2, 100, 30, 99)] // Right exceedance, partial overlap
        [DataRow(1, 100, 30, 100)] // Right exceedance, no overlap
        [DataRow(2, 100, 30, 150)] // Right exceedance, no overlap
        [DataRow(3, 10, 30, -10)] // Source is shorter than frame
        [DataRow(4, 10, 30, 0)] // Source is shorter than frame
        [DataRow(5, 10, 30, -20)] // Source is shorter than frame
        public void CheckWithNaiveImplementation(int channelCount, int sourceLength, int windowLength, int position)
        {
            var random = new Random(57);
            var source = new double[channelCount][];
            for (var ch = 0; ch < channelCount; ch++)
            {
                source[ch] = Enumerable.Range(0, sourceLength).Select(t => random.NextDouble()).ToArray();
            }
            var window = WindowFunctions.Hann(windowLength);

            var expected = GetFrame_Naive(source, window, position);

            var actual = Framing.GetFrame(source, window, position);

            for (var ch = 0; ch < channelCount; ch++)
            {
                for (var t = 0; t < actual.Length; t++)
                {
                    Assert.AreEqual(expected[ch][t], actual[ch][t], 1.0E-9);
                }
            }
        }

        private static double[] GetFrame_Naive(double[] source, double[] window, int position)
        {
            var frame = new double[window.Length];
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

        private static double[][] GetFrame_Naive(double[][] source, double[] window, int position)
        {
            return source.Select(s => GetFrame_Naive(s, window, position)).ToArray();
        }
    }
}
