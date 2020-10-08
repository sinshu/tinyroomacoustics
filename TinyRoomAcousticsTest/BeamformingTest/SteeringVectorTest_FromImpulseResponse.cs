using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using TinyRoomAcoustics;
using TinyRoomAcoustics.Beamforming;
using TinyRoomAcoustics.Dsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class SteeringVectorTest_FromImpulseResponse
    {
        [TestMethod]
        public void CheckWithDelayFilter()
        {
            var channelCount = 3;

            var impulseResponse = new double[][]
            {
                new double[] { 0.0, 0.0, 0.0, 1.0 },
                new double[] { 0.0, 0.0, 1.0 },
                new double[] { 0.0, 0.0, 0.0, 0.0, 1.0 }
            };

            var delaySampleCounts = new int[]
            {
                3,
                2,
                4
            };

            var dftLength = 1024;

            var sv = SteeringVector.FromImpulseResponse(impulseResponse, dftLength);

            Assert.AreEqual(dftLength / 2 + 1, sv.Length);

            for (var ch = 0; ch < channelCount; ch++)
            {
                var expected = Filtering.CreateFrequencyDomainDelayFilter(dftLength, delaySampleCounts[ch]);

                for (var w = 0; w < dftLength / 2 + 1; w++)
                {
                    Assert.AreEqual(expected[w].Real, sv[w][ch].Real, 1.0E-6);
                    Assert.AreEqual(expected[w].Imaginary, sv[w][ch].Imaginary, 1.0E-6);
                }
            }
        }
    }
}
