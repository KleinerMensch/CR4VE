using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Controls;
using CR4VE.GameBase.Terrain;

namespace CR4VE.GameBase.Terrain
{
    class Tiles
    {
        #region Field Region
        protected Model model;
        private Rectangle rectangle;
        #endregion



        #region property region
        // wir haben allgemein einen array von Rectangles
        public Rectangle Rectangle
        {
            get { return rectangle; }
            protected set { rectangle = value; }
        }

        private static ContentManager content;

        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }
        #endregion

         #region Draw
          public void Draw(SpriteBatch spriteBtach)
          {
       //  spriteBtach.Draw(texture, rectangle, Color.White);

          }
          }
             #endregion

        #region class ColTiles, LOAD
        // 2DTexturen werden hier runtergeladen
        //wichtig: "tile"+1, dh bildformate nummerieren
        // hier müssen dann die 3D objekte hochgeladen werden

        class ColTiles : Tiles
        {
            public ColTiles(int i, Rectangle newRectangle)
            {
                model = Content.Load<Model>("Assets/Models/protoTerrain" + i);
                //texture = Content.Load<Texture2D>("Assets/Sprites/Tile" + i);


                this.Rectangle = newRectangle;
            }

        #endregion

        }
    }


        
    