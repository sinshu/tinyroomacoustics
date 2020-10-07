using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics.Dsp
{
    public static class WindowFunctions
    {
        public static double[] Hann(int length)
        {
            var window = new double[length];
            for (var t = 0; t < length; t++)
            {
                var x = 2 * Math.PI * t / length;
                window[t] = (1 - Math.Cos(x)) / 2;
            }
            return window;
        }

        public static double[] SqrtHann(int length)
        {
            var window = new double[length];
            for (var t = 0; t < length; t++)
            {
                var x = 2 * Math.PI * t / length;
                window[t] = Math.Sqrt((1 - Math.Cos(x)) / 2);
            }
            return window;
        }
    }
}
