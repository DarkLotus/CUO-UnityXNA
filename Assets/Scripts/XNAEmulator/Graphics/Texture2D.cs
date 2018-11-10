using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Xna.Framework.Graphics
{
	public class Texture2D : IDisposable
	{
        private UnityEngine.Texture2D unityTexture;

        public UnityEngine.Texture2D UnityTexture
        {
            get { return unityTexture; }
            set { unityTexture = value; }
        }

        public Texture2D()
        {

        }
        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        public Texture2D(UnityEngine.Texture2D unityTexture)
        {
            // TODO: Complete member initialization
            this.unityTexture = unityTexture;
        }

        public Texture2D( GraphicsDevice graphicsDevice, int width, int height )
        {
            UnityTexture = new UnityEngine.Texture2D( width, height);
        }

        public Texture2D( GraphicsDevice graphicsDevice, int width, int height, bool v, SurfaceFormat surfaceFormat ) : this( graphicsDevice, width, height )
        {
        }

        public int Width
        {
            get { return unityTexture.width; }
        }
        public int Height
        {
            get { return unityTexture.height; }
        }

        public bool IsDisposed { get; internal set; }

        public void Dispose()
        {
            UnityTexture = null;
            IsDisposed = true;
        }
        internal void SetData<T>( T[] data ) where T : struct
        {
            //TODO
            SetData<T>( data, 0, data.Length );
        }

        internal unsafe void SetData<T>( T[] data, int startIndex, int elementCount ) where T : struct
        {
            if(typeof(T) != typeof(ushort) )
            {
                return;
            }

            byte[] databy = new byte[elementCount * 2];
            Buffer.BlockCopy( data, 0, databy, 0, elementCount );
            UnityEngine.ImageConversion.LoadImage( UnityTexture, databy );
            UnityTexture.Apply();
            File.WriteAllBytes( "test.png", UnityEngine.ImageConversion.EncodeToPNG( UnityTexture ) );
            return;

            UnityEngine.Color[] cols = new UnityEngine.Color[elementCount];

            for ( int i = 0; i < elementCount; i++ )
            {
                ushort col = Convert.ToUInt16(data[i]);
                if(col > 0)
                {
                    int xxxx = 0;
                }
                int red = ( col & 0x7C00 ) >> 10;
                int green = ( col & 0x3E0 ) >> 5;
                int blue = ( col & 0x1F );
                int alpha = ( col & 0x8000 ) >> 15;
                red *= 50;
                green *= 50;
                blue *= 50;
                alpha *= 200;
                cols[i] = new UnityEngine.Color( (byte)red, (byte)green, (byte)blue, 255 );
            }

             var dst = UnityTexture.GetRawTextureData<UnityEngine.Color>();
            for ( int i = 0; i < elementCount; i++ )
                if(i < cols.Length && i < dst.Length)
                    dst[i] = cols[i];
            else
                {
                    Console.Write( "a" );
                }
            UnityTexture.Apply();
            //TODO
        }
        }
    }
