using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace CR4VE.GameBase.Camera
{
    public static class CameraArena
    {
        #region Attributes
        private static Vector3 position = new Vector3(0, 50, 50);
        private static Vector2 viewportSize;
        #endregion

        #region Properties
        public static Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public static int ViewPortWidth
        {
            get { return (int)viewportSize.X; }
            set { viewportSize.X = value; }
        }
        public static int ViewPortHeight
        {
            get { return (int)viewportSize.Y; }
            set { viewportSize.Y = value; }
        }
        #endregion

        #region Methods
        
        #endregion
    }
}
