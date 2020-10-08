﻿using System;
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
        public static Vector<Complex>[] FromImpulseResponse(double[][] impulseResponses, int dftLength)
        {
            if (impulseResponses == null)
            {
                throw new ArgumentNullException(nameof(impulseResponses));
            }
            if (impulseResponses.Length == 0)
            {
                throw new ArgumentException(nameof(impulseResponses), "The number of impulse responses must be non-zero.");
            }
            if (impulseResponses.Any(fir => fir == null))
            {
                throw new ArgumentException(nameof(impulseResponses), "All the impulse response data must be non-null.");
            }
            if (impulseResponses.Any(fir => fir.Length == 0))
            {
                throw new ArgumentException(nameof(impulseResponses), "The length of all the impulse responses must be non-zero.");
            }

            if (dftLength <= 0 || dftLength % 2 != 0)
            {
                throw new ArgumentException(nameof(dftLength), "The DFT length must be positive and even.");
            }

            var channelCount = impulseResponses.Length;

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
                    var ir = impulseResponses[ch];
                    var length = Math.Min(ir.Length, dftLength);
                    for (var t = 0; t < length; t++)
                    {
                        dft[t] = ir[t];
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
