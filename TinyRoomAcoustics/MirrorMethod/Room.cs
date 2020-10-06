using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics.MirrorMethod
{
    public sealed class Room
    {
        public static readonly double SoundSpeed = 340.0;

        private readonly Vector<double> size;
        private readonly DistanceAttenuation distanceAttenuation;
        private readonly ReflectionAttenuation reflectionAttenuation;
        private readonly int maxReflectionCount;

        public Room(Vector<double> size, DistanceAttenuation distanceAttenuation, ReflectionAttenuation reflectionAttenuation, int maxReflectionCount)
        {
            this.size = size.Clone();
            this.distanceAttenuation = distanceAttenuation;
            this.reflectionAttenuation = reflectionAttenuation;
            this.maxReflectionCount = maxReflectionCount;
        }

        public Vector<double> Size => size;
        public DistanceAttenuation DistanceAttenuation => distanceAttenuation;
        public ReflectionAttenuation ReflectionAttenuation => reflectionAttenuation;
        public int MaxReflectionCount => maxReflectionCount;
    }
}
