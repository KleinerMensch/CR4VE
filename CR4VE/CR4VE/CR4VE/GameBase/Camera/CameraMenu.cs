using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.Camera
{
    public static class CameraMenu
    {
        #region Attributes
        private static Vector2 position2D = new Vector2(0, 0);
        private static Vector3 position3D = new Vector3(0, 0, 100);
        private static Vector3 target = new Vector3(0, 0, 0);

        private static Rectangle menuRec = new Rectangle(0,0,0,0);
        private static Vector2 viewportSize;
        private static float ratio;

        //Viewport Matrices
        private static Matrix worldMatr = Matrix.Identity;
        private static Matrix viewMatr;
        private static Matrix projMatr;
        #endregion

        #region Properties
        //Positions
        public static Vector2 Position2D
        {
            get { return position2D; }
            set
            {
                position2D = new Vector2(MathHelper.Clamp(value.X, menuRec.X, menuRec.Width - viewportSize.X),
                                       MathHelper.Clamp(value.Y, menuRec.Y, menuRec.Height - viewportSize.Y));
            }
        }
        public static Vector3 Position3D
        {
            get { return position3D; }
        }

        //Background Dimensions
        public static Rectangle BackgroundRectangle
        {
            get { return menuRec; }
        }

        public static float AspectRatio
        {
            get { return ratio; }
        }

        //Viewport Matrices
        public static Matrix WorldMatrix
        {
            get { return worldMatr; }
        }
        public static Matrix ViewMatrix
        {
            get { return viewMatr; }
        }
        public static Matrix ProjectionMatrix
        {
            get { return projMatr; }
        }
        #endregion

        #region Methods
        public static void Initialize(float width, float height)
        {
            viewportSize = new Vector2(width, height);

            //maximum background sprite dimensions
            menuRec.Width = (int) width;
            menuRec.Height = (int) height;

            //Aspectratio
            ratio = (float) width / height;

            //Viewport Matrices
            viewMatr = Matrix.CreateLookAt(position3D, target, Vector3.Up);
            projMatr = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), ratio, 10f, 150);
        }

        //transforms world coordinates to screen coordinates
        public static Vector3 transform3D(Vector3 point)
        {
            return point - new Vector3(position3D.X, -position3D.Y, 0);
        }
        #endregion
    }
}
