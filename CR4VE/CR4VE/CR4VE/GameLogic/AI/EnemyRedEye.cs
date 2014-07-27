﻿using CR4VE.GameBase.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class EnemyRedEye : AIInterface
    {
        #region Attributes
        public Entity enemy;
        public Vector3 enemyPosition;

        Random random = new Random();
        float move;
        float rotationY;
        #endregion

        public EnemyRedEye(Vector3 position)
        {
            this.enemyPosition = position;
            move = -0.5f;
            rotationY = MathHelper.ToRadians(-90);
        }

        public void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            enemy = new Entity(enemyPosition, "EnemyEye", content);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
            enemy.position.X += move;
            if (enemy.position.X < 50 || enemy.position.X > 130)
            {
                move *= -1;
                rotationY += MathHelper.ToRadians(180);
            }

            // Position zum Test abfragen
            //Console.WriteLine(enemy.position);
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            enemy.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, rotationY, 0);
        }
    }
}
