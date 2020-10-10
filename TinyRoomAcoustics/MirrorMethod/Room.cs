using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics.MirrorMethod
{
    /// <summary>
    /// Represents a room to be simulated.
    /// The shape of a room is restricted to cuboid and all the walls share the same property.
    /// </summary>
    public sealed class Room
    {
        private readonly Vector<double> size;
        private readonly DistanceAttenuation distanceAttenuation;
        private readonly ReflectionAttenuation reflectionAttenuation;
        private readonly int maxReflectionCount;

        /// <summary>
        /// Create a room to be simulated.
        /// </summary>
        /// <param name="size">The size of the room as a 3-dimensional vector.</param>
        /// <param name="distanceAttenuation">The distance attenuation characteristics of the room.</param>
        /// <param name="reflectionAttenuation">The reflection attenuation characteristics of the room.</param>
        /// <param name="maxReflectionCount">The number of reflections to be simulated.</param>
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
                throw new ArgumentException(nameof(maxReflectionCount), "The max reflection count must be greater than or equal to zero.");
            }

            this.size = size.Clone();
            this.distanceAttenuation = distanceAttenuation;
            this.reflectionAttenuation = reflectionAttenuation;
            this.maxReflectionCount = maxReflectionCount;
        }

        /// <summary>
        /// The size of the room.
        /// </summary>
        public Vector<double> Size => size;

        /// <summary>
        /// The distance attenuation characteristics of the room.
        /// </summary>
        public DistanceAttenuation DistanceAttenuation => distanceAttenuation;

        /// <summary>
        /// The reflection attenuation characteristics of the room.
        /// </summary>
        public ReflectionAttenuation ReflectionAttenuation => reflectionAttenuation;

        /// <summary>
        /// The number of reflections to be simulated.
        /// </summary>
        public int MaxReflectionCount => maxReflectionCount;
    }
}
