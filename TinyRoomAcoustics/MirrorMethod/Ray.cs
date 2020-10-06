using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics.MirrorMethod
{
    public sealed class Ray
    {
        private readonly double distance;
        private readonly int reflectionCount;

        public Ray(double distance, int reflectionCount)
        {
            this.distance = distance;
            this.reflectionCount = reflectionCount;
        }

        public double Distance => distance;
        public int ReflectionCount => reflectionCount;
    }
}
