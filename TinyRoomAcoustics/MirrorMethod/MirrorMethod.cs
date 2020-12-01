using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TinyRoomAcoustics.Dsp;

namespace TinyRoomAcoustics.MirrorMethod
{
    /// <summary>
    /// The room acoustics simulation module based on the mirror method.
    /// </summary>
    public static class MirrorMethod
    {
        /// <summary>
        /// Generate mirrored room indices which are necessary to simulate the given number of times of sound reflection.
        /// </summary>
        /// <param name="maxReflectionCount">The max number of times of sound reflection.</param>
        /// <returns>All the mirrored room indices which are necessary to simulate the given number of times of sound reflection.</returns>
        /// <seealso cref="MirroredRoomIndex"/>
        public static IEnumerable<MirroredRoomIndex> GenerateMirroredRoomIndices(int maxReflectionCount)
        {
            if (maxReflectionCount < 0)
            {
                throw new ArgumentException(nameof(maxReflectionCount), "The number of times of reflection must be greater than or equal to zero.");
            }

            return GenerateMirroredRoomIndicesCore(maxReflectionCount);
        }

        private static IEnumerable<MirroredRoomIndex> GenerateMirroredRoomIndicesCore(int maxReflectionCount)
        {
            for (var reflectionCount = 0; reflectionCount <= maxReflectionCount; reflectionCount++)
            {
                var maxX = reflectionCount;
                var minX = -maxX;
                for (var x = minX; x <= maxX; x++)
                {
                    var maxY = maxX - Math.Abs(x);
                    var minY = -maxY;
                    for (var y = minY; y <= maxY; y++)
                    {
                        var z1 = maxY - Math.Abs(y);
                        var z2 = -z1;
                        if (z1 == z2)
                        {
                            yield return new MirroredRoomIndex(x, y, z1);
                        }
                        else
                        {
                            yield return new MirroredRoomIndex(x, y, z1);
                            yield return new MirroredRoomIndex(x, y, z2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the position of the imaginary microphone in the mirrored room.
        /// </summary>
        /// <param name="room">The room to be simulated.</param>
        /// <param name="microphone">The microphone in the original room.</param>
        /// <param name="index">The index of the mirrored room.</param>
        /// <returns>The position of the imaginary microphone in the mirrored room.</returns>
        /// <seealso cref="MirroredRoomIndex"/>
        public static Vector<double> GetMirroredPosition(Room room, Microphone microphone, MirroredRoomIndex index)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            if (microphone == null)
            {
                throw new ArgumentNullException(nameof(microphone));
            }
            if (index == null)
            {
                throw new ArgumentNullException(nameof(index));
            }

            for (var i = 0; i < 3; i++)
            {
                if (microphone.Position[i] <= 0 || room.Size[i] <= microphone.Position[i])
                {
                    throw new ArgumentException("The microphone must be inside the room.");
                }
            }

            var offsetX = index.X * room.Size[0];
            var offsetY = index.Y * room.Size[1];
            var offsetZ = index.Z * room.Size[2];

            var mirroredX = (index.X & 1) == 0 ? microphone.Position[0] : room.Size[0] - microphone.Position[0];
            var mirroredY = (index.Y & 1) == 0 ? microphone.Position[1] : room.Size[1] - microphone.Position[1];
            var mirroredZ = (index.Z & 1) == 0 ? microphone.Position[2] : room.Size[2] - microphone.Position[2];

            var array = new double[]
            {
                offsetX + mirroredX,
                offsetY + mirroredY,
                offsetZ + mirroredZ
            };

            return DenseVector.OfArray(array);
        }

        /// <summary>
        /// Generate sound rays.
        /// </summary>
        /// <param name="room">The room to be simulated.</param>
        /// <param name="soundSource">The sound source to be simulated.</param>
        /// <param name="microphone">The microphone to be simulated.</param>
        /// <returns>Simulated sound rays.</returns>
        public static IEnumerable<SoundRay> GenerateSoundRays(Room room, SoundSource soundSource, Microphone microphone)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            if (soundSource == null)
            {
                throw new ArgumentNullException(nameof(soundSource));
            }
            if (microphone == null)
            {
                throw new ArgumentNullException(nameof(microphone));
            }

            return GenerateSoundRaysCore(room, soundSource, microphone);
        }

        private static IEnumerable<SoundRay> GenerateSoundRaysCore(Room room, SoundSource soundSource, Microphone microphone)
        {
            foreach (var index in GenerateMirroredRoomIndices(room.MaxReflectionCount))
            {
                var mirroredPosition = GetMirroredPosition(room, microphone, index);
                var distance = (mirroredPosition - soundSource.Position).L2Norm();
                var reflectionCount = Math.Abs(index.X) + Math.Abs(index.Y) + Math.Abs(index.Z);
                yield return new SoundRay(distance, reflectionCount);
            }
        }

        /// <summary>
        /// Generate the room impulse response in the frequency domain.
        /// </summary>
        /// <param name="room">The room to be simulated.</param>
        /// <param name="soundSource">The sound source to be simulated.</param>
        /// <param name="microphone">The microphone to be simulated.</param>
        /// <param name="sampleRate">The sampling frequency of the impulse response.</param>
        /// <param name="dftLength">The length of the DFT.</param>
        /// <returns>
        /// The simulated impulse response in the frequency domain.
        /// Since the components higher than the Nyquist frequency are discarded,
        /// the length of the returned array is dftLength / 2 + 1.
        /// </returns>
        public static Complex[] GenerateFrequencyDomainImpulseResponse(Room room, SoundSource soundSource, Microphone microphone, int sampleRate, int dftLength)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            if (soundSource == null)
            {
                throw new ArgumentNullException(nameof(soundSource));
            }
            if (microphone == null)
            {
                throw new ArgumentNullException(nameof(microphone));
            }

            if (sampleRate <= 0)
            {
                throw new ArgumentException(nameof(sampleRate), "The sampling frequency must be greater than zero.");
            }
            if (dftLength <= 0 || dftLength % 2 != 0)
            {
                throw new ArgumentException(nameof(dftLength), "The length of the DFT must be positive and even.");
            }

            var response = new Complex[dftLength / 2 + 1];
            foreach (var ray in GenerateSoundRays(room, soundSource, microphone))
            {
                var time = ray.Distance / AcousticConstants.SoundSpeed;
                var delaySampleCount = sampleRate * time;
                var delayFilter = Filtering.CreateFrequencyDomainDelayFilter(dftLength, delaySampleCount);
                var distanceAttenuation = room.DistanceAttenuation(ray.Distance);
                for (var w = 0; w < response.Length; w++)
                {
                    var frequency = (double)w / dftLength * sampleRate;
                    var reflectionAttenuation = Math.Pow(room.ReflectionAttenuation(frequency), ray.ReflectionCount);
                    response[w] += distanceAttenuation * reflectionAttenuation * delayFilter[w];
                }
            }
            return response;
        }

        /// <summary>
        /// Generate the room impulse response.
        /// </summary>
        /// <param name="room">The room to be simulated.</param>
        /// <param name="soundSource">The sound source to be simulated.</param>
        /// <param name="microphone">The microphone to be simulated.</param>
        /// <param name="sampleRate">The sampling frequency of the impulse response.</param>
        /// <param name="dftLength">The length of the DFT.</param>
        /// <returns>
        /// The simulated impulse response.
        /// Since the acausal components are discarded,
        /// the length of the returned array is dftLength / 2.
        /// </returns>
        public static double[] GenerateImpulseResponse(Room room, SoundSource soundSource, Microphone microphone, int sampleRate, int dftLength)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            if (soundSource == null)
            {
                throw new ArgumentNullException(nameof(soundSource));
            }
            if (microphone == null)
            {
                throw new ArgumentNullException(nameof(microphone));
            }

            if (sampleRate <= 0)
            {
                throw new ArgumentException(nameof(sampleRate), "The sampling frequency must be greater than zero.");
            }
            if (dftLength <= 0 || dftLength % 2 != 0)
            {
                throw new ArgumentException(nameof(dftLength), "The length of the DFT must be positive and even.");
            }

            var response = GenerateFrequencyDomainImpulseResponse(room, soundSource, microphone, sampleRate, dftLength);

            var timeDomainSignal = new Complex[dftLength];
            timeDomainSignal[0] = response[0];
            for (var w = 1; w < dftLength / 2; w++)
            {
                timeDomainSignal[w] = response[w];
                timeDomainSignal[dftLength - w] = response[w].Conjugate();
            }
            timeDomainSignal[dftLength / 2] = response[dftLength / 2];
            Fourier.Inverse(timeDomainSignal, FourierOptions.AsymmetricScaling);

            return timeDomainSignal.Select(value => value.Real).Take(dftLength / 2).ToArray();
        }
    }
}
