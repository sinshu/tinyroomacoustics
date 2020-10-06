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
    public class MirroredRoomIndexTest
    {
        [TestMethod]
        public void Constructor()
        {
            var index = new MirroredRoomIndex(1, 2, 3);
            Assert.AreEqual(1, index.X);
            Assert.AreEqual(2, index.Y);
            Assert.AreEqual(3, index.Z);
        }
    }
}
