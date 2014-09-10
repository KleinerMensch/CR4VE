using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public static new float manaLeft = 3;
        public static List<Entity> minionList = new List<Entity>();
        Entity algaWhip, laser;
        TimeSpan timeSpan = TimeSpan.FromSeconds(10);

        Vector3 currentViewingDirection;
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
        public override void Update(GameTime time, SoundEffect effect)
        {
            #region MinionsFromRangedAttack
            #region Removing Minions After Defined Time
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
                launchedRanged = false;
            }
            #endregion

            foreach (Entity minion in minionList)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    Vector3 direction = new Vector3(0, 0, 0);
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
                            enemy.hp -= 0.01f;
                            Console.WriteLine("Seraphin hit enemy by RangedAttack");
                        }
                    }
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    Vector3 direction = minion.position - Arena.player.position;
                    direction.Normalize();
                    direction = moveSpeed * direction;
                    minion.position += direction;

                    if (minion.boundary.Intersects(Arena.player.boundary))
                    {
                        Arena.opheliaHud.healthLeft -= 1;
                        Console.WriteLine("Seraphin hit enemy by RangedAttack");
                    }
                }
                #endregion
            }
            #endregion

            #region UpdateLaserFromSpecialAttack
            if (launchedSpecial)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    laser.position += speed * currentViewingDirection;
                    laser.boundary.Min += speed * currentViewingDirection;
                    laser.boundary.Max += speed * currentViewingDirection;

                    //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                    if (laser.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedSpecial = true;
                        if (enemyHit)
                        {
                            launchedSpecial = false;
                            attackList.Remove(laser);
                        }
                        else
                        {
                            foreach (Enemy enemy in Singleplayer.enemyList)
                            {
                                if (enemyHit)
                                {
                                    launchedSpecial = false;
                                    attackList.Remove(laser);
                                }
                                else
                                {
                                    foreach (Entity seraphinsLaser in attackList)
                                    {
                                        if (seraphinsLaser.boundary.Intersects(enemy.boundary))
                                        {
                                            enemy.hp -= 3;
                                            enemyHit = true;
                                            Console.WriteLine("Seraphin hit enemy by SpecialAttack");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        launchedSpecial = false;
                        attackList.Remove(laser);
                    }
                    enemyHit = false;
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    laser.position += speed * currentViewingDirection;
                    laser.boundary.Min += speed * currentViewingDirection;
                    laser.boundary.Max += speed * currentViewingDirection;

                    //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                    if (laser.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedSpecial = true;
                        if (enemyHit)
                        {
                            launchedSpecial = false;
                            attackList.Remove(laser);
                        }
                        else
                        {
                            if (enemyHit)
                            {
                                launchedSpecial = false;
                                attackList.Remove(laser);
                            }
                            else
                            {
                                foreach (Entity seraphinsLaser in attackList)
                                {
                                    if (seraphinsLaser.boundary.Intersects(Arena.player.boundary))
                                    {
                                        Arena.opheliaHud.healthLeft -= 3;
                                        enemyHit = true;
                                        Console.WriteLine("Seraphin hit Player by SpecialAttack");
                                    }
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        launchedSpecial = false;
                        attackList.Remove(laser);
                    }
                    enemyHit = false;
                }
                #endregion
            }
            #endregion
        }

        public override void MeleeAttack(GameTime time)
        {
            launchedMelee = true;

            #region Singleplayer
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Vector3 algaWhipPosition = Singleplayer.player.Position + viewingDirection * offset;
                algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Singleplayer.cont);
                algaWhip.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                attackList.Add(algaWhip);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    foreach (Entity seraphinsWhip in attackList)
                    {
                        if (seraphinsWhip.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 1;
                            Console.WriteLine("Seraphin hit enemy by MeleeAttack");
                        }
                    }
                }
            }
            #endregion
            #region Arena
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Vector3 algaWhipPosition = this.position + viewingDirection * offset;
                algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Arena.cont);
                algaWhip.boundary = new BoundingBox(this.position + new Vector3(-3,-3,-3) + viewingDirection * offset, this.position + new Vector3(3,3,3) + viewingDirection * offset);
                attackList.Add(algaWhip);


                foreach (Entity seraphinsWhip in attackList)
                {
                    if (seraphinsWhip.boundary.Intersects(Arena.player.boundary))
                    {
                        Arena.opheliaHud.healthLeft -= 1;
                        Console.WriteLine("Seraphin hit Player by MeleeAttack");
                    }
                }
                
            }
            #endregion
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft > 0)
            {
                manaLeft -= 1;
                launchedRanged = true;

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Singleplayer.cont));
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Arena.cont));
                }
                #endregion

                //max. 3 minions can be spawned
                if (minionList.Count > 3)
                    minionList.RemoveAt(0);
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            //Laser kann nur bei vollem Mana benutzt werden
            if (manaLeft == 3)
            {
                manaLeft -= 3;
                launchedSpecial = true;

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    currentViewingDirection = Singleplayer.player.viewingDirection;
                    laser = new Entity(this.position, "Enemies/skull", Singleplayer.cont);
                    laser.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(laser);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    currentViewingDirection = Arena.player.viewingDirection;
                    laser = new Entity(this.position, "Enemies/skull", Arena.cont);
                    laser.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(laser);
                }
                #endregion
            }
        }

        public override void DrawAttacks()
        {
            #region DrawMelee
            if (launchedMelee)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    algaWhip.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    algaWhip.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                launchedMelee = false;
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity minion in minionList)
                    {
                        minion.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);
                    }
                }
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    foreach (Entity minion in minionList)
                    {
                        minion.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);
                    }
                }
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    laser.drawIn2DWorld(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    laser.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
            }
            #endregion
        }
        #endregion
    }
}
