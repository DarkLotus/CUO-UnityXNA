using System;
using System.Linq;
using ClassicUO.Renderer;

namespace Microsoft.Xna.Framework.Graphics
{
    public class DynamicVertexBuffer : VertexBuffer
    {
        public DynamicVertexBuffer(GraphicsDevice graphicsDevice, Type type, int maxVertices, BufferUsage writeOnly)
        {   
            
        }
    }
    public class VertexBuffer
    {
        internal PositionNormalTextureColor[] Data;
        public void SetData<T>(T[] vertexInfo, int i, int i1) where T : struct, IVertexType
        {
            var data = Enumerable.Cast<PositionNormalTextureColor>(vertexInfo);
            Data = data.ToArray();
        }
    }
}