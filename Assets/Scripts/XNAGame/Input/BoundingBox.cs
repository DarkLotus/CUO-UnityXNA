using System;
using Microsoft.Xna.Framework;

namespace ClassicUO.Input
{
    internal class BoundingBox
    {
        private Vector3 vector31;
        private Vector3 vector32;

        public BoundingBox( Vector3 vector31, Vector3 vector32 )
        {
            this.vector31 = vector31;
            this.vector32 = vector32;
        }

        internal ContainmentType Contains( Vector3 vector3 )
        {
            throw new NotImplementedException();
        }
    }
}