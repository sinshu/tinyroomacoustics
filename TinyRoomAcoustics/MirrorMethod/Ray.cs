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
    /// Represents a sound ray.
    /// </summary>
    public sealed class Ray
    {
        private readonly double distance;
        private readonly int reflectionCount;

        /// <summary>
        /// Create a new sound ray.
        /// </summary>
        /// <param name="distance">The distance the sound ray traveled in meters.</param>
        /// <param name="reflectionCount">The number of times the sound ray reflected.</param>
        public Ray(double distance, int reflectionCount)
        {
            this.distance = distance;
            this.reflectionCount = reflectionCount;
        }

        /// <summary>
        /// The distance the sound ray traveled in meters.
        /// </summary>
        public double Distance => distance;

        /// <summary>
        /// The number of times the sound ray reflected.
        /// </summary>
        public int ReflectionCount => reflectionCount;
    }
}
