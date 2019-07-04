using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Xna.Framework.Graphics
{
	public class Texture2D : IDisposable
	{
        private UnityEngine.Texture unityTexture;

        public UnityEngine.Texture UnityTexture
        {
            get { return ( UnityEngine.Texture )unityTexture; }
            set { unityTexture = value; }
        }

        public Texture2D()
        {

        }
        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);
        //public UnityEngine.RenderTexture RenderTexture => unityTexture as UnityEngine.RenderTexture;

        public Texture2D(UnityEngine.Texture2D unityTexture)
        {
            // TODO: Complete member initialization
            this.unityTexture = unityTexture;
        }
        public Texture2D( UnityEngine.RenderTexture unityTexture )
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
        public static int u16Tou32( ushort color )
        {
            Int32 red = (Int32)( ( ( color >> 0xA ) & 0x1F ) * 8.225806f );
            Int32 green = (Int32)( ( ( color >> 0x5 ) & 0x1F ) * 8.225806f );
            Int32 blue = (Int32)( ( color & 0x1F ) * 8.225806f );

            if ( red < 0 )
                red = 0;
            else if ( red > 0xFF )
                red = 0xFF;

            if ( green < 0 )
                green = 0;
            else if ( green > 0xFF )
                green = 255;

            if ( blue < 0 )
                blue = 0;
            else if ( blue > 0xFF )
                blue = 0xFF;

            return ( ( red << 0x10 ) | ( green << 0x8 ) | blue );
        }
        internal unsafe void SetData<T>( T[] data, int startIndex, int elementCount ) where T : struct
        {
            int sizeMulti = 2;

            if(typeof(T) == typeof(ushort) )
            {
                sizeMulti = 4;
                var bmp = new Bitmap( Width, Height, PixelFormat.Format32bppRgb );
                BitmapData bd = bmp.LockBits(
                    new System.Drawing.Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb );
                int[] temp = new int[elementCount];

                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = u16Tou32(Convert.ToUInt16(data[i]));
                }
                
                byte[] buf = new byte[elementCount * sizeMulti];
               
                Buffer.BlockCopy( temp, 0, buf, 0, buf.Length );
                Marshal.Copy( buf, 0, bd.Scan0, buf.Length );
                bmp.UnlockBits( bd );

                byte[] result = null;
                using ( MemoryStream stream = new MemoryStream() )
                {
                    bmp.Save( stream, ImageFormat.Png );
                    result = stream.ToArray();
                }

                UnityEngine.ImageConversion.LoadImage( UnityTexture as UnityEngine.Texture2D, result );
            }
            else if ( typeof( T ) == typeof( uint ) )
            {
                sizeMulti = 4;
                var bmp = new Bitmap( Width, Height, PixelFormat.Format32bppArgb );
                BitmapData bd = bmp.LockBits(
                    new System.Drawing.Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                byte[] buf = new byte[elementCount * sizeMulti];
                Buffer.BlockCopy( data, 0, buf, 0, buf.Length );
                Marshal.Copy( buf, 0, bd.Scan0, buf.Length );
                bmp.UnlockBits( bd );

                byte[] result = null;
                using ( MemoryStream stream = new MemoryStream() )
                {
                    bmp.Save( stream, ImageFormat.Png );
                    result = stream.ToArray();
                }

                UnityEngine.ImageConversion.LoadImage( UnityTexture as UnityEngine.Texture2D, result );
            }
            else
            {
                throw new Exception( "Not supported " + data.GetType() );
            }
            

            return;

            /*UnityEngine.Color[] cols = new UnityEngine.Color[elementCount];

            for ( int i = 0; i < elementCount; i++ )
            {
                if (typeof(T) == typeof(uint))
                {
                    Console.WriteLine("was uint");
                }

                ushort col = (ushort)Convert.ToUInt32(data[i]);//Convert.ToUInt16(data[i]);
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

            var destText = UnityTexture as UnityEngine.Texture2D;
            
             var dst = destText.GetRawTextureData<UnityEngine.Color>();
            for ( int i = 0; i < elementCount; i++ )
                if(i < cols.Length && i < dst.Length)
                    dst[i] = cols[i];
            else
                {
                    Console.Write( "fail??" );
                }
            destText.Apply();*/
            //TODO
        }
        }
    }
