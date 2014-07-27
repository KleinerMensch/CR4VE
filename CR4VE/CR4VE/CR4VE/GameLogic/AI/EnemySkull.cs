using CR4VE.GameBase.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class EnemySkull : AIInterface
    {
        #region Attributes
        public Entity enemy;
        public Vector3 enemyPosition;

        Random random = new Random();
        float move;
        float rotationY;
        float rotationX;
        #endregion

        public EnemySkull(Vector3 position)
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
            enemy.position.X += move;

            rotationX -= 0.1f;

            if (enemy.position.X < 230 || enemy.position.X > 280)
            {
                move *= -1;
                rotationX *= -1;
                rotationY += MathHelper.ToRadians(180);
            }
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            enemy.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, rotationY + rotationX, 0);
        }
    }
}
