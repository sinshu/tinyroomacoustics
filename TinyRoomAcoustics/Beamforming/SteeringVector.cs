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

        public static Vector<Complex>[] FromNearFieldGeometry(IReadOnlyList<Microphone> microphones, SoundSource soundSource, int sampleRate, int dftLength)
        {
            if (microphones == null)
            {
                throw new ArgumentNullException(nameof(microphones));
            }
            if (microphones.Count == 0)
            {
                throw new ArgumentException(nameof(microphones), "The number of microphones must be non-zero.");
            }
            if (microphones.Any(mic => mic == null))
            {
                throw new ArgumentException(nameof(microphones), "All the microphone object must be non-null.");
            }

            if (soundSource == null)
            {
                throw new ArgumentNullException(nameof(soundSource));
            }

            if (sampleRate <= 0)
            {
                throw new ArgumentException(nameof(sampleRate), "The sample rate must be positive.");
            }

            if (dftLength <= 0 || dftLength % 2 != 0)
            {
                throw new ArgumentException(nameof(dftLength), "The DFT length must be positive and even.");
            }

            var channelCount = microphones.Count;

            var destination = new Vector<Complex>[dftLength / 2 + 1];
            for (var w = 0; w < destination.Length; w++)
            {
                destination[w] = new DenseVector(channelCount);
            }

            for (var ch = 0; ch < channelCount; ch++)
            {
                var distance = (soundSource.Position - microphones[ch].Position).L2Norm();
                var time = distance / AcousticConstants.SoundSpeed;
                var delaySampleCount = sampleRate * time;
                var delayFilter = Filtering.CreateFrequencyDomainDelayFilter(dftLength, delaySampleCount);

                for (var w = 0; w < destination.Length; w++)
                {
                    destination[w][ch] = delayFilter[w];
                }
            }

            return destination;
        }

        public static Vector<Complex>[] FromFarFieldGeometry(IReadOnlyList<Microphone> microphones, double direction, double pitch, int sampleRate, int dftLength)
        {
            if (microphones == null)
            {
                throw new ArgumentNullException(nameof(microphones));
            }
            if (microphones.Count == 0)
            {
                throw new ArgumentException(nameof(microphones), "The number of microphones must be non-zero.");
            }
            if (microphones.Any(mic => mic == null))
            {
                throw new ArgumentException(nameof(microphones), "All the microphone object must be non-null.");
            }

            if (sampleRate <= 0)
            {
                throw new ArgumentException(nameof(sampleRate), "The sample rate must be positive.");
            }

            if (dftLength <= 0 || dftLength % 2 != 0)
            {
                throw new ArgumentException(nameof(dftLength), "The DFT length must be positive and even.");
            }

            var channelCount = microphones.Count;

            var destination = new Vector<Complex>[dftLength / 2 + 1];
            for (var w = 0; w < destination.Length; w++)
            {
                destination[w] = new DenseVector(channelCount);
            }

            var dv = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(3);
            dv[0] = Math.Cos(direction) * Math.Cos(pitch);
            dv[1] = Math.Sin(direction) * Math.Cos(pitch);
            dv[2] = Math.Sin(pitch);

            var distances = new double[channelCount];
            for (var ch = 0; ch < channelCount; ch++)
            {
                distances[ch] = dv * microphones[ch].Position;
            }
            var offset = distances.Min();
            for (var ch = 0; ch < channelCount; ch++)
            {
                distances[ch] -= offset;
            }

            for (var ch = 0; ch < channelCount; ch++)
            {
                var distance = distances[ch];
                var time = distance / AcousticConstants.SoundSpeed;
                var delaySampleCount = sampleRate * time;
                var delayFilter = Filtering.CreateFrequencyDomainDelayFilter(dftLength, delaySampleCount);

                for (var w = 0; w < destination.Length; w++)
                {
                    destination[w][ch] = delayFilter[w];
                }
            }

            return destination;
        }
    }
}
