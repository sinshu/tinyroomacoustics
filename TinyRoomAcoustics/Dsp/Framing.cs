using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TinyRoomAcoustics.Dsp
{
    /// <summary>
    /// This module provides common framing operations for signal processing..
    /// </summary>
    public static class Framing
    {
        /// <summary>
        /// Extract a short-time frame from the source signal.
        /// </summary>
        /// <param name="source">The source signal.</param>
        /// <param name="window">The window function applied to the frame.</param>
        /// <param name="position">The start position of the frame in the source signal.</param>
        /// <returns>The short-time frame.
        /// The length of the frame is the same as the window.</returns>
        public static double[] GetFrame(this double[] source, double[] window, int position)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var sourceStart = position;
            var sourceEnd = position + window.Length;

            var frameStart = 0;
            var frameEnd = window.Length;

            if (sourceStart < 0)
            {
                var exceedance = -sourceStart;
                sourceStart += exceedance;
                frameStart += exceedance;
            }

            if (sourceEnd > source.Length)
            {
                var exceedance = sourceEnd - source.Length;
                sourceEnd -= exceedance;
                frameEnd -= exceedance;
            }

            var frame = new double[window.Length];

            if (frameEnd - frameStart > 0)
            {
                var st = sourceStart;
                for (var ft = frameStart; ft < frameEnd; ft++)
                {
                    frame[ft] = window[ft] * source[st];
                    st++;
                }
            }

            return frame;
        }

        /// <summary>
        /// Extract a short-time frame as an array of complex number from the source signal.
        /// </summary>
        /// <param name="source">The source signal.</param>
        /// <param name="window">The window function applied to the frame.</param>
        /// <param name="position">The start position of the frame in the source signal.</param>
        /// <returns>The short-time frame converted to an array of complex number.
        /// The length of the frame is the same as the window.</returns>
        public static Complex[] GetFrameComplex(this double[] source, double[] window, int position)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var sourceStart = position;
            var sourceEnd = position + window.Length;

            var frameStart = 0;
            var frameEnd = window.Length;

            if (sourceStart < 0)
            {
                var exceedance = -sourceStart;
                sourceStart += exceedance;
                frameStart += exceedance;
            }

            if (sourceEnd > source.Length)
            {
                var exceedance = sourceEnd - source.Length;
                sourceEnd -= exceedance;
                frameEnd -= exceedance;
            }

            var frame = new Complex[window.Length];

            if (frameEnd - frameStart > 0)
            {
                var st = sourceStart;
                for (var ft = frameStart; ft < frameEnd; ft++)
                {
                    frame[ft] = window[ft] * source[st];
                    st++;
                }
            }

            return frame;
        }

        /// <summary>
        /// Overlap-add the short-time frame to the destination signal.
        /// </summary>
        /// <param name="destination">The destination signal.</param>
        /// <param name="frame">The short-time frame.</param>
        /// <param name="window">The window function applied to the frame.</param>
        /// <param name="position">The start position of the frame in the destination signal.</param>
        public static void OverlapAdd(this double[] destination, double[] frame, double[] window, int position)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (frame.Length != window.Length)
            {
                throw new ArgumentException("The frame and window must be the same length.");
            }

            var destinationStart = position;
            var destinationEnd = position + frame.Length;

            var frameStart = 0;
            var frameEnd = frame.Length;

            if (destinationStart < 0)
            {
                var exceedance = -destinationStart;
                destinationStart += exceedance;
                frameStart += exceedance;
            }

            if (destinationEnd > destination.Length)
            {
                var exceedance = destinationEnd - destination.Length;
                destinationEnd -= exceedance;
                frameEnd -= exceedance;
            }

            if (destinationEnd - destinationStart > 0)
            {
                var ft = frameStart;
                for (var dt = destinationStart; dt < destinationEnd; dt++)
                {
                    destination[dt] += window[ft] * frame[ft];
                    ft++;
                }
            }
        }

        /// <summary>
        /// Overlap-add the short-time frame consisted of complex numbers to the destination signal.
        /// Only real parts of the complex numbers in the frame are used.
        /// </summary>
        /// <param name="destination">The destination signal.</param>
        /// <param name="frame">The short-time frame.</param>
        /// <param name="window">The window function applied to the frame.</param>
        /// <param name="position">The start position of the frame in the destination signal.</param>
        public static void OverlapAdd(this double[] destination, Complex[] frame, double[] window, int position)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (frame.Length != window.Length)
            {
                throw new ArgumentException("The frame and window must be the same length.");
            }

            var destinationStart = position;
            var destinationEnd = position + frame.Length;

            var frameStart = 0;
            var frameEnd = frame.Length;

            if (destinationStart < 0)
            {
                var exceedance = -destinationStart;
                destinationStart += exceedance;
                frameStart += exceedance;
            }

            if (destinationEnd > destination.Length)
            {
                var exceedance = destinationEnd - destination.Length;
                destinationEnd -= exceedance;
                frameEnd -= exceedance;
            }

            if (destinationEnd - destinationStart > 0)
            {
                var ft = frameStart;
                for (var dt = destinationStart; dt < destinationEnd; dt++)
                {
                    destination[dt] += window[ft] * frame[ft].Real;
                    ft++;
                }
            }
        }
    }
}
