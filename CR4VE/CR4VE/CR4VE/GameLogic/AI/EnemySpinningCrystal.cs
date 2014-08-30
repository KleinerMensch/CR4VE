using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameLogic.Controls;
using Microsoft.Xna.Framework.Input;

namespace CR4VE.GameLogic.AI
{
    class EnemySpinningCrystal : Enemy
    {
        #region Attributes
        Random random = new Random();
        float moveSpeed = -0.5f;
        float rotationX = 0.4f;
        float rotationY = MathHelper.ToRadians(-90);
        #endregion

        #region inherited Constructors
        public EnemySpinningCrystal():base() { }
        public EnemySpinningCrystal(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public EnemySpinningCrystal(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        public override void UpdateSingleplayer(GameTime gameTime)
        {
            rotationX -= 0.1f;

            Vector3 playerPos = Singleplayer.player.position;
            Vector3 direction = this.position - playerPos;
            float distance = direction.Length();

            if (distance < 50)
            {
                direction.Normalize();
                direction = moveSpeed * direction;
                this.position += direction;
            } else {
                //noch zu aendern, was passiert, wenn player nicht mehr in der AggroRange
                this.position.X += moveSpeed;
                if (this.position.X < 150 || this.position.X > 250)
                {
                    moveSpeed *= -1;
                    rotationX *= -1;
                    rotationY += MathHelper.ToRadians(180);
                }
            }

            //updating bounding box & check collision
            this.boundary.Min = this.position + new Vector3(-3, -3, -3);
            this.boundary.Max = this.position + new Vector3(3, 3, 3);

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
                
            }
        }

        public override void Draw()
        {
            this.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, rotationY + rotationX, 0);
        }
    }
}
