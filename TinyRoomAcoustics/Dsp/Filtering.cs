using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TinyRoomAcoustics.Dsp
{
    /// <summary>
    /// This module provides common filtering operations.
    /// </summary>
    public static class Filtering
    {
        /// <summary>
        /// Convolve the FIR to the source signal.
        /// </summary>
        /// <param name="source">The source signal to be convolved.</param>
        /// <param name="fir">The FIR to convolve.</param>
        /// <returns>The convolved signal.</returns>
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

        /// <summary>
        /// Create a delay filter in the frequency domain.
        /// </summary>
        /// <param name="dftLength">The length of the DFT.</param>
        /// <param name="delaySampleCount">The desired amount of delay in samples.
        /// Both integer and non-integer values are acceptable.</param>
        /// <returns>The delay filter in the frequency domain.
        /// Since the components higher than Nyquist frequency are discarded,
        /// the length of the returned array is dftLength / 2 + 1.</returns>
        public static Complex[] CreateFrequencyDomainDelayFilter(int dftLength, double delaySampleCount)
        {
            if (dftLength <= 0 || dftLength % 2 != 0)
            {
                throw new ArgumentException(nameof(dftLength), "The length of the DFT must be positive and even.");
            }

            var filter = new Complex[dftLength / 2 + 1];
            for (var i = 0; i < filter.Length; i++)
            {
                var theta = 2 * Math.PI * delaySampleCount / dftLength * i;
                filter[i] = new Complex(Math.Cos(theta), -Math.Sin(theta));
            }
            return filter;
        }
    }
}
