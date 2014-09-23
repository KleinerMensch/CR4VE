using CR4VE.GameBase.HeadUpDisplay;
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
        public List<Entity> meleeAttackList = new List<Entity>();

        Entity crystalShield;
        BoundingSphere rangeOfFlyingCrystals;
        Enemy nearestEnemy = new Enemy();

        TimeSpan timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);

        Vector3 currentCharacterPosition;
        float speed = 1;
        bool enemyHit = false;
        bool enemyHitByMelee = false;
        bool listContainsCrystalShield = false;
        public bool soundPlayed = false;
        public bool soundPlayedRanged = false;
        #endregion

        #region Properties
        public override String CharacterType
        {
            get { return "Fractus"; }
        }
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterFractus() : base() { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        #region Methods
        public override void Update(GameTime time)
        {
            #region Timeupdate for DrawAttacks
            timeSpan -= time.ElapsedGameTime;
            if (timeSpan <= TimeSpan.Zero)
            {
                timeSpan = TimeSpan.FromMilliseconds(270);
                meleeAttackList.Clear();

                launchedMelee = false;
                listContainsCrystalShield = false;
            }
            #endregion

            #region Update Melee By Characterposition
            if (launchedMelee)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    crystalShield = new Entity(this.position, "5x5x5Box1", Singleplayer.cont);
                    crystalShield.boundary = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));

                    if (!listContainsCrystalShield)
                    {
                        meleeAttackList.Add(crystalShield);
                        listContainsCrystalShield = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(crystalShield);
                    }

                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity fractusShield in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (fractusShield.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
                                Console.WriteLine("Fractus hit enemy by crystalShield");
                            }
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        foreach (Entity fractusShield in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (fractusShield.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
                                Console.WriteLine("Fractus hit enemy by crystalShield");
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    crystalShield = new Entity(this.position, "5x5x5Box1", Arena.cont);
                    crystalShield.boundary = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));
                    
                    if (!listContainsCrystalShield)
                    {
                        meleeAttackList.Add(crystalShield);
                        listContainsCrystalShield = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(crystalShield);
                    }

                    foreach (Entity fractusShield in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        if (fractusShield.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.kazumiHud.healthLeft -= 1;
                            enemyHitByMelee = true;
                            Console.WriteLine("Fractus hit Boss by crystalShield");
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    crystalShield = new Entity(this.position, "5x5x5Box1", Multiplayer.cont);
                    crystalShield.boundary = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));

                    if (!listContainsCrystalShield)
                    {
                        meleeAttackList.Add(crystalShield);
                        listContainsCrystalShield = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(crystalShield);
                    }

                    foreach (Entity fractusShield in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        //if (fractusShield.boundary.Intersects(Multiplayer.playerX.boundary))
                        //{
                        //    Multiplayer.playerXHud.healthLeft -= 1;
                        //    enemyHitByMelee = true;
                        //    Console.WriteLine("Fractus hit PlayerX by crystalShield");
                        //}
                    }
                }
                #endregion
            }
            #endregion

            #region Update Flying Crystals From Rangedattack
            if (launchedRanged)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Kristalle fliegen nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                        if (!attackList[i].boundary.Intersects(rangeOfFlyingCrystals))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedRanged = false;
                            break;
                        }
                        else
                        {
                            launchedRanged = true;

                            #region enemyList1
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                            {
                                if (attackList.Count > 0)
                                {
                                    if (attackList[i].boundary.Intersects(enemy.boundary))
                                    {
                                        enemy.hp -= 2;
                                        enemyHit = true;
                                        Console.WriteLine("Fractus hit enemy by RangedAttack");
                                    }
                                    //verschwindet auch bei Kollision mit Gegner
                                    if (enemyHit)
                                    {
                                        if (attackList.Count == 0)
                                            launchedRanged = false;
                                        attackList.Remove(attackList[i]);
                                    }
                                }
                            }
                            #endregion
                            #region enemyList2
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                            {
                                if (attackList.Count > 0)
                                {
                                    if (attackList[i].boundary.Intersects(enemy.boundary))
                                    {
                                        enemy.hp -= 2;
                                        enemyHit = true;
                                        Console.WriteLine("Fractus hit enemy by RangedAttack");
                                    }
                                    //verschwindet auch bei Kollision mit Gegner
                                    if (enemyHit)
                                    {
                                        if (attackList.Count == 0)
                                            launchedRanged = false;
                                        attackList.Remove(attackList[i]);
                                    }
                                }
                            }
                            #endregion
                        }
                        enemyHit = false;
                    }
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Kristalle fliegen nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                        if (!attackList[i].boundary.Intersects(rangeOfFlyingCrystals))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                            {
                                launchedRanged = false;
                                soundPlayedRanged = false;
                            }
                            break;
                        }
                        else
                        {
                            launchedRanged = true;
                            if (!soundPlayedRanged)
                            {
                                Sounds.freezingIce.Play();

                                soundPlayedRanged = true;
                            }

                            if (attackList[i].boundary.Intersects(Arena.boss.boundary))
                            {
                                Arena.kazumiHud.healthLeft -= 3;
                                enemyHit = true;
                                Console.WriteLine("Fractus hit Player by SpecialAttack");
                            }
                            //verschwindet auch bei Kollision mit Gegner
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                {
                                    launchedRanged = false;
                                    soundPlayedRanged = false;
                                }

                                attackList.Remove(attackList[i]);
                            }
                        }
                        enemyHit = false;
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Kristalle fliegen nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                        if (!attackList[i].boundary.Intersects(rangeOfFlyingCrystals))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                            {
                                launchedRanged = false;
                                soundPlayedRanged = false;
                            }
                            break;
                        }
                        else
                        {
                            launchedRanged = true;
                            if (!soundPlayedRanged)
                            {
                                Sounds.freezingIce.Play();

                                soundPlayedRanged = true;
                            }

                            //if (attackList[i].boundary.Intersects(Multiplayer.playerX.boundary))
                            //{
                            //    Multiplayer.playerXHud.healthLeft -= 3;
                            //    enemyHit = true;
                            //    Console.WriteLine("Fractus hit PlayerX by SpecialAttack");
                            //}
                            //verschwindet auch bei Kollision mit Gegner
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                {
                                    launchedRanged = false;
                                    soundPlayedRanged = false;
                                }
                                attackList.Remove(attackList[i]);
                            }
                        }
                        enemyHit = false;
                    }
                }
                #endregion
            }
            #endregion

            #region Update HealthAbsorbingCrystals From Specialattack
            #region Removing Crystals After Defined Time
            timeSpanForHealthAbsorbingCrystals -= time.ElapsedGameTime;
            if (timeSpanForHealthAbsorbingCrystals <= TimeSpan.Zero && crystalList.Count > 0)
            {
                crystalList.RemoveAt(0);
                // Re initializes the timespan for the next time
                // minion vanishes after 10 seconds
                timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);

                if(crystalList.Count == 0)
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
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
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
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
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
                        if ((healthAbsorbingCrystal.position - Arena.boss.position).Length() < 15 && Arena.kazumiHud.healthLeft > 0)
                        {
                            Console.WriteLine(" Fractus hit enemy by SpecialAttack");
                            Arena.kazumiHud.healthLeft -= 1;

                            //transfers health to Fractus in Arena
                            if (Arena.fractusBossHUD.trialsLeft <= 3 && Arena.fractusBossHUD.healthLeft < Arena.fractusBossHUD.fullHealth)
                                Arena.fractusBossHUD.healthLeft += (int)(Arena.fractusBossHUD.fullHealth * 0.01f);
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    crystal.boundary = new BoundingBox(crystal.position + new Vector3(-2, -2, -2), crystal.position + new Vector3(2, 2, 2));

                    foreach (Entity healthAbsorbingCrystal in crystalList)
                    {
                        ////absorbs health of nearest enemy if enemy is in range of effect
                        //if ((healthAbsorbingCrystal.position - nearestPlayer).Length() < 15 && nearestPlayerHud.healthLeft > 0)
                        //{
                        //    Console.WriteLine(" Fractus hit nearestPlayer by SpecialAttack");
                        //    nearestPlayerHud.healthLeft -= 1;

                        //    //transfers health to Fractus in Arena
                        //    if (Multiplayer.fractusHUD.trialsLeft <= 3 && Multiplayer.fractusHUD.healthLeft < Multiplayer.fractusHUD.fullHealth)
                        //        Multiplayer.fractusHUD.healthLeft += (int)(Multiplayer.fractusHUD.fullHealth * 0.01f);
                        //}
                    }
                }
                #endregion
            }
            #endregion
        }

        public override void MeleeAttack(GameTime time)
        {
            launchedMelee = true;
            timeSpan = TimeSpan.FromMilliseconds(270);
            enemyHitByMelee = false;
            soundPlayed = false;
            if (!soundPlayed)
            {
                Sounds.freezingIce.Play();

                soundPlayed = true;
            }
            //Rest wird in der Update berechnet
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft >= 1)
            {
                manaLeft -= 1;
                launchedRanged = true;
                currentCharacterPosition = this.Position;
                rangeOfFlyingCrystals = new BoundingSphere(currentCharacterPosition, 50);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    Entity flyingCrystals = new Entity(this.position, "Enemies/skull", Singleplayer.cont);
                    flyingCrystals.viewingDirection = viewingDirection;
                    flyingCrystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(flyingCrystals);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    Entity flyingCrystals = new Entity(this.position, "Enemies/skull", Arena.cont);
                    flyingCrystals.viewingDirection = viewingDirection;
                    flyingCrystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(flyingCrystals);
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    Entity flyingCrystals = new Entity(this.position, "Enemies/skull", Multiplayer.cont);
                    flyingCrystals.viewingDirection = viewingDirection;
                    flyingCrystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(flyingCrystals);
                }
                #endregion

                //Rest wird in der Update berechnet
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (manaLeft >= 1.5f)
            {
                manaLeft -= 1.5f;
                launchedSpecial = true;
                timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);

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
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    crystalList.Add(new Entity(this.position, "Enemies/enemySpinningNoAnim", CR4VE.GameLogic.GameStates.Multiplayer.cont));
                }
                #endregion

                //Rest wird in der Update berechnet

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
                {
                    foreach (Entity shield in meleeAttackList)
                    {
                        shield.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity shield in meleeAttackList)
                    {
                        shield.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                    }
                }
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity fractusCrystals in attackList)
                    {
                        fractusCrystals.drawIn2DWorld(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90) * this.viewingDirection.X, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity fractusCrystals in attackList)
                    {
                        float crystalBlickwinkel = (float)Math.Atan2(-fractusCrystals.viewingDirection.Z, fractusCrystals.viewingDirection.X);
                        fractusCrystals.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90) + crystalBlickwinkel, 0);
                    }
                }
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
                else if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
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
