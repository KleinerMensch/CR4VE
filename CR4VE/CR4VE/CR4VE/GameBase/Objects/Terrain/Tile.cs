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

        //Damage Values
        public static readonly int waterDmg = 210;
        public static readonly int lavaDmg = 1000;
        private int dmg;
        #endregion

        #region Properties
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }
        public int Damage
        {
            get { return this.dmg; }
        }
        #endregion

        #region Constructors
        public Tile(String modelType, int modelNumber, Vector3 pos, BoundingBox bound, int damage)
        {
            this.position = pos;
            this.model = Content.Load<Model>("Assets/Models/Terrain/" + modelType + modelNumber);
            this.boundary = bound;
            this.dmg = damage;
        }
        #endregion

        #region Methods
        public void Draw()
        {
            this.drawIn2DWorldWithoutBones(Vector3.One, 0, 0, 0);
        }
        #endregion
    }
}
