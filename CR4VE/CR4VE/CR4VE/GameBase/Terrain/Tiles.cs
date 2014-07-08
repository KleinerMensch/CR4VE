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
    class Tiles : Entity
    {
        #region Attributes
        private static ContentManager content;
        #endregion

        #region Properties
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }



        #region ColTiles Class, LOAD
        //2D Texturen werden hier runtergeladen
        //wichtig: "tile"+1, d.h. Dateinamen entsprechend nummerieren
        //hier müssen dann die 3D Objekte hochgeladen werden
        class ColTiles : Tiles
        {
            #region Constructors
            public ColTiles(String type, int i, Vector3 pos)
            {
                this.position = pos;
                this.model = Content.Load<Model>("Assets/Models/" + type + i);
            }
            #endregion
        }
        #endregion

}
