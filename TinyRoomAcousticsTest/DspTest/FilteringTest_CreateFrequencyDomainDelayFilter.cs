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
    public class FilteringTest_CreateFrequencyDomainDelayFilter
    {
        [DataTestMethod]
        [DataRow(16)]
        [DataRow(64)]
        [DataRow(256)]
        public void CheckTimeDomainSignal(int dftLength)
        {
            for (var delaySampleCount = 0; delaySampleCount < dftLength; delaySampleCount++)
            {
                var filter = Filtering.CreateFrequencyDomainDelayFilter(dftLength, delaySampleCount);

                var timeDomainSignal = new Complex[dftLength];
                timeDomainSignal[0] = filter[0];
                for (var w = 1; w < dftLength / 2; w++)
                {
                    timeDomainSignal[w] = filter[w];
                    timeDomainSignal[dftLength - w] = filter[w].Conjugate();
                }
                timeDomainSignal[dftLength / 2] = filter[dftLength / 2];
                Fourier.Inverse(timeDomainSignal, FourierOptions.AsymmetricScaling);

                for (var t = 0; t < dftLength; t++)
                {
                    if (t == delaySampleCount)
                    {
                        Assert.AreEqual(1.0, timeDomainSignal[t].Real, 1.0E-6);
                        Assert.AreEqual(0.0, timeDomainSignal[t].Imaginary, 1.0E-6);
                    }
                    else
                    {
                        Assert.AreEqual(0.0, timeDomainSignal[t].Real, 1.0E-6);
                        Assert.AreEqual(0.0, timeDomainSignal[t].Imaginary, 1.0E-6);
                    }
                }
            }
        }
    }
}
