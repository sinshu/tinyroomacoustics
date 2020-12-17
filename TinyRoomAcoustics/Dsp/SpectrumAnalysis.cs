using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;

namespace TinyRoomAcoustics.Dsp
{
    /// <summary>
    /// This module provides spectrum analysis methods.
    /// </summary>
    public static class SpectrumAnalysis
    {
        /// <summary>
        /// Estimate per-frequency delays between the two DFTs.
        /// </summary>
        /// <param name="x">The DFT as the base.</param>
        /// <param name="y">The DFT to be compared.</param>
        /// <returns>
        /// The delays in samples.
        /// Since the components higher than the Nyquist frequency are discarded,
        /// the length of the returned array is x.Length / 2 + 1.
        /// If y delays compared to x at a frequency, the result is a positive value.
        /// If y precedes, the result is a negative value.
        /// </returns>
        public static double[] EstimatePerFrequencyDelays(Complex[] x, Complex[] y)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (x.Length == 0 || x.Length % 2 != 0)
            {
                throw new ArgumentException(nameof(x), "The length of the DFT must be non-zero and even.");
            }
            if (y.Length == 0 || y.Length % 2 != 0)
            {
                throw new ArgumentException(nameof(y), "The length of the DFT must be non-zero and even.");
            }

            if (x.Length != y.Length)
            {
                throw new ArgumentException("The lengths of the DFTs must be the same.");
            }

            var frameLength = x.Length;

            var delays = new double[frameLength / 2 + 1];

            for (var w = 1; w < delays.Length; w++)
            {
                var waveLength = (double)frameLength / w;

                var dp = x[w].Phase - y[w].Phase;
                dp = Mod(dp + Math.PI, 2 * Math.PI) - Math.PI;

                delays[w] = dp / (2 * Math.PI) * waveLength;
            }

            return delays;
        }

        private static double Mod(double a, double b)
        {
            var result = a % b;
            if (result < 0)
            {
                return result + b;
            }
            else
            {
                return result;
            }
        }
    }
}
