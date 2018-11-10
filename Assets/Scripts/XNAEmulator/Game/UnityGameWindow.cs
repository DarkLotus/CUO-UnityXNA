using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Xna.Framework
{
    class UnityGameWindow : GameWindow
	{
        public override bool AllowUserResizing
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }
        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
        }
        public override Rectangle ClientBounds => new Rectangle( 0, 0, 640, 480 );
        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
        }
        public override IntPtr Handle
        {
            get { throw new NotImplementedException(); }
        }
        public override string ScreenDeviceName
        {
            get { throw new NotImplementedException(); }
        }
        protected override void SetTitle(string title)
        {
            
        }
	}
}
