﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using TinyRoomAcoustics.Dsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class FramingTest_GetFrameComplex
    {
        [DataTestMethod]
        [DataRow(100, 30, 50)]
        [DataRow(50, 50, 0)]
        [DataRow(100, 10, 0)]
        [DataRow(100, 10, 90)]
        public void GetFrame_Normal_CheckWithLinq(int sourceLength, int windowLength, int position)
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
        [DataRow(100, 30, 50)]
        [DataRow(50, 50, 0)]
        [DataRow(100, 10, 0)]
        [DataRow(100, 10, 90)]
        public void GetFrame_Normal_CheckWithNaiveImplementation(int sourceLength, int windowLength, int position)
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

        [DataTestMethod]
        [DataRow(100, 30, -1)]
        [DataRow(100, 30, -10)]
        [DataRow(100, 30, -29)]
        public void GetFrame_LeftExceedance_PartialOverlap(int sourceLength, int windowLength, int position)
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

        [DataTestMethod]
        [DataRow(100, 30, -30)]
        [DataRow(100, 30, -50)]
        public void GetFrame_LeftExceedance_NoOverlap(int sourceLength, int windowLength, int position)
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

        [DataTestMethod]
        [DataRow(100, 30, 71)]
        [DataRow(100, 30, 85)]
        [DataRow(100, 30, 99)]
        public void GetFrame_RightExceedance_PartialOverlap(int sourceLength, int windowLength, int position)
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

        [DataTestMethod]
        [DataRow(100, 30, 100)]
        [DataRow(100, 30, 150)]
        public void GetFrame_RightExceedance_NoOverlap(int sourceLength, int windowLength, int position)
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

        [DataTestMethod]
        [DataRow(10, 30, -10)]
        [DataRow(10, 30, 0)]
        [DataRow(10, 30, -20)]
        public void GetFrame_SourceIsShorterThanFrame(int sourceLength, int windowLength, int position)
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

        public static Complex[] GetFrameComplex_Naive(double[] source, double[] window, int position)
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