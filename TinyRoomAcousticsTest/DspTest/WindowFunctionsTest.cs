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
    public class WindowFunctionsTest
    {
        [TestMethod]
        public void Hann()
        {
            var hann = WindowFunctions.Hann(256);

            Assert.AreEqual(256, hann.Length);

            Assert.AreEqual(0.0, hann.First(), 1.0E-6);
            Assert.AreEqual(0.0, hann.Last(), 0.001);

            Assert.AreEqual(1.0, hann[128], 1.0E-6);

            Assert.AreEqual(0.5, hann[64], 1.0E-6);
            Assert.AreEqual(0.5, hann[192], 1.0E-6);

            Assert.AreEqual(0.0, hann.Min(), 1.0E-6);
            Assert.AreEqual(1.0, hann.Max(), 1.0E-6);
            Assert.AreEqual(0.5, hann.Average(), 1.0E-6);
        }

        [TestMethod]
        public void SqrtHann()
        {
            var hann = WindowFunctions.Hann(256);

            var sqrtHann = WindowFunctions.SqrtHann(256);

            for (var t = 0; t < 256; t++)
            {
                Assert.AreEqual(Math.Sqrt(hann[t]), sqrtHann[t], 1.0E-6);
            }
        }
    }
}
