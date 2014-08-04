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
    class EnemyRedEye : AIInterface
    {
        #region Attributes
        public Entity enemy;
        public Vector3 enemyPosition;
        public static List<Entity> laserList = new List<Entity>();

        Random random = new Random();
        float move;
        float rotationY;
        public float spawn = 0;
        #endregion

        public EnemyRedEye(Vector3 position)
        {
            this.enemyPosition = position;
            move = -0.5f;
            rotationY = MathHelper.ToRadians(-90);
        }

        public void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            enemy = new Entity(enemyPosition,"EnemyEye", content);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            enemy.position.X += move;
            if (enemy.position.X < 50 || enemy.position.X > 105)
            {
                move *= -1;
                rotationY += MathHelper.ToRadians(180);
            }

            //updating bounding box
            enemy.boundary.Min = enemy.position + new Vector3(-3, -3, -3);
            enemy.boundary.Max = enemy.position + new Vector3(3, 3, 3);

            if (Singleplayer.player.boundary.Intersects(enemy.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }

            #region Collision with Laser
            foreach (Entity laser in laserList)
            {
                laser.boundary = new BoundingBox(laser.position + new Vector3(-2, -2, -2), laser.position + new Vector3(2, 2, 2));
                if (Singleplayer.player.boundary.Intersects(laser.boundary))
                {
                    Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
                }
                laser.position.X -= 0.7f;
            }
            #endregion
        }

        #region Laser vom Auge
        //bisher nur einzelne Nadeln gespawnt
        public void LoadEnemies(Vector3 EyePosition, ContentManager content)
        {
            // spawning laser every second
            if (spawn > 1)
            {
                spawn = 0;
                if (laserList.Count() < 10)
                    laserList.Add(new Entity(EyePosition, "ImaFirinMahLaserr", content));
            }
            for (int i = 0; i < laserList.Count; i++)
            {
                // wenn laser zu weit weg, dann verschwindet er
                // 'laser' verschwindet nocht nicht, wenn er den Spieler beruehrt
                if (laserList[i].position.X < 10 || laserList[i].position.X > 150)
                {
                    laserList.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            enemy.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, rotationY, 0);
        }
    }
}
