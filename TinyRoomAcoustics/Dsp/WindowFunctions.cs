using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TinyRoomAcoustics.Dsp
{
    /// <summary>
    /// This module provides window functions for signal processing.
    /// </summary>
    public static class WindowFunctions
    {
        /// <summary>
        /// Create a Hann window.
        /// </summary>
        /// <param name="length">The length of the window.</param>
        /// <returns>The Hann window.</returns>
        public static double[] Hann(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException(nameof(length), "The window length must be greater than zero.");
            }

            var window = new double[length];
            for (var t = 0; t < length; t++)
            {
                var x = 2 * Math.PI * t / length;
                window[t] = (1 - Math.Cos(x)) / 2;
            }
            return window;
        }

        /// <summary>
        /// Create a square-root Hann window.
        /// </summary>
        /// <param name="length">The length of the window.</param>
        /// <returns>The square-root Hann window.</returns>
        public static double[] SqrtHann(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException(nameof(length), "The window length must be greater than zero.");
            }

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
