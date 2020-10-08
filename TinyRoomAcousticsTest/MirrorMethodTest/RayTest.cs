using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TinyRoomAcoustics;
using TinyRoomAcoustics.MirrorMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class RayTest
    {
        [TestMethod]
        public void Constructor()
        {
            var ray = new Ray(3.0, 5);
            Assert.AreEqual(3.0, ray.Distance, 1.0E-9);
            Assert.AreEqual(5, ray.ReflectionCount);
        }
    }
}
