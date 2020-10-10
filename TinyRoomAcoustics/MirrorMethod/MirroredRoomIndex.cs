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
    /// Represents the index of a mirrored room generated in the mirror method.
    /// Since the shape of rooms is cuboid, mirrored rooms are arranged in a grid pattern.
    /// A room can be therefore identified by the three integers which represents the XYZ position of the grid.
    /// The index of the original room is (0, 0, 0).
    /// The index of the mirrored room next to the right side of the original room is (1, 0, 0), for instance.
    /// </summary>
    public sealed class MirroredRoomIndex
    {
        private readonly int x;
        private readonly int y;
        private readonly int z;

        /// <summary>
        /// Create a new room index.
        /// </summary>
        /// <param name="x">The X position of the room.</param>
        /// <param name="y">The Y position of the room.</param>
        /// <param name="z">The Z position of the room.</param>
        public MirroredRoomIndex(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }

        /// <summary>
        /// The X position of the room.
        /// </summary>
        public int X => x;

        /// <summary>
        /// The Y position of the room.
        /// </summary>
        public int Y => y;

        /// <summary>
        /// The Z position of the room.
        /// </summary>
        public int Z => z;
    }
}
