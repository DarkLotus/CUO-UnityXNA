﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Graphics
{
    
    public class RenderTarget2D : Texture2D
    {
        public RenderTarget2D( GraphicsDevice graphicsDevice, int width, int height, bool v, SurfaceFormat surfaceFormat, DepthFormat depth24Stencil8, int v1, RenderTargetUsage discardContents ) : base (graphicsDevice,width,height,v,surfaceFormat)
        {

        }
    }
    /*public class RenderTarget2D : UnityEngine.RenderTexture
    {
        public RenderTarget2D( GraphicsDevice graphicsDevice, int width, int height, bool v, SurfaceFormat surfaceFormat, DepthFormat depth24Stencil8, int v1, RenderTargetUsage discardContents ) : base (width,height,24)
        {

        }
    }*/
}
