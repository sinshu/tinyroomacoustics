using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics
{
    /// <summary>
    /// Represents a sound source.
    /// </summary>
    public sealed class SoundSource
    {
        private readonly Vector<double> position;

        /// <summary>
        /// Create a new sound source.
        /// </summary>
        /// <param name="x">The X position of the sound source.</param>
        /// <param name="y">The Y position of the sound source.</param>
        /// <param name="z">The Z position of the sound source.</param>
        public SoundSource(double x, double y, double z)
        {
            var array = new double[] { x, y, z };
            position = DenseVector.OfArray(array);
        }

        /// <summary>
        /// Create a new sound source.
        /// </summary>
        /// <param name="position">The position of the sound source.</param>
        public SoundSource(Vector<double> position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }
            if (position.Count != 3)
            {
                throw new ArgumentException(nameof(position), "The length of the position vector must be 3.");
            }

            this.position = position.Clone();
        }

        /// <summary>
        /// The position of the sound source.
        /// </summary>
        public Vector<double> Position => position;
    }
}
