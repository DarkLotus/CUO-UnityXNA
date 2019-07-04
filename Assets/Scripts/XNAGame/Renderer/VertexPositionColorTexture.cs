#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2018 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using static Microsoft.Xna.Framework.Graphics.VertexDeclaration;
#endregion

namespace Microsoft.Xna.Framework.Graphics
{
    public struct VertexElement
    {
        #region Public Properties

        public int Offset
        {
            get;
            set;
        }

        public VertexElementFormat VertexElementFormat
        {
            get;
            set;
        }

        public VertexElementUsage VertexElementUsage
        {
            get;
            set;
        }

        public int UsageIndex
        {
            get;
            set;
        }

        #endregion

        #region Public Constructor

        public VertexElement(
            int offset,
            VertexElementFormat elementFormat,
            VertexElementUsage elementUsage,
            int usageIndex
        ) : this()
        {
            Offset = offset;
            UsageIndex = usageIndex;
            VertexElementFormat = elementFormat;
            VertexElementUsage = elementUsage;
        }

        #endregion

        #region Public Static Operators and Override Methods

        public override int GetHashCode()
        {
            // TODO: Fix hashes
            return 0;
        }

        public override string ToString()
        {
            return (
                "{{Offset:" + Offset.ToString() +
                " Format:" + VertexElementFormat.ToString() +
                " Usage:" + VertexElementUsage.ToString() +
                " UsageIndex: " + UsageIndex.ToString() +
                "}}"
            );
        }

        public override bool Equals( object obj )
        {
            if ( obj == null )
            {
                return false;
            }
            if ( obj.GetType() != base.GetType() )
            {
                return false;
            }
            return ( this == ( (VertexElement)obj ) );
        }

        public static bool operator ==( VertexElement left, VertexElement right )
        {
            return ( ( left.Offset == right.Offset ) &&
                    ( left.UsageIndex == right.UsageIndex ) &&
                    ( left.VertexElementUsage == right.VertexElementUsage ) &&
                    ( left.VertexElementFormat == right.VertexElementFormat ) );
        }

        public static bool operator !=( VertexElement left, VertexElement right )
        {
            return !( left == right );
        }

        #endregion
    }
    public class VertexDeclaration : GraphicsResource
    {
        #region Public Properties

        public int VertexStride
        {
            get;
            private set;
        }

        #endregion

        #region Internal Variables

        internal VertexElement[] elements;

        #endregion

        #region Public Constructors

        public VertexDeclaration(
            params VertexElement[] elements
        ) : this( GetVertexStride( elements ), elements )
        {
        }

        public VertexDeclaration(
            int vertexStride,
            params VertexElement[] elements
        )
        {
            if ( ( elements == null ) || ( elements.Length == 0 ) )
            {
                throw new ArgumentNullException( "elements", "Elements cannot be empty" );
            }

            this.elements = (VertexElement[])elements.Clone();
            VertexStride = vertexStride;
        }

        #endregion

        #region Public Methods

        public VertexElement[] GetVertexElements()
        {
            return (VertexElement[])elements.Clone();
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Returns the VertexDeclaration for Type.
        /// </summary>
        /// <param name="vertexType">A value type which implements the IVertexType interface.</param>
        /// <returns>The VertexDeclaration.</returns>
        /// <remarks>
        /// Prefer to use VertexDeclarationCache when the declaration lookup
        /// can be performed with a templated type.
        /// </remarks>
        internal static VertexDeclaration FromType( Type vertexType )
        {
            if ( vertexType == null )
            {
                throw new ArgumentNullException( "vertexType", "Cannot be null" );
            }

            if ( !vertexType.IsValueType )
            {
                throw new ArgumentException( "vertexType", "Must be value type" );
            }

            IVertexType type = Activator.CreateInstance( vertexType ) as IVertexType;
            if ( type == null )
            {
                throw new ArgumentException( "vertexData does not inherit IVertexType" );
            }

            VertexDeclaration vertexDeclaration = type.VertexDeclaration;
            if ( vertexDeclaration == null )
            {
                throw new ArgumentException( "vertexType's VertexDeclaration cannot be null" );
            }

            return vertexDeclaration;
        }
        public interface IVertexType
        {
            VertexDeclaration VertexDeclaration
            {
                get;
            }
        }
        #endregion

        #region Private Static VertexElement Methods

        private static int GetVertexStride( VertexElement[] elements )
        {
            int max = 0;

            for ( int i = 0; i < elements.Length; i += 1 )
            {
                int start = elements[i].Offset + GetTypeSize( elements[i].VertexElementFormat );
                if ( max < start )
                {
                    max = start;
                }
            }

            return max;
        }

        private static int GetTypeSize( VertexElementFormat elementFormat )
        {
            switch ( elementFormat )
            {
                case VertexElementFormat.Single:
                    return 4;
                case VertexElementFormat.Vector2:
                    return 8;
                case VertexElementFormat.Vector3:
                    return 12;
                case VertexElementFormat.Vector4:
                    return 16;
                case VertexElementFormat.Color:
                    return 4;
                case VertexElementFormat.Byte4:
                    return 4;
                case VertexElementFormat.Short2:
                    return 4;
                case VertexElementFormat.Short4:
                    return 8;
                case VertexElementFormat.NormalizedShort2:
                    return 4;
                case VertexElementFormat.NormalizedShort4:
                    return 8;
                case VertexElementFormat.HalfVector2:
                    return 4;
                case VertexElementFormat.HalfVector4:
                    return 8;
            }
            return 0;
        }

        #endregion
    }
    [Serializable]
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct VertexPositionColorTexture : IVertexType
    {
        #region Private Properties

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        #endregion

        #region Public Variables

        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoordinate;

        #endregion

        #region Public Static Variables

        public static readonly VertexDeclaration VertexDeclaration;

        #endregion

        #region Private Static Constructor

        static VertexPositionColorTexture()
        {
            VertexDeclaration = new VertexDeclaration(
                new VertexElement[]
                {
                    new VertexElement(
                        0,
                        VertexElementFormat.Vector3,
                        VertexElementUsage.Position,
                        0
                    ),
                    new VertexElement(
                        12,
                        VertexElementFormat.Color,
                        VertexElementUsage.Color,
                        0
                    ),
                    new VertexElement(
                        16,
                        VertexElementFormat.Vector2,
                        VertexElementUsage.TextureCoordinate,
                        0
                    )
                }
            );
        }

        #endregion

        #region Public Constructor

        public VertexPositionColorTexture(
            Vector3 position,
            Color color,
            Vector2 textureCoordinate
        )
        {
            Position = position;
            Color = color;
            TextureCoordinate = textureCoordinate;
        }

        #endregion

        #region Public Static Operators and Override Methods

        public override int GetHashCode()
        {
            // TODO: Fix GetHashCode
            return 0;
        }

        public override string ToString()
        {
            return (
                "{{Position:" + Position.ToString() +
                " Color:" + Color.ToString() +
                " TextureCoordinate:" + TextureCoordinate.ToString() +
                "}}"
            );
        }

        public static bool operator ==( VertexPositionColorTexture left, VertexPositionColorTexture right )
        {
            return ( ( left.Position == right.Position ) &&
                    ( left.Color == right.Color ) &&
                    ( left.TextureCoordinate == right.TextureCoordinate ) );
        }

        public static bool operator !=( VertexPositionColorTexture left, VertexPositionColorTexture right )
        {
            return !( left == right );
        }

        public override bool Equals( object obj )
        {
            if ( obj == null )
            {
                return false;
            }

            if ( obj.GetType() != base.GetType() )
            {
                return false;
            }

            return ( this == ( (VertexPositionColorTexture)obj ) );
        }

        #endregion
    }
}