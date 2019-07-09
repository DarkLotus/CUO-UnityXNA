#region license

//  Copyright (C) 2019 ClassicUO Development Community on Github
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

using Microsoft.Xna.Framework;

using SDL2;

namespace ClassicUO.Input
{
    internal static class Mouse
    {
        public const int MOUSE_DELAY_DOUBLE_CLICK = 350;
        private static Point _position;

        public static uint LastLeftButtonClickTime { get; set; }

        public static uint LastMidButtonClickTime { get; set; }

        public static uint LastRightButtonClickTime { get; set; }

        public static bool CancelDoubleClick { get; set; }

        public static bool LButtonPressed { get; set; }

        public static bool RButtonPressed { get; set; }

        public static bool MButtonPressed { get; set; }

        public static bool IsDragging { get; set; }

        public static Point Position => _position;

        public static Point RealPosition { get; private set; }

        public static Point LDropPosition { get; set; }

        public static Point RDropPosition { get; set; }

        public static Point MDropPosition { get; set; }

        public static Point LDroppedOffset => LButtonPressed ? RealPosition - LDropPosition : Point.Zero;

        public static Point RDroppedOffset => RButtonPressed ? RealPosition - RDropPosition : Point.Zero;

        public static Point MDroppedOffset => MButtonPressed ? RealPosition - MDropPosition : Point.Zero;

        public static bool MouseInWindow { get; set; }

        public static void Begin()
        {
            //SDL.SDL_CaptureMouse(SDL.SDL_bool.SDL_TRUE);
        }

        public static void End()
        {
          //  if (!(LButtonPressed || RButtonPressed || MButtonPressed))
          //      SDL.SDL_CaptureMouse(SDL.SDL_bool.SDL_FALSE);
        }

        public static void Update()
        {

 if ( UnityEngine.Input.mousePosition.x < 0 || UnityEngine.Input.mousePosition.y < 0 || UnityEngine.Input.mousePosition.x > UnityEngine.Screen.width || UnityEngine.Input.mousePosition.y > UnityEngine.Screen.height )
                return;
                _position.X = (int)UnityEngine.Input.mousePosition.x;
            _position.Y = UnityEngine.Screen.height - ( int)UnityEngine.Input.mousePosition.y;

            LButtonPressed = UnityEngine.Input.GetMouseButtonDown( 0 );
            RButtonPressed = UnityEngine.Input.GetMouseButtonDown( 1 );
            IsDragging = LButtonPressed || RButtonPressed || MButtonPressed;
            RealPosition = _position;
        }
    }
}