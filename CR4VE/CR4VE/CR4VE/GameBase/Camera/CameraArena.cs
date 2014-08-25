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
        private static Vector3 position = new Vector3(0, 100, 100);
        private static Vector3 target = new Vector3(0, 0, 25);
        private static Vector2 viewportSize;
        private static float ratio;

        //Viewport BoundingFrunstum
        private static BoundingFrustum frustum;
        private static Vector3 frustumPos;
        private static Vector3 frustumTarget;

        //Viewport Matrices
        private static Matrix worldMatr = Matrix.Identity;
        private static Matrix viewMatr;
        private static Matrix projMatr;
        #endregion

        #region Properties
        public static Vector3 CamPosition
        {
            get { return position; }
            set { position = value; }
        }
        public static Vector3 CamTarget
        {
            get { return target; }
            set { target = value; }
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
        public static float AspectRatio
        {
            get { return ratio; }
            set { ratio = value; }
        }

        //BoundingFrustum
        public static BoundingFrustum BoundingFrustum
        {
            get { return frustum; }
            set { frustum = value; }
        }
        public static Vector3 FrustumPosition
        {
            get { return frustumPos; }
        }
        public static Vector3 FrustumTarget
        {
            get { return frustumTarget; }
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
        public static void Initialize(int width, int height)
        {
            //Screen Resolution
            ViewPortWidth = width;
            ViewPortHeight = height;

            //AspectRatio
            ratio = (float) width / height;

            //Viewport Matrices
            viewMatr = Matrix.CreateLookAt(CamPosition, CamTarget, Vector3.Up);
            projMatr = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), AspectRatio, 1, 1500);

            //Viewport BoundingFrustum
            BoundingFrustum = new BoundingFrustum(ViewMatrix * ProjectionMatrix);
            frustumPos = CamPosition;
            frustumTarget = CamTarget;
        }
        #endregion
    }
}
