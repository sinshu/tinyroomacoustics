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
    public class RoomTest
    {
        [TestMethod]
        public void Constructor()
        {
            var roomSize = DenseVector.OfArray(new double[] { 3.0, 4.0, 2.0 });
            var distanceAttenuation = new DistanceAttenuation(distance => 0.9);
            var reflectionAttenuation = new ReflectionAttenuation(frequency => 0.7);
            var room = new Room(roomSize, distanceAttenuation, reflectionAttenuation, 1);

            Assert.IsTrue((roomSize - room.Size).L2Norm() < 1.0E-6);
            Assert.AreEqual(distanceAttenuation(1), room.DistanceAttenuation(1));
            Assert.AreEqual(distanceAttenuation(3), room.DistanceAttenuation(3));
            Assert.AreEqual(reflectionAttenuation(0), room.ReflectionAttenuation(0));
            Assert.AreEqual(reflectionAttenuation(1), room.ReflectionAttenuation(1));
            Assert.AreEqual(reflectionAttenuation(3), room.ReflectionAttenuation(3));
            Assert.AreEqual(1, room.MaxReflectionCount);
        }
    }
}
