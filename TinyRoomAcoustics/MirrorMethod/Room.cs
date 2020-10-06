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
            if (size == null)
            {
                throw new ArgumentNullException(nameof(size));
            }
            if (size.Count != 3)
            {
                throw new ArgumentException(nameof(size), "The length of the size vector must be 3.");
            }
            if (size.Any(value => value <= 0))
            {
                throw new ArgumentException(nameof(size), "All the values of the size vector must be positive.");
            }

            if (distanceAttenuation == null)
            {
                throw new ArgumentNullException(nameof(distanceAttenuation));
            }
            if (reflectionAttenuation == null)
            {
                throw new ArgumentNullException(nameof(reflectionAttenuation));
            }

            if (maxReflectionCount < 0)
            {
                throw new ArgumentException(nameof(maxReflectionCount), "The max reflection count must be non-negative.");
            }

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
