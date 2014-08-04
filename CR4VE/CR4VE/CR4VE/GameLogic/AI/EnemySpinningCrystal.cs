using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace CR4VE.GameLogic.AI
{
    class EnemySpinningCrystal : AIInterface
    {
        #region Attributes
        public Entity enemy;
        public Vector3 enemyPosition;

        Random random = new Random();
        float move;
        float rotationY;
        float rotationX;
        #endregion

        public EnemySpinningCrystal(Vector3 position)
        {
            this.enemyPosition = position;
            move = -0.5f;
            rotationX = 0.4f;
            rotationY = MathHelper.ToRadians(-90);
        }

        public void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            enemy = new Entity(enemyPosition, "enemySpinningNoAnim", content);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            rotationX -= 0.1f;

            Vector3 playerPos = Singleplayer.player.position;
            Vector3 direction = enemy.position - playerPos;
            float distance = direction.Length();

            if (distance < 50)
            {
                direction.Normalize();
                direction = move * direction;
                enemy.position += direction;
            } else {
                //noch zu aendern, was passiert, wenn player nicht mehr in der AggroRange
                enemy.position.X += move;
                if (enemy.position.X < 115 || enemy.position.X > 180)
                {
                    move *= -1;
                    rotationX *= -1;
                    rotationY += MathHelper.ToRadians(180);
                }
            }

            //updating bounding box & check collision
            enemy.boundary.Min = enemy.position + new Vector3(-3, -3, -3);
            enemy.boundary.Max = enemy.position + new Vector3(3, 3, 3);

            if (Singleplayer.player.boundary.Intersects(enemy.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            enemy.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, rotationY+rotationX, 0);
        }
    }
}
