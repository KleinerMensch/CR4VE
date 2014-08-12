using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.Characters
{
    class CharacterSeraphin : Character
    {
        #region Attributes
        public static List<Entity> minionList = new List<Entity>();
        TimeSpan timeSpan = TimeSpan.FromSeconds(10);

        float offset = 8;
        float speed = 1;
        float moveSpeed = -0.2f;
        float spawn = 0;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterSeraphin():base() { }
        public CharacterSeraphin(Vector3 pos, String modelName, ContentManager cm):base(pos, modelName, cm) { }
        public CharacterSeraphin(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
        #endregion

        #region Methods
        public override void Update(GameTime time)
        {
            spawn += (float)time.ElapsedGameTime.TotalSeconds;

            // Decrements the timespan
            timeSpan -= time.ElapsedGameTime;
            // If the timespan is equal or smaller time "0"
            if (timeSpan <= TimeSpan.Zero && minionList.Count > 0)
            {
                // Remove the object from list
                minionList.RemoveAt(0);
                // Re initializes the timespan for the next time
                // minion vanishes after 10 seconds
                timeSpan = TimeSpan.FromSeconds(10);
            }

            foreach (Entity minion in minionList)
            {
                minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));
                
                Vector3 direction = new Vector3(0,0,0);
                float minDistance = float.MaxValue;

                //checks which enemy is next to it
                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    Vector3 dir = minion.position - enemy.position;
                    float distance = dir.Length();
                    if (distance < minDistance)
                    {
                        direction = dir;
                        minDistance = distance;
                    }
                }
                direction.Normalize();
                direction = moveSpeed * direction;
                minion.position += direction;

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (minion.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 0.01f;
                        Console.WriteLine("Seraphin hit enemy by RangedAttack");
                    }
                }
            }
        }

        public override void MeleeAttack(GameTime time)
        {
            BoundingBox algaWhip = new BoundingBox(this.position + new Vector3(-1 + offset*viewingDirection.X, -1, -1), this.position + new Vector3(1 + offset*viewingDirection.X, 1, 1));
            foreach (Enemy enemy in Singleplayer.enemyList)
            {
                if (algaWhip.Intersects(enemy.boundary))
                {
                    enemy.health -= 1;
                    Console.WriteLine("Seraphin hit enemy by MeleeAttack");
                }
            }
        }

        public override void RangedAttack(GameTime time)
        {
            minionList.Add(new Entity(this.position, "EnemyEye", CR4VE.GameLogic.GameStates.Singleplayer.cont));

            //max. 3 minions can be spawned
            if (minionList.Count > 3)
                minionList.RemoveAt(0);
        }

        public override void SpecialAttack(GameTime time)
        {
            //bisher nur fuer den Singleplayer
            BoundingBox laser = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

            //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
            while (laser.Min.X != this.position.X + 50*viewingDirection.X)
            {
                if (enemyHit) break;
                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (enemyHit) break;
                    if (laser.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        enemyHit = true;
                        Console.WriteLine("Seraphin hit enemy by SpecialAttack");
                    }
                }
                laser.Min += new Vector3(speed*viewingDirection.X, 0, 0);
                laser.Max += new Vector3(speed*viewingDirection.X, 0, 0);
            }
            enemyHit = false;
        }
        #endregion
    }
}
