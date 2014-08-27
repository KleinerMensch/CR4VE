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
        private static Vector3 camTarget = new Vector3(0, 0, 0);

        private static Vector2 viewPortSize;
        private static Rectangle worldRec = new Rectangle(0, 0, 0, 0);
        private static float ratio;
        
        //Viewport BoundingFrunstum
        private static BoundingFrustum frustum;
        private static Vector3 frustumPos;
        private static Vector3 frustumTarget;

        //Viewport Matrices
        private static Matrix worldMatr = Matrix.Identity;
        private static Matrix viewMatr;
        private static Matrix projMatr;

        //Grenzen fuer Kamerabewegung
        //(spaeter durch prozentuale Angaben ersetzen)
        private static readonly int topLimit = 20;
        private static readonly int botLimit = -20;
        private static readonly int leftLimit = -20;
        private static readonly int rightLimit = 20;
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
        public static float AspectRatio
        {
            get { return ratio; }
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
            get { return new Rectangle((int)Position2D.X, (int)Position2D.Y, ViewPortWidth, ViewPortHeight); }
        }

        //Camera Position (on 2D screen)
        public static Vector2 Position2D
        {
            get { return camPosition2D; }
            set
            {
                camPosition2D = new Vector2(MathHelper.Clamp(value.X, worldRec.X, worldRec.Width - ViewPortWidth),
                                       MathHelper.Clamp(value.Y, worldRec.Y, worldRec.Height - ViewPortHeight));
            }
        }

        //Camera Position (in 3D World)
        public static Vector3 Position3D
        {
            get { return camPosition3D; }
        }
        public static Vector3 CamTarget
        {
            get { return camTarget; }
        }

        //BoundingFrustum
        public static BoundingFrustum BoundFrustum
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
            viewMatr = Matrix.CreateLookAt(Position3D, CamTarget, Vector3.Up);
            projMatr = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), AspectRatio, 10f, 150);

            //Viewport BoundingFrustum
            BoundFrustum = new BoundingFrustum(viewMatr * projMatr);
            frustumPos = Position3D;
            frustumTarget = CamTarget;

            WorldRectangle = new Rectangle(0, 0, 1920, 1080);
        }

        //moves camera rectangle in 2D screen coordinates
        public static void movePosition(Vector2 offset)
        {
            Position2D += offset;
        }
        //moves BoundingFrustum in 3D screen coordinates
        public static void moveFrustum(Vector3 offset)
        {
            //adding offset values if screen limits arent reached
            if (Position2D.X > worldRec.X && Position2D.X < worldRec.Width - ViewPortWidth)
            {
                frustumPos.X += offset.X;
                frustumTarget.X += offset.X;
            }
            if (Position2D.Y > worldRec.Y && Position2D.Y < worldRec.Height - ViewPortHeight)
            {
                frustumPos.Y += offset.Y;
                frustumTarget.Y += offset.Y;
            }

            Matrix view = Matrix.CreateLookAt(frustumPos, frustumTarget, Vector3.Up);

            BoundFrustum = new BoundingFrustum(view * projMatr);
        }

        //moves Position2D of camera and BoundingFrustum accordingly
        public static void realign(Vector3 moveVecPlayer, Vector3 playerPos)
        {
            Vector2 moveVecCam = Vector2.Zero;
            Vector3 moveVecFrus = Vector3.Zero;

            //calculate moveVecCam if player reaches screen limit
            //(using absolute values because of reversed Y movement for 2D objects)
            if (transform3D(playerPos).X > rightLimit)
            {
                moveVecCam += new Vector2(Math.Abs(moveVecPlayer.X), 0);
                moveVecFrus += new Vector3(moveVecPlayer.X, 0, 0);
            }
            else if (transform3D(playerPos).X < leftLimit)
            {
                moveVecCam -= new Vector2(Math.Abs(moveVecPlayer.X), 0);
                moveVecFrus += new Vector3(moveVecPlayer.X, 0, 0);
            }

            if (transform3D(playerPos).Y > topLimit)
            {
                moveVecCam -= new Vector2(0, Math.Abs(moveVecPlayer.Y));
                moveVecFrus += new Vector3(0, moveVecPlayer.Y, 0);
            }
            else if (transform3D(playerPos).Y < botLimit)
            {
                moveVecCam += new Vector2(0, Math.Abs(moveVecPlayer.Y));
                moveVecFrus += new Vector3(0, moveVecPlayer.Y, 0);
            }

            //update Position2D
            movePosition(moveVecCam);

            //move position and target of BoundingFrustum (in 3D screen coordinates)
            moveFrustum(moveVecFrus);
        }

        //checks if a rectangle intersects with viewport
        public static bool isVisible(Rectangle boundRec)
        {
            return Viewport.Intersects(boundRec);
        }

        //transforms world coordinates to screen coordinates
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
            return point - new Vector3(Position2D.X, -Position2D.Y, 0);
        }
        #endregion
    }
}