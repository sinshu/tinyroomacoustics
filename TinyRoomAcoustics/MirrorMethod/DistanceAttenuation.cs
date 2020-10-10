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
    /// Represents a distance attenuation characteristics as a function of distance.
    /// </summary>
    /// <param name="distance">The distance the sound ray traveled in meters.</param>
    /// <returns>The attenuation coefficient.</returns>
    public delegate double DistanceAttenuation(double distance);
}
