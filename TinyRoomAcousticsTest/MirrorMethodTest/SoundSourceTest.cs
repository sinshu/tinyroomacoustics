using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TinyRoomAcoustics.MirrorMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class SoundSourceTest
    {
        [TestMethod]
        public void Constructor_Case1()
        {
            var mic = new SoundSource(1.0, 2.0, 3.0);
            Assert.AreEqual(1.0, mic.Position[0], 1.0E-9);
            Assert.AreEqual(2.0, mic.Position[1], 1.0E-9);
            Assert.AreEqual(3.0, mic.Position[2], 1.0E-9);
        }

        [TestMethod]
        public void Constructor_Case2()
        {
            var array = new double[] { 1.0, 2.0, 3.0 };
            var position = DenseVector.OfArray(array);
            var mic = new SoundSource(position);
            Assert.AreEqual(1.0, mic.Position[0], 1.0E-9);
            Assert.AreEqual(2.0, mic.Position[1], 1.0E-9);
            Assert.AreEqual(3.0, mic.Position[2], 1.0E-9);
        }
    }
}
