using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics.SourceSeparation
{
    public class GeneralPerFrequencyDoaEstimator2D
    {
        private Microphone[] microphones;
        private int sampleRate;
        private int frameLength;

        private Tuple<int, int>[] pairs;
        private Matrix<double> delayToPosition;

        public GeneralPerFrequencyDoaEstimator2D(IReadOnlyList<Microphone> microphones, int sampleRate, int frameLength)
        {
            this.microphones = microphones.ToArray();
            this.sampleRate = sampleRate;
            this.frameLength = frameLength;

            InitializeMatrix();
        }

        private void InitializeMatrix()
        {
            pairs = new Tuple<int, int>[microphones.Length * (microphones.Length - 1) / 2];

            var count = 0;
            for (var i = 0; i < microphones.Length - 1; i++)
            {
                for (var j = i + 1; j < microphones.Length; j++)
                {
                    pairs[count] = Tuple.Create(i, j);
                    count++;
                }
            }

            Matrix<double> delay = new DenseMatrix(360, pairs.Length);
            Matrix<double> position = new DenseMatrix(360, 2);

            for (var deg = 0; deg < 360; deg++)
            {
                var doa = Math.PI * deg / 180;

                Vector<double> dv = DenseVector.OfArray(new double[]
                {
                    Math.Cos(doa),
                    Math.Sin(doa),
                    0
                });

                for (var i = 0; i < pairs.Length; i++)
                {
                    var pair = pairs[i];
                    var distance = (microphones[pair.Item1].Position - microphones[pair.Item2].Position) * dv;
                    var time = distance / AcousticConstants.SoundSpeed;
                    delay[deg, i] = time;
                    position[deg, 0] = dv[0];
                    position[deg, 1] = dv[1];
                }
            }

            delayToPosition = (delay.PseudoInverse() * position).Transpose();
        }

        public double[] Estimate(Complex[][] dfts)
        {
            var delays = pairs.Select(pair => SourceSeparation.EstimatePerFrequencyDelays(dfts[pair.Item1], dfts[pair.Item2])).ToArray();

            var doa = new double[frameLength / 2 + 1];

            for (var w = 1; w < doa.Length; w++)
            {
                Vector<double> delay = new DenseVector(pairs.Length);

                for (var i = 0; i < pairs.Length; i++)
                {
                    delay[i] = delays[i][w];
                    Vector<double> position = delayToPosition * delay;
                    doa[w] = Math.Atan2(position[1], position[0]);
                }
            }

            return doa;
        }
    }
}
