using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TinyRoomAcoustics.Dsp
{
    public static class Filtering
    {
        public static double[] Convolve(this double[] source, double[] fir)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (fir == null)
            {
                throw new ArgumentNullException(nameof(fir));
            }

            if (fir.Length == 0)
            {
                throw new ArgumentException(nameof(fir), "The length of the FIR must be non-zero.");
            }

            var dftLength = GetMinPot(2 * fir.Length);
            var frameLength = dftLength / 2;

            var firDft = new Complex[dftLength];
            for (var t = 0; t < fir.Length; t++)
            {
                firDft[t] = fir[t];
            }
            Fourier.Forward(firDft, FourierOptions.AsymmetricScaling);

            var destination = new double[source.Length];

            var frameDft = new Complex[dftLength];

            for (var position = 0; position < source.Length; position += frameLength)
            {
                {
                    var destinationEnd = position + frameLength;
                    var frameEnd = frameLength;

                    var exceedance = destinationEnd - source.Length;
                    if (exceedance > 0)
                    {
                        destinationEnd -= exceedance;
                        frameEnd -= exceedance;
                    }

                    for (var t = 0; t < frameEnd; t++)
                    {
                        frameDft[t] = source[position + t];
                    }
                    for (var t = frameEnd; t < frameDft.Length; t++)
                    {
                        frameDft[t] = 0;
                    }
                    Fourier.Forward(frameDft, FourierOptions.AsymmetricScaling);
                }

                frameDft[0] *= firDft[0];
                for (var w = 1; w < frameLength; w++)
                {
                    frameDft[w] *= firDft[w];
                    frameDft[frameDft.Length - w] = frameDft[w].Conjugate();
                }
                frameDft[frameLength] *= firDft[frameLength];
                Fourier.Inverse(frameDft, FourierOptions.AsymmetricScaling);

                {
                    var destinationEnd = position + dftLength;
                    var frameEnd = dftLength;

                    var exceedance = destinationEnd - source.Length;
                    if (exceedance > 0)
                    {
                        destinationEnd -= exceedance;
                        frameEnd -= exceedance;
                    }

                    for (var t = 0; t < frameEnd; t++)
                    {
                        destination[position + t] += frameDft[t].Real;
                    }
                }
            }

            return destination;
        }

        private static int GetMinPot(int value)
        {
            var pot = 1;
            while (pot < value)
            {
                pot *= 2;
            }
            return pot;
        }
    }
}
