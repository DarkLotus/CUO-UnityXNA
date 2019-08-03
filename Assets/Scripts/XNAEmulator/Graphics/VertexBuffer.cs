using System;
using System.Linq;
using ClassicUO.Renderer;
using UnityEngine;

namespace Microsoft.Xna.Framework.Graphics
{
    public class DynamicVertexBuffer : VertexBuffer
    {


        public DynamicVertexBuffer(GraphicsDevice graphicsDevice, Type type, int maxVertices, BufferUsage writeOnly)
        {
            m_Buffer = new ComputeBuffer(maxVertices, PositionNormalTextureColor.SizeInBytes);
        }
    }
    public class VertexBuffer
    {
        internal ComputeBuffer m_Buffer;
        public void SetData<T>(T[] vertexInfo, int i, int i1) where T : struct, IVertexType
        {
           // var data = Enumerable.Cast<PositionNormalTextureColor>(vertexInfo);
            //Data = data.ToArray();
            m_Buffer.SetData(vertexInfo);
        }

        public ComputeBuffer Buffer => m_Buffer;
    }
}