using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class Hands : Enemy
     {
        #region Attributes
        public new float hp = 1;

        private float rotX = MathHelper.ToRadians(90);
        private float distanceY = 0;

        private bool isUP = false;
        #endregion

        #region Properties
        public override bool isDead
        {
	        get { return this.hp <= 0; }
        }
        public override float Health
        {
            get { return this.hp; }
            set { this.hp = value; }
        }
        #endregion

        #region inherited Constructor
        public Hands(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        public override void UpdateSingleplayer(GameTime gameTime)
        {
        }

        public override void Draw()
        {
            this.drawIn2DWorldWithoutBones(new Vector3(1.1f, 1.1f, 1.1f), rotX, 0, 0);
        }

    }
}
