using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TinyRoomAcoustics.MirrorMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyRoomAcousticsTest
{
    [TestClass]
    public class MirrorMethodTest
    {
        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 7)]
        [DataRow(2, 25)]
        [DataRow(3, 63)]
        [DataRow(4, 129)]
        [DataRow(5, 231)]
        [DataRow(6, 377)]
        public void GenerateMirroredRoomIndices_CheckIndexCount(int maxReflectionCount, int expectedIndexCount)
        {
            var actualIndexCount = MirrorMethod.GenerateMirroredRoomIndices(maxReflectionCount).Count();
            Assert.AreEqual(expectedIndexCount, actualIndexCount);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void GenerateMirroredRoomIndices_CheckIndexSum(int maxReflectionCount)
        {
            foreach (var index in MirrorMethod.GenerateMirroredRoomIndices(maxReflectionCount))
            {
                var sum = Math.Abs(index.X) + Math.Abs(index.Y) + Math.Abs(index.Z);
                Assert.IsTrue(sum <= maxReflectionCount);
            }
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void GenerateMirroredRoomIndices_CheckIndexUniqueness(int maxReflectionCount)
        {
            var indices = MirrorMethod.GenerateMirroredRoomIndices(maxReflectionCount);
            var ids = indices.Select(index => index.ToString()).ToArray();
            var distincted = ids.Distinct().ToArray();
            Assert.AreEqual(ids.Length, distincted.Length);
        }

        [TestMethod]
        public void GetMirroredPosition_Case1()
        {
            var roomSize = DenseVector.OfArray(new double[] { 3.0, 4.0, 2.0 });
            var position = DenseVector.OfArray(new double[] { 0.5, 1.0, 1.5 });
            var index = new MirroredRoomIndex(0, 0, 0);
            var mirrored = MirrorMethod.GetMirroredPosition(roomSize, position, index);
            Assert.AreEqual(position[0], mirrored[0], 1.0E-9);
            Assert.AreEqual(position[1], mirrored[1], 1.0E-9);
            Assert.AreEqual(position[2], mirrored[2], 1.0E-9);
        }

        [TestMethod]
        public void GetMirroredPosition_Case2()
        {
            var roomSize = DenseVector.OfArray(new double[] { 3.0, 4.0, 2.0 });
            var position = DenseVector.OfArray(new double[] { 0.5, 1.0, 1.5 });
            var index = new MirroredRoomIndex(-1, -1, -1);
            var mirrored = MirrorMethod.GetMirroredPosition(roomSize, position, index);
            Assert.AreEqual(-position[0], mirrored[0], 1.0E-9);
            Assert.AreEqual(-position[1], mirrored[1], 1.0E-9);
            Assert.AreEqual(-position[2], mirrored[2], 1.0E-9);
        }

        [TestMethod]
        public void GetMirroredPosition_Case3()
        {
            var roomSize = DenseVector.OfArray(new double[] { 3.0, 4.0, 2.0 });
            var position = DenseVector.OfArray(new double[] { 0.5, 1.0, 1.5 });
            var index = new MirroredRoomIndex(5, -2, -3);
            var mirrored = MirrorMethod.GetMirroredPosition(roomSize, position, index);
            Assert.AreEqual(15.0 + 2.5, mirrored[0], 1.0E-9);
            Assert.AreEqual(-8.0 + 1.0, mirrored[1], 1.0E-9);
            Assert.AreEqual(-6.0 + 0.5, mirrored[2], 1.0E-9);
        }

        [TestMethod]
        public void GetMirroredPosition_Case4()
        {
            var roomSize = DenseVector.OfArray(new double[] { 3.0, 4.0, 2.0 });
            var position = DenseVector.OfArray(new double[] { 0.5, 1.0, 1.5 });
            var index = new MirroredRoomIndex(-4, 3, 8);
            var mirrored = MirrorMethod.GetMirroredPosition(roomSize, position, index);
            Assert.AreEqual(-12.0 + 0.5, mirrored[0], 1.0E-9);
            Assert.AreEqual(12.0 + 3.0, mirrored[1], 1.0E-9);
            Assert.AreEqual(16.0 + 1.5, mirrored[2], 1.0E-9);
        }

        [DataTestMethod]
        [DataRow(16)]
        [DataRow(64)]
        [DataRow(256)]
        public void GenerateDelayFilter_CheckTimeDomainSignal(int dftLength)
        {
            for (var delaySampleCount = 0; delaySampleCount < dftLength; delaySampleCount++)
            {
                var filter = MirrorMethod.GenerateDelayFilter(dftLength, delaySampleCount);

                var timeDomainSignal = new Complex[dftLength];
                timeDomainSignal[0] = filter[0];
                for (var w = 1; w < dftLength / 2; w++)
                {
                    timeDomainSignal[w] = filter[w];
                    timeDomainSignal[dftLength - w] = filter[w].Conjugate();
                }
                timeDomainSignal[dftLength / 2] = filter[dftLength / 2];
                Fourier.Inverse(timeDomainSignal, FourierOptions.AsymmetricScaling);

                for (var t = 0; t < dftLength; t++)
                {
                    if (t == delaySampleCount)
                    {
                        Assert.AreEqual(1.0, timeDomainSignal[t].Real, 1.0E-6);
                        Assert.AreEqual(0.0, timeDomainSignal[t].Imaginary, 1.0E-6);
                    }
                    else
                    {
                        Assert.AreEqual(0.0, timeDomainSignal[t].Real, 1.0E-6);
                        Assert.AreEqual(0.0, timeDomainSignal[t].Imaginary, 1.0E-6);
                    }
                }
            }
        }

        [TestMethod]
        public void GenerateRays_CheckRayProperties()
        {
            var roomSize = DenseVector.OfArray(new double[] { 2.0, 2.0, 2.0 });
            var distanceAttenuation = new DistanceAttenuation(distance => 0.9);
            var reflectionAttenuation = new ReflectionAttenuation(frequency => 0.7);
            var room = new Room(roomSize, distanceAttenuation, reflectionAttenuation, 1);
            var soundSource = new SoundSource(1.0, 1.0, 1.0);
            var microphone = new Microphone(1.0, 1.0, 1.0);
            var rays = MirrorMethod.GenerateRays(room, soundSource, microphone).ToArray();

            Assert.AreEqual(7, rays.Length);

            foreach (var ray in rays)
            {
                if (ray.Distance < 1.0E-6)
                {
                    Assert.AreEqual(0, ray.ReflectionCount);
                }
                else
                {
                    Assert.AreEqual(2.0, ray.Distance, 1.0E-6);
                    Assert.AreEqual(1, ray.ReflectionCount);
                }
            }
        }

        [TestMethod]
        public void GenerateImpulseResponseFrequencyDomain_CheckTimeDomainSignal()
        {
            var sampleRate = 16000;
            var dftLength = 1024;
            var delaySampleCount = 30;

            var time = (double)delaySampleCount / sampleRate;
            var distance = Room.SoundSpeed * time;

            var roomSize = DenseVector.OfArray(new double[] { distance, distance, distance });
            var distanceAttenuation = new DistanceAttenuation(distance => 0.9);
            var reflectionAttenuation = new ReflectionAttenuation(frequency => 0.7);
            var room = new Room(roomSize, distanceAttenuation, reflectionAttenuation, 1);
            var soundSource = new SoundSource(distance / 2, distance / 2, distance / 2);
            var microphone = new Microphone(distance / 2, distance / 2, distance / 2);

            var response = MirrorMethod.GenerateImpulseResponseFrequencyDomain(room, soundSource, microphone, sampleRate, dftLength);

            var timeDomainSignal = new Complex[dftLength];
            timeDomainSignal[0] = response[0];
            for (var w = 1; w < dftLength / 2; w++)
            {
                timeDomainSignal[w] = response[w];
                timeDomainSignal[dftLength - w] = response[w].Conjugate();
            }
            timeDomainSignal[dftLength / 2] = response[dftLength / 2];
            Fourier.Inverse(timeDomainSignal, FourierOptions.AsymmetricScaling);

            for (var t = 0; t < dftLength; t++)
            {
                if (t == 0)
                {
                    Assert.AreEqual(0.9, timeDomainSignal[t].Real, 1.0E-6);
                    Assert.AreEqual(0.0, timeDomainSignal[t].Imaginary, 1.0E-6);
                }
                else if (t == delaySampleCount)
                {
                    Assert.AreEqual(6 * 0.9 * 0.7, timeDomainSignal[t].Real, 1.0E-6);
                    Assert.AreEqual(0.0, timeDomainSignal[t].Imaginary, 1.0E-6);
                }
            }
        }

        [TestMethod]
        public void GenerateImpulseResponseFrequencyDomain_CheckReflectionAttenuation()
        {
            var sampleRate = 16000;
            var dftLength = 1024;
            var delaySampleCount = 30;

            var time = (double)delaySampleCount / sampleRate;
            var distance = Room.SoundSpeed * time;

            var cutOff = 4000;

            var roomSize = DenseVector.OfArray(new double[] { distance, distance, distance });
            var distanceAttenuation = new DistanceAttenuation(distance => distance < 1.0E-3 ? 0.0 : 0.9);
            var reflectionAttenuation = new ReflectionAttenuation(frequency => frequency < cutOff ? 0.7 : 0.0);
            var room = new Room(roomSize, distanceAttenuation, reflectionAttenuation, 1);
            var soundSource = new SoundSource(distance / 2, distance / 2, distance / 2);
            var microphone = new Microphone(distance / 2, distance / 2, distance / 2);

            var response = MirrorMethod.GenerateImpulseResponseFrequencyDomain(room, soundSource, microphone, sampleRate, dftLength);

            var validCount = 0;
            for (var w = 0; w < response.Length; w++)
            {
                var frequency = (double)w / dftLength * sampleRate;
                if (Math.Abs(frequency - cutOff) > 1)
                {
                    if (frequency < cutOff)
                    {
                        Assert.IsTrue(response[w].Magnitude > 1.0E-3);
                    }
                    else
                    {
                        Assert.IsTrue(response[w].Magnitude < 1.0E-3);
                    }
                    validCount++;
                }
            }
            Assert.IsTrue(validCount >= response.Length - 3);
        }

        [TestMethod]
        public void GenerateImpulseResponseTimeDomain_CheckSignal()
        {
            var sampleRate = 16000;
            var dftLength = 1024;
            var delaySampleCount = 30;

            var time = (double)delaySampleCount / sampleRate;
            var distance = Room.SoundSpeed * time;

            var roomSize = DenseVector.OfArray(new double[] { distance, distance, distance });
            var distanceAttenuation = new DistanceAttenuation(distance => 0.9);
            var reflectionAttenuation = new ReflectionAttenuation(frequency => 0.7);
            var room = new Room(roomSize, distanceAttenuation, reflectionAttenuation, 1);
            var soundSource = new SoundSource(distance / 2, distance / 2, distance / 2);
            var microphone = new Microphone(distance / 2, distance / 2, distance / 2);

            var response = MirrorMethod.GenerateImpulseResponseTimeDomain(room, soundSource, microphone, sampleRate, dftLength);

            Assert.AreEqual(dftLength / 2, response.Length);

            for (var t = 0; t < response.Length; t++)
            {
                if (t == 0)
                {
                    Assert.AreEqual(0.9, response[t], 1.0E-6);
                }
                else if (t == delaySampleCount)
                {
                    Assert.AreEqual(6 * 0.9 * 0.7, response[t], 1.0E-6);
                }
            }
        }
    }
}
