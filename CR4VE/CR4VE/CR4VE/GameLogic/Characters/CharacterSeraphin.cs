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

        Vector3 offset = new Vector3(8,8,8);
        float speed = 1;
        float moveSpeed = -0.2f;
        float spawn = 0;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterSeraphin() : base() { }
        public CharacterSeraphin(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public CharacterSeraphin(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
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
                //checks which enemy is next to it
                foreach (Enemy enemy in Arena.enemyList)
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
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Vector3 algaWhipPosition = Singleplayer.player.Position + viewingDirection * offset;
                Entity algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Singleplayer.cont);
                algaWhip.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                algaWhip.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (algaWhip.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Seraphin hit enemy by MeleeAttack");
                    }
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Vector3 algaWhipPosition = Arena.player.Position + viewingDirection * offset;
                Entity algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Arena.cont);
                algaWhip.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                algaWhip.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                foreach (Enemy enemy in Arena.enemyList)
                {
                    if (algaWhip.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Seraphin hit enemy by MeleeAttack");
                    }
                }
            }
        }

        public override void RangedAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                minionList.Add(new Entity(this.position, "EnemyEye", CR4VE.GameLogic.GameStates.Singleplayer.cont));
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                minionList.Add(new Entity(this.position, "EnemyEye", CR4VE.GameLogic.GameStates.Arena.cont));
            }
            //max. 3 minions can be spawned
            if (minionList.Count > 3)
                minionList.RemoveAt(0);
        }

        public override void SpecialAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Entity laser = new Entity(this.position, "skull", Singleplayer.cont);
                laser.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                while (laser.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    foreach (Enemy enemy in Singleplayer.enemyList)
                    {
                        if (enemyHit) break;
                        if (laser.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            enemyHit = true;
                            Console.WriteLine("Seraphin hit enemy by RangedAttack");
                        }
                    }
                    laser.position += speed * viewingDirection;
                    laser.boundary.Min += speed * viewingDirection;
                    laser.boundary.Max += speed * viewingDirection;
                    laser.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Entity laser = new Entity(this.position, "skull", Arena.cont);
                laser.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                while (laser.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    foreach (Enemy enemy in Arena.enemyList)
                    {
                        if (enemyHit) break;
                        if (laser.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            enemyHit = true;
                            Console.WriteLine("Seraphin hit enemy by RangedAttack");
                        }
                    }
                    laser.position += speed * viewingDirection;
                    laser.boundary.Min += speed * viewingDirection;
                    laser.boundary.Max += speed * viewingDirection;
                    laser.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
        }
        #endregion
    }
}
