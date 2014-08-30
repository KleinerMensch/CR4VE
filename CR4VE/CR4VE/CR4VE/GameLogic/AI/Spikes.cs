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
        float rotationY = MathHelper.ToRadians(-90);
        #endregion

        #region inherited Constructors
        public Spikes():base() { }
        public Spikes(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public Spikes(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
        #endregion

        public override void UpdateSingleplayer(Microsoft.Xna.Framework.GameTime gameTime) {

            //Vector3 playerPos = Singleplayer.player.position;
            //Vector3 direction = this.position - playerPos;
            float distance = this.position.X - Singleplayer.player.position.X;

            if (distance <= 10)
            {
                this.position.Y --; // fällt runter
            }

            //updating bounding box & check collision
            this.boundary.Min = this.position + new Vector3(-2,-2,-2);
            this.boundary.Max = this.position + new Vector3(2,2,2);

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }

        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.drawIn2DWorld(new Vector3(1,1,1), 0, rotationY, 0);
        }

    }
}
