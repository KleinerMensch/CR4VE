﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace CR4VE.GameBase.Camera
{
    public static class Camera2D
    {
        #region Attributes
        private static Vector2 position;
        private static Vector3 camPosition3D = new Vector3(0 , 0, 100);
        private static Vector2 viewPortSize;
        //private static Vector3 viewPortCenter;
        private static Rectangle worldRec = new Rectangle(0, 0, 0, 0);
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
            get { return new Vector3 (position,0) + new Vector3(ViewPortWidth / 2,ViewPortHeight / 2, 0); }
        }

        //Camera Positions
        public static Vector2 Position
        {
            get { return position; }
            set
            {
                position = new Vector2(MathHelper.Clamp(value.X, worldRec.X, worldRec.Width - ViewPortWidth),
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
        #endregion

        #region Methods
        public static void move(Vector2 offset)
        {
            Position += offset;
        }

        //checks if boundary rectangle of an object intersects with viewport
        public static bool isVisible(Rectangle boundRec)
        {
            return Viewport.Intersects(boundRec);
        }

        //transforms world coordinates in screen coordinates
        public static Vector2 transform2D(Vector2 point)
        {
            return point - position;
        }
        public static Rectangle transform2D(Rectangle rectangle)
        {
            return new Rectangle(rectangle.Left - (int)position.X, rectangle.Top - (int)position.Y, rectangle.Width, rectangle.Height);
        }

        public static Vector3 transform3D(Vector3 point)
        {
            // + 50 und -50 nur fuer meilenstein3
            return point - new Vector3(Position.X+50, -Position.Y-35, 0);
        }
        #endregion
    }
}