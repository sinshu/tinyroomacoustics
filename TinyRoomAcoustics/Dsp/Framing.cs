using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TinyRoomAcoustics.Dsp
{
    public static class Framing
    {
        public static double[] GetFrame(double[] source, double[] window, int position)
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

            if (sourceEnd - sourceStart > 0)
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

        public static Complex[] GetFrameComplex(double[] source, double[] window, int position)
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

            if (sourceEnd - sourceStart > 0)
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
    }
}
