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
    public class SteeringVectorTest_FromFarFieldGeometry
    {
        [TestMethod]
        public void CheckWithDelayFilter_Case1()
        {
            var delaySampleCount = 3;

            var sampleRate = 16000;
            var dftLength = 1024;

            var time = (double)delaySampleCount / sampleRate;
            var distance = AcousticConstants.SoundSpeed * time;

            var soundSource = new SoundSource(1.0, 1.0, 1.0);

            var microphones = new Microphone[]
            {
                new Microphone(1.0 + 1 * distance, 1.0, 1.0),
                new Microphone(1.0 + 2 * distance, 1.0, 1.0),
                new Microphone(1.0 + 3 * distance, 1.0, 1.0)
            };

            var sv = SteeringVector.FromFarFieldGeometry(microphones, 0.0, 0.0, sampleRate, dftLength);

            Assert.AreEqual(dftLength / 2 + 1, sv.Length);

            var delayFilters = new Complex[][]
            {
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 0),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 3),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 6)
            };

            for (var w = 0; w < dftLength / 2 + 1; w++)
            {
                for (var ch = 0; ch < 3; ch++)
                {
                    var actual = sv[w][ch];
                    var expected = delayFilters[ch][w];
                    Assert.AreEqual(expected.Real, actual.Real, 1.0E-6);
                    Assert.AreEqual(expected.Imaginary, actual.Imaginary, 1.0E-6);
                }
            }
        }

        [TestMethod]
        public void CheckWithDelayFilter_Case2()
        {
            var delaySampleCount = 3;

            var sampleRate = 16000;
            var dftLength = 1024;

            var time = (double)delaySampleCount / sampleRate;
            var distance = AcousticConstants.SoundSpeed * time;

            var soundSource = new SoundSource(1.0, 1.0, 1.0);

            var microphones = new Microphone[]
            {
                new Microphone(1.0 + 1 * distance, 1.0, 1.0),
                new Microphone(1.0 + 2 * distance, 1.0, 1.0),
                new Microphone(1.0 + 3 * distance, 1.0, 1.0)
            };

            var sv = SteeringVector.FromFarFieldGeometry(microphones, Math.PI / 4, 0.0, sampleRate, dftLength);

            Assert.AreEqual(dftLength / 2 + 1, sv.Length);

            var delayFilters = new Complex[][]
            {
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 0 / Math.Sqrt(2)),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 3 / Math.Sqrt(2)),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 6 / Math.Sqrt(2))
            };

            for (var w = 0; w < dftLength / 2 + 1; w++)
            {
                for (var ch = 0; ch < 3; ch++)
                {
                    var actual = sv[w][ch];
                    var expected = delayFilters[ch][w];
                    Assert.AreEqual(expected.Real, actual.Real, 1.0E-6);
                    Assert.AreEqual(expected.Imaginary, actual.Imaginary, 1.0E-6);
                }
            }
        }

        [TestMethod]
        public void CheckWithDelayFilter_Case3()
        {
            var delaySampleCount = 3;

            var sampleRate = 16000;
            var dftLength = 1024;

            var time = (double)delaySampleCount / sampleRate;
            var distance = AcousticConstants.SoundSpeed * time;

            var soundSource = new SoundSource(1.0, 1.0, 1.0);

            var microphones = new Microphone[]
            {
                new Microphone(1.0 + 1 * distance, 1.0, 1.0),
                new Microphone(1.0 + 2 * distance, 1.0, 1.0),
                new Microphone(1.0 + 3 * distance, 1.0, 1.0)
            };

            var sv = SteeringVector.FromFarFieldGeometry(microphones, 0.0, Math.PI / 4, sampleRate, dftLength);

            Assert.AreEqual(dftLength / 2 + 1, sv.Length);

            var delayFilters = new Complex[][]
            {
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 0 / Math.Sqrt(2)),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 3 / Math.Sqrt(2)),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 6 / Math.Sqrt(2))
            };

            for (var w = 0; w < dftLength / 2 + 1; w++)
            {
                for (var ch = 0; ch < 3; ch++)
                {
                    var actual = sv[w][ch];
                    var expected = delayFilters[ch][w];
                    Assert.AreEqual(expected.Real, actual.Real, 1.0E-6);
                    Assert.AreEqual(expected.Imaginary, actual.Imaginary, 1.0E-6);
                }
            }
        }

        [TestMethod]
        public void CheckWithDelayFilter_Case4()
        {
            var delaySampleCount = 3;

            var sampleRate = 16000;
            var dftLength = 1024;

            var time = (double)delaySampleCount / sampleRate;
            var distance = AcousticConstants.SoundSpeed * time;

            var soundSource = new SoundSource(1.0, 1.0, 1.0);

            var microphones = new Microphone[]
            {
                new Microphone(1.0 + 1 * distance, 1.0, 1.0),
                new Microphone(1.0 + 2 * distance, 1.0, 1.0),
                new Microphone(1.0 + 3 * distance, 1.0, 1.0)
            };

            var sv = SteeringVector.FromFarFieldGeometry(microphones, Math.PI / 2, Math.PI / 4, sampleRate, dftLength);

            Assert.AreEqual(dftLength / 2 + 1, sv.Length);

            var delayFilters = new Complex[][]
            {
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 0),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 0),
                Filtering.CreateFrequencyDomainDelayFilter(dftLength, 0)
            };

            for (var w = 0; w < dftLength / 2 + 1; w++)
            {
                for (var ch = 0; ch < 3; ch++)
                {
                    var actual = sv[w][ch];
                    var expected = delayFilters[ch][w];
                    Assert.AreEqual(expected.Real, actual.Real, 1.0E-6);
                    Assert.AreEqual(expected.Imaginary, actual.Imaginary, 1.0E-6);
                }
            }
        }
    }
}
