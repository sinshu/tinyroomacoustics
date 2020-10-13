using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using TinyRoomAcoustics.Dsp;

namespace TinyRoomAcoustics.Beamforming
{
    public sealed class DelayAndSum
    {
        private double[] window;
        private int frameShift;

        public DelayAndSum(double[] window, int frameShift)
        {
            this.window = window.ToArray();
            this.frameShift = frameShift;
        }

        public double[] Process(double[][] source, Vector<Complex>[] steering)
        {
            throw new NotImplementedException();
        }
    }
}
