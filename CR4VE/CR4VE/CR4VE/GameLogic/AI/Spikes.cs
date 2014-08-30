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
        private readonly float rotX = MathHelper.ToRadians(90);

        private bool isFalling = false;
        #endregion

        #region inherited Constructors
        public Spikes() : base() { }
        public Spikes(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public Spikes(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
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
                this.move(new Vector3(0, -1.5f, 0)); //faellt runter

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }
        }

        public override void Draw()
        {
            this.drawIn2DWorldWithoutBones(new Vector3(1.1f, 1.1f, 1.1f), rotX, 0, 0);
        }

    }
}
