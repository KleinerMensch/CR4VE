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
    class CharacterFractus : Character
    {
        #region Attributes
        public static new float manaLeft = 3;
        public static List<Entity> crystalList = new List<Entity>();
        Entity crystalShield, crystals;
        Enemy nearestEnemy = new Enemy();
        TimeSpan timeSpan = TimeSpan.FromSeconds(10);

        Vector3 currentViewingDirection;
        float speed = 1;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterFractus() : base() { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        #region Methods
        public override void Update(GameTime time, SoundEffect effect)
        {
            #region Update Crystals From Rangedattack
            if (launchedRanged)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    crystals.position += speed * currentViewingDirection;
                    crystals.boundary.Min += speed * currentViewingDirection;
                    crystals.boundary.Max += speed * currentViewingDirection;

                    //Kristalle verschwinden nach 50 Einheiten oder wenn sie mit etwas kollidiert
                    if (crystals.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedSpecial = true;
                        if (enemyHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(crystals);
                        }
                        else
                        {
                            #region enemyList1
                            foreach (Enemy enemy in Singleplayer.tileMaps[Singleplayer.activeIndex1].EnemyList)
                            {
                                if (enemyHit)
                                {
                                    launchedRanged = false;
                                    attackList.Remove(crystals);
                                }
                                else
                                {
                                    foreach (Entity fractusCrystals in attackList)
                                    {
                                        if (fractusCrystals.boundary.Intersects(enemy.boundary))
                                        {
                                            enemy.hp -= 2;
                                            enemyHit = true;
                                            Console.WriteLine("Fractus hit enemy by RangedAttack");
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region enemyList2
                            foreach (Enemy enemy in Singleplayer.tileMaps[Singleplayer.activeIndex2].EnemyList)
                            {
                                if (enemyHit)
                                {
                                    launchedRanged = false;
                                    attackList.Remove(crystals);
                                }
                                else
                                {
                                    foreach (Entity fractusCrystals in attackList)
                                    {
                                        if (fractusCrystals.boundary.Intersects(enemy.boundary))
                                        {
                                            enemy.hp -= 2;
                                            enemyHit = true;
                                            Console.WriteLine("Fractus hit enemy by RangedAttack");
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        launchedRanged = false;
                        attackList.Remove(crystals);
                    }
                    enemyHit = false;
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    crystals.position += speed * currentViewingDirection;
                    crystals.boundary.Min += speed * currentViewingDirection;
                    crystals.boundary.Max += speed * currentViewingDirection;

                    //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                    if (crystals.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedRanged = true;
                        if (enemyHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(crystals);
                        }
                        else
                        {
                            if (enemyHit)
                            {
                                launchedRanged = false;
                                attackList.Remove(crystals);
                            }
                            else
                            {
                                foreach (Entity fractusCrystals in attackList)
                                {
                                    if (fractusCrystals.boundary.Intersects(Arena.player.boundary))
                                    {
                                        //Arena.kazumiHud.healthLeft -= 3;
                                        enemyHit = true;
                                        Console.WriteLine("Fractus hit Player by SpecialAttack");
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        launchedRanged = false;
                        attackList.Remove(crystals);
                    }
                    enemyHit = false;
                }
                #endregion
            }
            #endregion

            #region Update HealthAbsorbingCrystals From Specialattack
            #region Removing Crystals After Defined Time
            // Decrements the timespan
            timeSpan -= time.ElapsedGameTime;
            // If the timespan is equal or smaller time "0"
            if (timeSpan <= TimeSpan.Zero && crystalList.Count > 0)
            {
                // Remove the object from list
                crystalList.RemoveAt(0);
                // Re initializes the timespan for the next time
                // minion vanishes after 10 seconds
                timeSpan = TimeSpan.FromSeconds(10);
                launchedSpecial = false;
            }
            #endregion

            foreach (Entity crystal in crystalList)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    crystal.boundary = new BoundingBox(crystal.position + new Vector3(-2, -2, -2), crystal.position + new Vector3(2, 2, 2));
                    float minDistance = float.MaxValue;

                    //calculating nearest enemy
                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.tileMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        float distance = (crystal.position - enemy.position).Length();
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.tileMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        float distance = (crystal.position - enemy.position).Length();
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }
                    #endregion

                    foreach (Entity healthAbsorbingCrystal in crystalList)
                    {
                        //absorbs health of nearest enemy if enemy is in range of effect
                        if ((healthAbsorbingCrystal.position - nearestEnemy.position).Length() < 50 && nearestEnemy.hp > 0)
                        {
                            Console.WriteLine(nearestEnemy.hp + " Fractus hit enemy by SpecialAttack");
                            nearestEnemy.hp -= 0.01f;

                            //transfers health to Fractus
                            if (Singleplayer.hud.trialsLeft <= 3 && Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                Singleplayer.hud.healthLeft += (int)(Singleplayer.hud.fullHealth * 0.01f);
                        }
                    }
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    crystal.boundary = new BoundingBox(crystal.position + new Vector3(-2, -2, -2), crystal.position + new Vector3(2, 2, 2));

                    foreach (Entity healthAbsorbingCrystal in crystalList)
                    {
                        //absorbs health of nearest enemy if enemy is in range of effect
                        if ((healthAbsorbingCrystal.position - Arena.player.position).Length() < 50 /*&& Arena.kazumiHud.healthleft > 0*/)
                        {
                            //Console.WriteLine(Arena.kazumiHud.healthleft + " Fractus hit enemy by SpecialAttack");
                            //Arena.kazumiHud.healthleft -= 0.01f;

                            //transfers health to Fractus in Arena
                            //if (Arena.fractusBossHud.trialsLeft <= 3 && Arena.fractusBossHud.healthLeft < Arena.fractusBossHud.fullHealth)
                            //    Arena.fractusBossHud.healthLeft += (int)(Arena.fractusBossHud.fullHealth * 0.01f);
                        }
                    }
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
                crystalShield = new Entity(this.position, "5x5x5Box1", Singleplayer.cont);
                crystalShield.boundary = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));
                attackList.Add(crystalShield);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    foreach (Entity fractusShield in attackList)
                    {
                        if (fractusShield.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 1;
                            Console.WriteLine("Fractus hit enemy by crystalShield");
                        }
                    }
                }
                attackList.Remove(crystalShield);
            }
            #endregion
            #region Arena
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                crystalShield = new Entity(this.position, "5x5x5Box1", Arena.cont);
                crystalShield.boundary = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));
                attackList.Add(crystalShield);

                foreach (Entity fractusShield in attackList)
                {
                    if (fractusShield.boundary.Intersects(Arena.player.boundary))
                    {
                        //Arena.kazumiHud.healthLeft -= 1;
                        Console.WriteLine("Fractus hit Player by crystalShield");
                    }
                }
                attackList.Remove(crystalShield);
            }
            #endregion
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft >= 1)
            {
                manaLeft -= 1;
                launchedRanged = true;

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    currentViewingDirection = Singleplayer.player.viewingDirection;
                    crystals = new Entity(this.position, "Enemies/skull", Singleplayer.cont);
                    crystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(crystals);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    currentViewingDirection = Arena.player.viewingDirection;
                    crystals = new Entity(this.position, "Enemies/skull", Arena.cont);
                    crystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(crystals);
                }
                #endregion
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (manaLeft >= 1.5f)
            {
                manaLeft -= 1.5f;
                launchedSpecial = true;

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    crystalList.Add(new Entity(this.position, "Enemies/enemySpinningNoAnim", CR4VE.GameLogic.GameStates.Singleplayer.cont));
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    crystalList.Add(new Entity(this.position, "Enemies/enemySpinningNoAnim", CR4VE.GameLogic.GameStates.Arena.cont));
                }
                #endregion

                //mehr als 2 Kristalle nicht moeglich
                if (crystalList.Count > 2)
                    crystalList.RemoveAt(0);
            }
        }

        public override void DrawAttacks()
        {
            #region DrawMelee
            if (launchedMelee)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    crystalShield.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    crystalShield.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                launchedMelee = false;
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    crystals.drawIn2DWorld(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90) * this.viewingDirection.X, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    crystals.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90) + Arena.blickWinkel, 0);
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity crystal in crystalList)
                    {
                        crystal.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, 0, 0);
                    }
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    foreach (Entity crystal in crystalList)
                    {
                        crystal.drawInArena(new Vector3(0.1f, 0.1f, 0.1f), 0, 0, 0);
                    }
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
