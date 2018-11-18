using System;
using Microsoft.Xna.Framework;

namespace ClassicUO.Input
{
    internal class BoundingBox
    {
        private Vector3 v1;
        private Vector3 v2;

        public BoundingBox( Vector3 vector31, Vector3 vector32 )
        {
            this.v1 = vector31;
            this.v2 = vector32;
        }

        internal ContainmentType Contains( Vector3 value )
        {
            return ( ( ( ( this.v1.X <= value.X ) && ( value.X < ( this.v2.X ) ) ) && ( this.v1.Y <= value.Y ) ) && ( value.Y <  this.v2.Y ) ) ? ContainmentType.Contains : ContainmentType.Disjoint;
        }
    }
}