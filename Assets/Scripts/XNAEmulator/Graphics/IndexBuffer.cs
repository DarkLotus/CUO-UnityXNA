using UnityEngine;

namespace Microsoft.Xna.Framework.Graphics
{
    public class IndexBuffer
    {
        public IndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize sixteenBits, int maxIndices, BufferUsage writeOnly)
        {
        }

        public void SetData(short[] generateIndexArray)
        {            
            m_Buffer.SetData(generateIndexArray);

        }
        internal ComputeBuffer m_Buffer;
        public ComputeBuffer Buffer => m_Buffer;
    }
}