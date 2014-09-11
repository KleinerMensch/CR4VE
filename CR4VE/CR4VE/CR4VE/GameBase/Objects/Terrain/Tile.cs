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
using CR4VE.GameLogic.GameStates;

namespace CR4VE.GameBase.Objects.Terrain
{
    public class Tile : Entity
    {
        #region Attributes
        private static ContentManager content = Singleplayer.cont;

        //Damage Values
        public static readonly int waterDmg = 10;
        public static readonly int lethalDmg = 50;
        protected int dmg;

        protected String type = "default";
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
        public String Type
        {
            get { return type; }
        }
        #endregion

        #region Constructors
        public Tile() { }
        public Tile(String modelName, int modelNumber, Vector3 pos, BoundingBox bound, String type)
        {
            this.position = pos;
            this.model = Content.Load<Model>("Assets/Models/Terrain/" + modelName + modelNumber);
            this.boundary = bound;
            this.type = type;

            //define tile damage
            switch (type)
            {
                case "ground":
                    this.dmg = lethalDmg;
                    break;

                case "lava":
                    this.dmg = lethalDmg;
                    break;

                case "water":
                    this.dmg = waterDmg;
                    break;

                default:
                    this.dmg = 0;
                    break;
            }
        }
        #endregion

        #region Methods
        public virtual void Draw()
        {
            this.drawIn2DWorldWithoutBones(Vector3.One, 0, 0, 0);
        }
        #endregion
    }
}
