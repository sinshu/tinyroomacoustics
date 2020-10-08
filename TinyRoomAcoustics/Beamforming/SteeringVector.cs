using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using TinyRoomAcoustics.Dsp;

namespace TinyRoomAcoustics.Beamforming
{
    public static class SteeringVector
    {
        public static Vector<Complex>[] FromImpulseResponse(double[][] impulseResponse, int dftLength)
        {
            var channelCount = impulseResponse.Length;

            var destination = new Vector<Complex>[dftLength / 2 + 1];
            for (var w = 0; w < destination.Length; w++)
            {
                destination[w] = new DenseVector(channelCount);
            }

            var dft = new Complex[dftLength];

            for (var w = 0; w < destination.Length; w++)
            {
                for (var ch = 0; ch < channelCount; ch++)
                {
                    var fir = impulseResponse[ch];
                    var length = Math.Min(fir.Length, dftLength);
                    for (var t = 0; t < length; t++)
                    {
                        dft[t] = fir[t];
                    }
                    for (var t = length; t < dftLength; t++)
                    {
                        dft[t] = 0;
                    }
                    Fourier.Forward(dft, FourierOptions.AsymmetricScaling);

                    destination[w][ch] = dft[w];
                }
            }

            return destination;
        }
    }
}
