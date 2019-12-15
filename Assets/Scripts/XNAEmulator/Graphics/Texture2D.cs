using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Microsoft.Xna.Framework.Graphics
{
	public class Texture2D : IDisposable
    {

        public int Hash = -1;
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
            GraphicDevice = graphicsDevice;
        }

        public GraphicsDevice GraphicDevice { get; set; }

        public Texture2D( GraphicsDevice graphicsDevice, int width, int height, bool v, SurfaceFormat surfaceFormat ) : this( graphicsDevice, width, height )
        {
        }

        public int Width
        {
            get { return unityTexture?.width ?? 0; }
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
        
        public void GetData(ushort[] originalData, int i, int pheight)
        {
            throw new NotImplementedException();
        }
        public static int u16Tou32( ushort color )
        {
            //Bgra5551
            if (color == 0)
                return 0;
            byte alpha = (byte) ((color >> 15) * 255);
            byte red = (byte)( ( ( color >> 0xA ) & 0x1F ) * 8.225806f );
            byte green = (byte)( ( ( color >> 0x5 ) & 0x1F ) * 8.225806f );
            byte blue = (byte)( ( (color >> 0) & 0x1F ) * 8.225806f );
            var ret = new byte[] {alpha, red, green, blue};
            return BitConverter.ToInt32(ret,0);
            var res = ( ( red << 16 ) | ( green << 8 ) | blue );
            return res;
            if ( red < 0 )
                red = 0;
            else if ( red > 0xFF )
                red = 0xFF;

            if ( green < 0 )
                green = 0;
            else if ( green > 0xFF )
                green = 0xFF;

            if ( blue < 0 )
                blue = 0;
            else if ( blue > 0xFF )
                blue = 0xFF;

            return ( ( red << 16 ) | ( green << 8 ) | blue << 0 );

            return ( ( red << 0x10 ) | ( green << 0x8 ) | blue );
        }
        internal unsafe void SetData<T>( T[] data, int startIndex, int elementCount ) where T : struct
        {
            int sizeMulti = 2;

            if(typeof(T) == typeof(ushort) )
            {
                UnityTexture = new UnityEngine.Texture2D( UnityTexture.width, UnityTexture.height,TextureFormat.ARGB32,false);

                sizeMulti = 4;
                uint[] temp = new uint[elementCount];

                for (int i = 0; i < elementCount; i++)
                {
                    temp[i] = (uint)u16Tou32(Convert.ToUInt16(data[i]));
                }
                
                //byte[] buf = new byte[elementCount * sizeMulti];
               
               // Buffer.BlockCopy( temp, 0, buf, 0, buf.Length );
                
                var destText = UnityTexture as UnityEngine.Texture2D;
                var dst = destText.GetRawTextureData<uint>();

                //if(dst.Length == temp.Length)
                //    destText.LoadRawTextureData(buf);
               /* else
                {
                    for ( int i = 0; i < elementCount; i++ )
                        if(i < temp.Length && i < dst.Length)
                            dst[i] = data[i];
                        else
                        {
                            Console.Write( "fail??" );
                        }
                    
                }*/
               
              
               for(int i = 0; i < temp.Length / 2; i++) {
                   var tt = temp[i];
                   temp[i] = temp[temp.Length - i - 1];
                   temp[temp.Length - i - 1] = tt;
               }
               {
                   for (int i = 0; i < elementCount; i++)
                   {
                       int x = i % UnityTexture.width;
                       int y = i / UnityTexture.width;
                       y *= UnityTexture.width;
                       var index = y + (UnityTexture.width - x);
                       if(index < temp.Length && i < dst.Length)
                           dst[i] = temp[index];
                       else
                       {
                           Console.Write( "fail??" );
                       }
                   }
                        
                    
               }
               
               
               
               
                destText.Apply();
                
              
            }
            else if ( typeof( T ) == typeof( uint ) ||  typeof( T ) == typeof( Color )  )
            {
                sizeMulti = 4;
                UnityTexture = new UnityEngine.Texture2D( UnityTexture.width, UnityTexture.height,TextureFormat.ARGB32,false);

                //byte[] buf = new byte[elementCount * sizeMulti];
                //Buffer.BlockCopy( data, 0, buf, 0, buf.Length );
                var destText = UnityTexture as UnityEngine.Texture2D;
                
                
                for(int i = 0; i < data.Length / 2; i++) {
                    var temp = data[i];
                    data[i] = data[data.Length - i - 1];
                    data[data.Length - i - 1] = temp;
                }
               
            
                var dst = destText.GetRawTextureData<T>();
                /*if(dst.Length == buf.Length)
                    destText.LoadRawTextureData(buf);
                else*/
                {
                    for (int i = 0; i < elementCount; i++)
                    {
                        int x = i % UnityTexture.width;
                        int y = i / UnityTexture.width;
                        y *= UnityTexture.width;
                        var index = y + (UnityTexture.width - x);
                        if(index < data.Length && i < dst.Length)
                            dst[i] = data[index];
                        else
                        {
                            Console.Write( "fail??" );
                        }
                    }
                        
                    
                }

                destText.Apply();
            }
            
            else
            {
                throw new Exception( "Not supported " + data.GetType() );
            }

            Hash = UnityTexture.GetHashCode();
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
