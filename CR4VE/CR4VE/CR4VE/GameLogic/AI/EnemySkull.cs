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
    class EnemySkull : Enemy
    {
        #region Attributes
        Random random = new Random();
        float moveSpeed = -0.5f;
        float rotationX = 0.4f;
        float rotationY = MathHelper.ToRadians(-90);
        #endregion

        #region inherited Constructors
        public EnemySkull():base() { }
        public EnemySkull(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public EnemySkull(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
        #endregion

        public override void UpdateSingleplayer(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //skull rolling over the floor
            this.position.X += moveSpeed;
            rotationX -= 0.1f;

            if (this.position.X < 230 || this.position.X > 280)
            {
                moveSpeed *= -1;
                rotationX *= -1;
                rotationY += MathHelper.ToRadians(180);
            }

            //updating bounding box
            this.boundary.Min = this.position + new Vector3(-3, -3, -3);
            this.boundary.Max = this.position + new Vector3(3, 3, 3);

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.drawIn2DWorld(new Vector3(0.05f, 0.05f, 0.05f), 0, rotationY , rotationX);
        }
    }
}
