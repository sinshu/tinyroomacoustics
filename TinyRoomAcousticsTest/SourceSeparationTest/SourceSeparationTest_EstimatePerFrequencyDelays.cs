using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using TinyRoomAcoustics;
using TinyRoomAcoustics.SourceSeparation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class SourceSeparationTest_EstimatePerFrequencyDelays
    {
        [DataTestMethod]
        [DataRow(64, 0, 1)]
        [DataRow(64, 1, 0)]
        [DataRow(512, 10, 12)]
        [DataRow(512, 12, 10)]
        [DataRow(1024, 16, 32)]
        [DataRow(1024, 32, 16)]
        public void CheckWithIntegerDelay_Impulse(int frameLength, int peakPosition1, int peakPosition2)
        {
            var expectedDelay = peakPosition2 - peakPosition1;

            var x = new Complex[frameLength];
            var y = new Complex[frameLength];

            x[peakPosition1] = 1;
            y[peakPosition2] = 1;

            Fourier.Forward(x, FourierOptions.AsymmetricScaling);
            Fourier.Forward(y, FourierOptions.AsymmetricScaling);

            var delays = SourceSeparation.EstimatePerFrequencyDelays(x, y);

            for (var w = 1; w < delays.Length; w++)
            {
                var waveLength = (double)frameLength / w;

                if (Math.Abs(expectedDelay) + 1.0E-6 < waveLength / 2)
                {
                    Assert.AreEqual(expectedDelay, delays[w], 1.0E-6);
                }
            }
        }

        [DataTestMethod]
        [DataRow(64, 1)]
        [DataRow(64, -1)]
        [DataRow(512, 2)]
        [DataRow(512, -2)]
        [DataRow(1024, 16)]
        [DataRow(1024, -16)]
        public void CheckWithIntegerDelay_WhiteNoise(int frameLength, int expectedDelay)
        {
            var random = new Random(57);

            var x = new Complex[frameLength];
            var y = new Complex[frameLength];

            for (var t = 0; t < frameLength; t++)
            {
                var u = (t + expectedDelay) % frameLength;
                if (u < 0)
                {
                    u += frameLength;
                }

                x[t] = 2 * random.NextDouble() - 1;
                y[u] = x[t];
            }

            Fourier.Forward(x, FourierOptions.AsymmetricScaling);
            Fourier.Forward(y, FourierOptions.AsymmetricScaling);

            var delays = SourceSeparation.EstimatePerFrequencyDelays(x, y);

            for (var w = 1; w < delays.Length; w++)
            {
                var waveLength = (double)frameLength / w;

                if (Math.Abs(expectedDelay) + 1.0E-6 < waveLength / 2)
                {
                    Assert.AreEqual(expectedDelay, delays[w], 1.0E-6);
                }
            }
        }
    }
}
