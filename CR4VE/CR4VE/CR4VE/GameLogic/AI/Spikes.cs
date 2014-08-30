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
    class Spikes : Enemy
    {
        #region Attributes
        public new float hp = 1;

        private readonly float rotX = MathHelper.ToRadians(90);
        private float distanceY = 0;

        private bool isFalling = false;
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
        public Spikes(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        public override void UpdateSingleplayer(GameTime gameTime)
        {
            float distance = 0;

            if (!isFalling)
                distance = Math.Abs(this.position.X - Singleplayer.player.position.X);

            if (distance <= 20)
            {
                isFalling = true;
            }

            if (isFalling)
            {
                this.move(new Vector3(0, -1.5f, 0)); //faellt runter
                this.distanceY += 1.5f;
            }

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.1);
                this.hp = 0;
            }
            else if (distanceY > 100)
                this.hp = 0;
        }

        public override void Draw()
        {
            this.drawIn2DWorldWithoutBones(new Vector3(1.1f, 1.1f, 1.1f), rotX, 0, 0);
        }

    }
}
