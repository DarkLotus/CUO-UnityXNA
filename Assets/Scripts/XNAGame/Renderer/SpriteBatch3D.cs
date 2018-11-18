#region license

//  Copyright (C) 2018 ClassicUO Development Community on Github
//
//	This project is an alternative client for the game Ultima Online.
//	The goal of this is to develop a lightweight client considering 
//	new technologies.  
//      
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Renderer
{
    // https://git.gmantaos.com/gamedev/Engine/blob/fcc0d5bcca1e9fcf5eb8481185ff7ecffbbe4fe2/Engine/Nez/Utils/MonoGameCompat.cs

    public static class Ext
    {
        public static void DrawIndexedPrimitives(this GraphicsDevice self, PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
        {
            //self.DrawIndexedPrimitives(primitiveType, baseVertex, 0, primitiveCount * 2, startIndex, primitiveCount);
        }
    }

}