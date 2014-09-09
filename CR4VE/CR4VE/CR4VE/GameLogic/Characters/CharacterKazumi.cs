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
    class CharacterKazumi : Character
    {
        #region Attributes
        public static new float manaLeft = 3;
        Entity claws, fireBall, danceOfFireFox;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);

        Vector3 offset = new Vector3(8,8,8);
        Vector3 currentViewingDirection;
        float speed = 1;
        bool enemyHit = false;
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
            // Decrements the timespan
            timeSpan -= time.ElapsedGameTime;
            // If the timespan is equal or smaller time "0"
            if (timeSpan <= TimeSpan.Zero)
            {
                timeSpan = TimeSpan.FromMilliseconds(270);
                attackList.Remove(claws);
                attackList.Remove(danceOfFireFox);
                launchedMelee = false;
                launchedSpecial = false;
            }
            #endregion

            #region UpdateFireball
            if (launchedRanged)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    fireBall.move(speed * currentViewingDirection);

                    //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                    if (fireBall.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedRanged = true;
                        if (enemyHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(fireBall);
                        }
                        else
                        {
                            #region enemyList1
                            foreach (Enemy enemy in Singleplayer.gameMaps[Singleplayer.activeIndex1].EnemyList)
                            {
                                if (enemyHit)
                                {
                                    launchedRanged = false;
                                    attackList.Remove(fireBall);
                                }
                                else
                                {
                                    foreach (Entity kazumisFireBall in attackList)
                                    {
                                        if (kazumisFireBall.boundary.Intersects(enemy.boundary))
                                        {
                                            enemy.hp -= 2;
                                            enemyHit = true;
                                            Console.WriteLine("Kazumi hit enemy by RangedAttack");
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region enemyList2
                            foreach (Enemy enemy in Singleplayer.gameMaps[Singleplayer.activeIndex2].EnemyList)
                            {
                                if (enemyHit)
                                {
                                    launchedRanged = false;
                                    attackList.Remove(fireBall);
                                }
                                else
                                {
                                    foreach (Entity kazumisFireBall in attackList)
                                    {
                                        if (kazumisFireBall.boundary.Intersects(enemy.boundary))
                                        {
                                            enemy.hp -= 2;
                                            enemyHit = true;
                                            Console.WriteLine("Kazumi hit enemy by RangedAttack");
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
                        attackList.Remove(fireBall);
                    }
                    enemyHit = false;
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    fireBall.move(speed * currentViewingDirection);

                    //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                    if (fireBall.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedRanged = true;
                        if (enemyHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(fireBall);
                        }
                        else
                        {
                            if (enemyHit)
                            {
                                launchedRanged = false;
                                attackList.Remove(fireBall);
                            }
                            else
                            {
                                foreach (Entity kazumisFireball in attackList)
                                {
                                    if (kazumisFireball.boundary.Intersects(Arena.boss.boundary))
                                    {
                                        //Arena.fractusHud.healthLeft -= 1;
                                        enemyHit = true;
                                        Console.WriteLine("Kazumi hit Boss by RangedAttack");
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        launchedRanged = false;
                        attackList.Remove(fireBall);
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
            currentViewingDirection = viewingDirection;
            timeSpan = TimeSpan.FromMilliseconds(270);

            #region Singleplayer
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Vector3 clawsPosition = Singleplayer.player.Position + currentViewingDirection * offset;
                claws = new Entity(clawsPosition, "5x5x5Box1", Singleplayer.cont);
                claws.boundary = new BoundingBox(this.position + new Vector3(-8f, -2.5f, -2.5f) + currentViewingDirection * offset, this.position + new Vector3(8f, 2.5f, 2.5f) + currentViewingDirection * offset);
                attackList.Add(claws);

                //Kollision mit Attacke
                #region enemyList1
                foreach (Enemy enemy in Singleplayer.gameMaps[Singleplayer.activeIndex1].EnemyList)
                {
                    foreach (Entity kazumisClaws in attackList)
                    {
                        if (kazumisClaws.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 1;
                            Console.WriteLine("Kazumi hit enemy by MeleeAttack");
                        }
                    }
                }
                #endregion
                #region enemyList2
                foreach (Enemy enemy in Singleplayer.gameMaps[Singleplayer.activeIndex2].EnemyList)
                {
                    foreach (Entity kazumisClaws in attackList)
                    {
                        if (kazumisClaws.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 1;
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
                Vector3 clawsPosition = Arena.player.Position + currentViewingDirection * offset;
                claws = new Entity(clawsPosition, "5x5x5Box1", Arena.cont);
                claws.boundary = new BoundingBox(this.position + new Vector3(-8f, -2.5f, -2.5f) + currentViewingDirection * offset, this.position + new Vector3(8f, 2.5f, 2.5f) + currentViewingDirection * offset);
                attackList.Add(claws);

                //Kollision mit Attacke
                foreach (Entity kazumisClaws in attackList)
                {
                    if (kazumisClaws.boundary.Intersects(Arena.boss.boundary))
                    {
                        Arena.seraphinBossHUD.healthLeft -= 5;
                        Console.WriteLine("Kazumi hit Boss by MeleeAttack");
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
                    currentViewingDirection = Singleplayer.player.viewingDirection;
                    fireBall = new Entity(this.position, "Enemies/skull", Singleplayer.cont);
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(fireBall);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    currentViewingDirection = Arena.player.viewingDirection;
                    fireBall = new Entity(this.position, "Enemies/skull", Arena.cont);
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(fireBall);
                }
                #endregion
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
                    attackList.Add(danceOfFireFox);

                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.gameMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity kazumisDanceOfFirefox in attackList)
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
                    foreach (Enemy enemy in Singleplayer.gameMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        foreach (Entity kazumisDanceOfFirefox in attackList)
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
                    attackList.Add(danceOfFireFox);

                    foreach (Entity kazumisDanceOfFirefox in attackList)
                    {
                        if (kazumisDanceOfFirefox.boundary.Intersects(Arena.boss.boundary))
                        {
                            //Arena.fractusBossHUD.healthLeft -= 50;
                            Console.WriteLine("Kazumi hit Boss by AoE");
                        }
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
                    claws.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, MathHelper.ToRadians(90) * Singleplayer.player.viewingDirection.X);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    claws.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    fireBall.drawIn2DWorld(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) * this.viewingDirection.X, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    fireBall.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + Arena.blickWinkel, 0);
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    danceOfFireFox.drawIn2DWorld(Vector3.One, 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    danceOfFireFox.drawInArena(Vector3.One, 0, 0, 0);
            }
            #endregion
        }
        #endregion
    }
}
