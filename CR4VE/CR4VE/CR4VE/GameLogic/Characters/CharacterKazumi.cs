﻿using CR4VE.GameBase.Objects;
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
    class CharacterKazumi : Character
    {
        #region Attributes
        public static new float manaLeft = 3;
        public List<Entity> meleeAttackList = new List<Entity>();
        public List<Entity> aoeList = new List<Entity>();
        Entity claws, danceOfFireFox;
        BoundingSphere rangeOfFireBall;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);

        Vector3 currentCharacterPosition;
        Vector3 clawsPosition;
        Vector3 offset = new Vector3(8,8,8);

        float speed = 1;

        bool enemyHit = false;
        bool enemyHitByMelee = false;
        bool listContainsClaws = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterKazumi() : base() { }
        public CharacterKazumi(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public CharacterKazumi(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
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
                aoeList.Remove(danceOfFireFox);

                launchedMelee = false;
                launchedSpecial = false;
                listContainsClaws = false;
            }
            #endregion

            #region Update Melee By Characterposition
            if (launchedMelee)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    clawsPosition = this.Position + viewingDirection * offset;
                    claws = new Entity(clawsPosition, "5x5x5Box1", Singleplayer.cont);
                    claws.boundary = new BoundingBox(clawsPosition + new Vector3(-8f, -2.5f, -2.5f), clawsPosition + new Vector3(8f, 2.5f, 2.5f));

                    if (!listContainsClaws)
                    {
                        meleeAttackList.Add(claws);
                        listContainsClaws = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(claws);
                    }

                    //Kollision mit Attacke
                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity kazumisClaws in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (kazumisClaws.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
                                Console.WriteLine("Kazumi hit enemy by MeleeAttack");
                            }
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        foreach (Entity kazumisClaws in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (kazumisClaws.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
                                Console.WriteLine("Kazumi hit enemy by MeleeAttack");
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    clawsPosition = this.position + viewingDirection * offset;
                    claws = new Entity(clawsPosition, "5x5x5Box1", Arena.cont);
                    claws.boundary = new BoundingBox(clawsPosition + new Vector3(-2.5f, -2.5f, -2.5f), clawsPosition + new Vector3(2.5f, 2.5f, 2.5f));
                    
                    if (!listContainsClaws)
                    {
                        meleeAttackList.Add(claws);
                        listContainsClaws = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(claws);
                    }

                    //Kollision mit Attacke
                    foreach (Entity kazumisClaws in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        if (kazumisClaws.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.seraphinBossHUD.healthLeft -= 5;
                            enemyHitByMelee = true;
                            Console.WriteLine("Kazumi hit Boss by MeleeAttack");
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    clawsPosition = this.position + viewingDirection * offset;
                    claws = new Entity(clawsPosition, "5x5x5Box1", Multiplayer.cont);
                    claws.boundary = new BoundingBox(clawsPosition + new Vector3(-2.5f, -2.5f, -2.5f), clawsPosition + new Vector3(2.5f, 2.5f, 2.5f));

                    if (!listContainsClaws)
                    {
                        meleeAttackList.Add(claws);
                        listContainsClaws = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(claws);
                    }

                    //Kollision mit Attacke
                    foreach (Entity kazumisClaws in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        //if (kazumisClaws.boundary.Intersects(Multiplayer.playerX.boundary))
                        //{
                        //    Multiplayer.playerXHUD.healthLeft -= 5;
                        //    enemyHitByMelee = true;
                        //    Console.WriteLine("Kazumi hit PlayerX by MeleeAttack");
                        //}
                    }
                }
                #endregion
            }
            #endregion

            #region UpdateFireball
            if (launchedRanged)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Feuerball fliegt nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfFireBall))
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
                                        Console.WriteLine("Kazumi hit enemy by RangedAttack");
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
                                        Console.WriteLine("Kazumi hit enemy by RangedAttack");
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
                        //Feuerball fliegt nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfFireBall))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedRanged = false;
                            break;
                        }
                        else
                        {
                            launchedRanged = true;

                            if (attackList[i].boundary.Intersects(Arena.boss.boundary))
                            {
                                Arena.fractusBossHUD.healthLeft -= 40;
                                enemyHit = true;
                                Console.WriteLine("Kazumi hit Boss by RangedAttack");
                            }
                            //verschwindet auch bei Kollision mit Gegner
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                    launchedRanged = false;
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
                        //Feuerball fliegt nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfFireBall))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedRanged = false;
                            break;
                        }
                        else
                        {
                            launchedRanged = true;

                            //if (attackList[i].boundary.Intersects(Multiplayer.playerX.boundary))
                            //{
                            //    Multiplayer.playerXHUD.healthLeft -= 40;
                            //    enemyHit = true;
                            //    Console.WriteLine("Kazumi hit PlayerX by RangedAttack");
                            //}
                            //verschwindet auch bei Kollision mit Gegner
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                    launchedRanged = false;
                                attackList.Remove(attackList[i]);
                            }
                        }
                        enemyHit = false;
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
            //Rest wird in der Update berechnet
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft > 0)
            {
                manaLeft -= 1;
                launchedRanged = true;
                currentCharacterPosition = this.Position;
                rangeOfFireBall = new BoundingSphere(currentCharacterPosition, 50);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    Entity fireBall = new Entity(this.position, "Enemies/skull", Singleplayer.cont);
                    fireBall.viewingDirection = viewingDirection;
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(fireBall);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    Entity fireBall = new Entity(this.position, "Enemies/skull", Arena.cont);
                    fireBall.viewingDirection = viewingDirection;
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(fireBall);
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    Entity fireBall = new Entity(this.position, "Enemies/skull", Multiplayer.cont);
                    fireBall.viewingDirection = viewingDirection;
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(fireBall);
                }
                #endregion

                //Rest wird in der Update berechnet
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (manaLeft >= 2)
            {
                manaLeft -= 2;
                launchedSpecial = true;
                timeSpan = TimeSpan.FromMilliseconds(270);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    danceOfFireFox = new Entity(this.Position, "Terrain/10x10x10Box1", Singleplayer.cont);
                    danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                    aoeList.Add(danceOfFireFox);

                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity kazumisDanceOfFirefox in aoeList)
                        {
                            if (kazumisDanceOfFirefox.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 3;
                                Console.WriteLine("Kazumi hit enemy by AoE");
                            }
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        foreach (Entity kazumisDanceOfFirefox in aoeList)
                        {
                            if (kazumisDanceOfFirefox.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 3;
                                Console.WriteLine("Kazumi hit enemy by AoE");
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    danceOfFireFox = new Entity(this.Position, "Terrain/10x10x10Box1", Arena.cont);
                    danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                    aoeList.Add(danceOfFireFox);

                    foreach (Entity kazumisDanceOfFirefox in aoeList)
                    {
                        if (kazumisDanceOfFirefox.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.fractusBossHUD.healthLeft -= 50;
                            Console.WriteLine("Kazumi hit Boss by AoE");
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    danceOfFireFox = new Entity(this.Position, "Terrain/10x10x10Box1", Multiplayer.cont);
                    danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                    aoeList.Add(danceOfFireFox);

                    foreach (Entity kazumisDanceOfFirefox in aoeList)
                    {
                        //if (kazumisDanceOfFirefox.boundary.Intersects(Multiplayer.playerX.boundary))
                        //{
                        //    Multiplayer.playerXHUD.healthLeft -= 50;
                        //    Console.WriteLine("Kazumi hit Boss by AoE");
                        //}
                    }
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
                {
                    foreach (Entity claw in meleeAttackList)
                    {
                        claw.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, MathHelper.ToRadians(90) * viewingDirection.X);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity claw in meleeAttackList)
                    {
                        claw.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                    }
                }
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity ball in attackList)
                    {
                        ball.drawIn2DWorld(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) * ball.viewingDirection.X, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity ball in attackList)
                    {
                        float ballBlickwinkel = (float)Math.Atan2(-ball.viewingDirection.Z, ball.viewingDirection.X);
                        ball.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + ballBlickwinkel, 0);
                    }
                }
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity dance in aoeList)
                    {
                        dance.drawIn2DWorld(Vector3.One, 0, 0, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity dance in aoeList)
                    {
                        dance.drawInArena(Vector3.One, 0, 0, 0);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
