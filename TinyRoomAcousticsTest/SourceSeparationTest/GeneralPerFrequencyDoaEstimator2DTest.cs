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
        public void Estimate1()
        {
            var sampleRate = 16000;
            var frameLength = 1024;

            var microphones = new Microphone[]
            {
                new Microphone(0.00, 0.00, 1.00),
                new Microphone(0.00, 0.03, 1.00),
                new Microphone(0.03, 0.00, 1.00)
            };

            var maxSpace = Math.Sqrt(2) * 0.03;

            EstimateCore(microphones, sampleRate, frameLength, maxSpace);
        }

        [TestMethod]
        public void Estimate2()
        {
            var sampleRate = 16000;
            var frameLength = 1024;

            var microphones = new Microphone[]
            {
                new Microphone(0.000,                0.000, 1.0),
                new Microphone(0.030,                0.000, 1.0),
                new Microphone(0.015, Math.Sqrt(3) * 0.015, 1.0)
            };

            var maxSpace = 0.03;

            EstimateCore(microphones, sampleRate, frameLength, maxSpace);
        }

        [TestMethod]
        public void Estimate3()
        {
            var sampleRate = 16000;
            var frameLength = 1024;

            var microphones = new Microphone[]
            {
                new Microphone(0.00, 0.00, 1.0),
                new Microphone(0.02, 0.00, 1.0),
                new Microphone(0.00, 0.02, 1.0),
                new Microphone(0.02, 0.02, 1.0)
            };

            var maxSpace = Math.Sqrt(2) * 0.02;

            EstimateCore(microphones, sampleRate, frameLength, maxSpace);
        }

        private void EstimateCore(Microphone[] microphones, int sampleRate, int frameLength, double maxSpace)
        {
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

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void InvalidMicrophoneArrangement()
        {
            var sampleRate = 16000;
            var frameLength = 1024;

            var microphones = new Microphone[]
            {
                new Microphone(0.00, 0.00, 1.0),
                new Microphone(0.03, 0.03, 1.0),
                new Microphone(0.06, 0.06, 1.0)
            };

            new GeneralPerFrequencyDoaEstimator2D(microphones, sampleRate, frameLength);
        }
    }
}
