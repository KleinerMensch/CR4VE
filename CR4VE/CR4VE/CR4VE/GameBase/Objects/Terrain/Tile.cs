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

namespace CR4VE.GameBase.Objects.Terrain
{
    public class Tile : Entity
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
        public Tile(String modelType, int modelNumber, Vector3 pos, BoundingBox bound)
        {
            this.position = pos;
            this.model = Content.Load<Model>("Assets/Models/" + modelType + modelNumber);
            this.boundary = bound;
        }
        #endregion

        #region Methods
        #endregion
    }
}
