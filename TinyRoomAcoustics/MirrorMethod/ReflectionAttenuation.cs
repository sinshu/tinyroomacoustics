using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TinyRoomAcoustics.MirrorMethod
{
    /// <summary>
    /// Represents a reflection attenuation characteristics as a function of frequency.
    /// This function is applied for each reflection.
    /// </summary>
    /// <param name="frequency">The frequency of the sound ray in Hertz (Hz).</param>
    /// <returns>The attenuation coefficient.</returns>
    public delegate double ReflectionAttenuation(double frequency);
}
