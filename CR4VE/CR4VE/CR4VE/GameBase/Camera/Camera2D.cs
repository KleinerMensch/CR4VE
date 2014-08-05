using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace CR4VE.GameBase.Camera
{
    public static class Camera2D
    {
        #region Attributes
        private static Vector2 camPosition2D;
        private static Vector3 camPosition3D = new Vector3(0, 0, 100);
        private static Vector2 viewPortSize;
        private static Rectangle worldRec = new Rectangle(0, 0, 0, 0);
        private static BoundingFrustum frustum;
        private static Vector3 camTarget = Vector3.Zero;
        #endregion

        #region Properties
        //Viewport Size
        public static int ViewPortWidth
        {
            get { return (int)viewPortSize.X; }
            set { viewPortSize.X = value; }
        }
        public static int ViewPortHeight
        {
            get { return (int)viewPortSize.Y; }
            set { viewPortSize.Y = value; }
        }
        public static Vector3 ViewportCenter
        {
            get { return new Vector3 (camPosition2D,0) + new Vector3(ViewPortWidth / 2,ViewPortHeight / 2, 0); }
        }

        //Camera Positions
        public static Vector2 Position
        {
            get { return camPosition2D; }
            set
            {
                camPosition2D = new Vector2(MathHelper.Clamp(value.X, worldRec.X, worldRec.Width - ViewPortWidth),
                                       MathHelper.Clamp(value.Y, worldRec.Y, worldRec.Height - ViewPortHeight));
            }
        }
        //wird zum zeichnen der 3D Objekte gebraucht
        public static Vector3 CamPosition3D
        {
            get { return camPosition3D; }
            set { camPosition3D = value; }
        }

        //Rectangle representing Game World
        public static Rectangle WorldRectangle
        {
            get { return worldRec; }
            set { worldRec = value; }
        }

        //Viewport Rectangle
        public static Rectangle Viewport
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, ViewPortWidth, ViewPortHeight); }
        }

        public static BoundingFrustum BoundFrustum
        {
            get { return frustum; }
            set { frustum = value; }
        }

        public static Vector3 CamTarget
        {
            get { return camTarget; }
            set { camTarget = value; }
        }
        #endregion

        #region Methods
        public static void movePosition(Vector2 offset)
        {
            Position += offset;
        }
        public static void moveTarget(Vector3 offset)
        {
            CamTarget += offset;
        }

        //checks if boundary rectangle of an object intersects with viewport
        public static bool isVisible(Rectangle boundRec)
        {
            return Viewport.Intersects(boundRec);
        }

        //transforms world coordinates in screen coordinates
        public static Vector2 transform2D(Vector2 point)
        {
            return point - camPosition2D;
        }
        public static Rectangle transform2D(Rectangle rectangle)
        {
            return new Rectangle(rectangle.Left - (int)camPosition2D.X, rectangle.Top - (int)camPosition2D.Y, rectangle.Width, rectangle.Height);
        }

        public static Vector3 transform3D(Vector3 point)
        {
            return point - new Vector3(Position.X, -Position.Y, 0);
        }
        #endregion
    }
}