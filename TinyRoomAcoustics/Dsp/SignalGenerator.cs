using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TinyRoomAcoustics.Dsp
{
    public static class SignalGenerator
    {
        public static double[] Tsp(int length)
        {
            double n = length;
            double j = n / 2;

            var fft = new Complex[length];

            for (var k = 0; k < length / 2 + 1; k++)
            {
                fft[k] = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * j * ((k / n) * (k / n)));
            }

            for (var k = 1; k < length / 2; k++)
            {
                fft[length - k] = fft[k].Conjugate();
            }

            Fourier.Inverse(fft, FourierOptions.AsymmetricScaling);

            var result = fft.Select(x => x.Real).ToArray();
            var max = result.Select(x => Math.Abs(x)).Max();
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = 0.9 * result[i] / max;
            }

            var shift = (int)((n - j) / 2);
            var result2 = new double[result.Length];
            for (var i = 0; i < result.Length; i++)
            {
                var i2 = (i + shift) % result.Length;
                result2[i2] = result[i];
            }

            return result2;
        }
    }
}
