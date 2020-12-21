using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TinyRoomAcoustics;
using TinyRoomAcoustics.Dsp;
using TinyRoomAcoustics.SourceSeparation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class GeneralPerFrequencyDoaEstimator2DTest
    {
        [TestMethod]
        public void Estimate()
        {
            var sampleRate = 16000;
            var frameLength = 1024;

            var microphones = new Microphone[]
            {
                new Microphone(0.00, 0.00, 1.00),
                new Microphone(0.00, 0.03, 1.00),
                new Microphone(0.03, 0.00, 1.00),
            };

            var maxSpace = Math.Sqrt(2) * 0.03;

            var estimator = new GeneralPerFrequencyDoaEstimator2D(microphones, sampleRate, frameLength);

            for (var deg = 1; deg < 360; deg++)
            {
                var expectedDoa = Math.PI * deg / 180 - Math.PI;

                MathNet.Numerics.LinearAlgebra.Vector<double> dv = DenseVector.OfArray(new double[]
                {
                    Math.Cos(expectedDoa),
                    Math.Sin(expectedDoa),
                    0
                });

                var dfts = new Complex[frameLength][];
                for (var ch = 0; ch < microphones.Length; ch++)
                {
                    var distance = microphones[ch].Position * dv;
                    var delaySampleCount = -distance / AcousticConstants.SoundSpeed * sampleRate;
                    var delayFilter = Filtering.CreateFrequencyDomainDelayFilter(frameLength, delaySampleCount);
                    var dft = new Complex[frameLength];
                    Array.Copy(delayFilter, dft, delayFilter.Length);
                    dfts[ch] = dft;
                }

                var actualDoa = estimator.Estimate(dfts);

                for (var w = 1; w < frameLength / 2 + 1; w++)
                {
                    var waveLength = (double)frameLength / w / sampleRate * AcousticConstants.SoundSpeed;

                    if (maxSpace + 1.0E-6 < waveLength / 2)
                    {
                        Assert.AreEqual(expectedDoa, actualDoa[w], 1.0E-6);
                    }
                }
            }
        }
    }
}
