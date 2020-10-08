using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics
{
    public sealed class SoundSource
    {
        private readonly Vector<double> position;

        public SoundSource(double x, double y, double z)
        {
            var array = new double[] { x, y, z };
            position = DenseVector.OfArray(array);
        }

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

        public Vector<double> Position => position;
    }
}
