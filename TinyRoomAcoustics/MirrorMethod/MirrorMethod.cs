using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics.MirrorMethod
{
    public static class MirrorMethod
    {
        public static IEnumerable<MirroredRoomIndex> GenerateMirroredRoomIndices(int maxReflectionCount)
        {
            if (maxReflectionCount < 0)
            {
                throw new ArgumentOutOfRangeException("The max reflection count must be non-negative.");
            }

            return GenerateMirroredRoomIndicesCore(maxReflectionCount);
        }

        private static IEnumerable<MirroredRoomIndex> GenerateMirroredRoomIndicesCore(int maxReflectionCount)
        {
            if (maxReflectionCount < 0)
            {
                throw new ArgumentOutOfRangeException("The max reflection count must be non-negative.");
            }

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

        public static Vector<double> GetMirroredPosition(Vector<double> roomSize, Vector<double> position, MirroredRoomIndex index)
        {
            if (roomSize == null)
            {
                throw new ArgumentNullException(nameof(roomSize));
            }
            if (roomSize.Count != 3)
            {
                throw new ArgumentException("The length of the room size vector must be 3.");
            }
            if (roomSize.Any(value => value <= 0))
            {
                throw new ArgumentException("All the values of the room size vector must be positive.");
            }

            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }
            if (position.Count != 3)
            {
                throw new ArgumentException("The length of the position vector must be 3.");
            }
            for (var i = 0; i < 3; i++)
            {
                if (position[i] <= 0 || roomSize[i] <= position[i])
                {
                    throw new ArgumentException("The position must be inside the room.");
                }
            }

            var offsetX = index.X * roomSize[0];
            var offsetY = index.Y * roomSize[1];
            var offsetZ = index.Z * roomSize[2];

            var mirroredX = (index.X & 1) == 0 ? position[0] : roomSize[0] - position[0];
            var mirroredY = (index.Y & 1) == 0 ? position[1] : roomSize[1] - position[1];
            var mirroredZ = (index.Z & 1) == 0 ? position[2] : roomSize[2] - position[2];

            var array = new double[]
            {
                offsetX + mirroredX,
                offsetY + mirroredY,
                offsetZ + mirroredZ
            };

            return DenseVector.OfArray(array);
        }

        public static Complex[] GenerateDelayFilter(int dftLength, double delaySampleCount)
        {
            if (dftLength <= 0 || dftLength % 2 != 0)
            {
                throw new ArgumentException("The DFT length must be positive and even.");
            }

            var filter = new Complex[dftLength / 2 + 1];
            for (var i = 0; i < filter.Length; i++)
            {
                var theta = 2 * Math.PI * delaySampleCount / dftLength * i;
                filter[i] = new Complex(Math.Cos(theta), -Math.Sin(theta));
            }
            return filter;
        }

        public static IEnumerable<Ray> GenerateRays(Room room, SoundSource soundSource, Microphone microphone)
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

            return GenerateRaysCore(room, soundSource, microphone);
        }

        private static IEnumerable<Ray> GenerateRaysCore(Room room, SoundSource soundSource, Microphone microphone)
        {
            foreach (var index in GenerateMirroredRoomIndices(room.MaxReflectionCount))
            {
                var mirroredPosition = GetMirroredPosition(room.Size, microphone.Position, index);
                var distance = (mirroredPosition - soundSource.Position).L2Norm();
                var reflectionCount = Math.Abs(index.X) + Math.Abs(index.Y) + Math.Abs(index.Z);
                yield return new Ray(distance, reflectionCount);
            }
        }

        public static Complex[] GenerateImpulseResponseFrequencyDomain(Room room, SoundSource soundSource, Microphone microphone, int sampleRate, int dftLength)
        {
            var response = new Complex[dftLength / 2 + 1];
            foreach (var ray in GenerateRays(room, soundSource, microphone))
            {
                var time = ray.Distance / Room.SoundSpeed;
                var delaySampleCount = sampleRate * time;
                var delayFilter = GenerateDelayFilter(dftLength, delaySampleCount);
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

        public static double[] GenerateImpulseResponseTimeDomain(Room room, SoundSource soundSource, Microphone microphone, int sampleRate, int dftLength)
        {
            var response = GenerateImpulseResponseFrequencyDomain(room, soundSource, microphone, sampleRate, dftLength);

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
