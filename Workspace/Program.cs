using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NAudio.Wave;
using TinyRoomAcoustics;
using TinyRoomAcoustics.Beamforming;
using TinyRoomAcoustics.Dsp;
using TinyRoomAcoustics.MirrorMethod;
using TinyRoomAcoustics.SourceSeparation;

namespace Workspace
{
    static class Program
    {
        static void Main(string[] args)
        {
            var test = SignalGenerator.Tsp(65536);
            var format = new WaveFormat(48000, 1);
            using (var writer = new WaveFileWriter("test.wav", format))
            {
                foreach (var value in test)
                {
                    writer.WriteSample((float)value);
                }
            }

            var test2 = test.Concat(new double[test.Length]).ToArray();

            var inv = test.Reverse().ToArray();
            var imp = Filtering.Convolve(test2, inv);
            var max = imp.Select(x => Math.Abs(x)).Max();
            imp = imp.Select(x => 0.95 * x / max).ToArray();
            using (var writer = new WaveFileWriter("test2.wav", format))
            {
                foreach (var value in imp)
                {
                    writer.WriteSample((float)value);
                }
            }

            var sum = imp.Select(x => Math.Abs(x)).Sum();
            Console.WriteLine(sum);

            var pos = 0;
            for (var i = 0; i < imp.Length; i++)
            {
                if (imp[i] > 0.9)
                {
                    pos = i;
                    Console.WriteLine("PEAK!");
                }
            }

            for (var i = pos - 10; i <= pos + 10; i++)
            {
                Console.WriteLine(imp[i]);
            }
        }
    }
}
