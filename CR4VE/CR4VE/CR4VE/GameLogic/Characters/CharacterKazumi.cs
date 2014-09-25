using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.Controls;
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
        Vector3 offset = new Vector3(4,4,4);

        float speed = 1;

        bool enemyHit = false;
        bool enemyHitByMelee = false;
        bool listContainsClaws = false;

        //Sounds
        private bool soundPlayed = false;
        private bool soundPlayedRanged = false;
        private bool soundPlayedEnemy = false;
        private bool soundPlayedSpecial = false;
        #endregion

        #region Properties
        public override String CharacterType
        {
            get { return "Kazumi"; }
        }
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
                    claws = new Entity(clawsPosition, "kazumisClaws", Singleplayer.cont);
                    claws.boundary = new BoundingBox(clawsPosition + new Vector3(-3f, -6f, -4f), clawsPosition + new Vector3(3f, 6f, 4f));

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
                                soundPlayedEnemy = false;
                                if (!soundPlayedEnemy)
                                {
                                    Sounds.punch.Play();

                                    soundPlayedEnemy = true;
                                }
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
                                soundPlayedEnemy = false;
                                if (!soundPlayedEnemy)
                                {
                                    Sounds.punch.Play();

                                    soundPlayedEnemy = true;
                                }
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
                    claws = new Entity(clawsPosition, "kazumisClaws", Arena.cont);
                    claws.boundary = new BoundingBox(clawsPosition + new Vector3(-2.5f, -6f, -2.5f), clawsPosition + new Vector3(2.5f, 2.5f, 2.5f));
                    
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
                            Arena.fractusBossHUD.healthLeft -= 8;
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
                    claws = new Entity(clawsPosition, "kazumisClaws", Multiplayer.cont);
                    claws.boundary = new BoundingBox(clawsPosition + new Vector3(-2.5f, -6f, -2.5f), clawsPosition + new Vector3(2.5f, 6f, 2.5f));

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
                        for (int i = 0; i < GameControls.ConnectedControllers; i++)
                        {
                            if (kazumisClaws.boundary.Intersects(Multiplayer.Players[i].boundary) && Multiplayer.Players[i].CharacterType != "Kazumi")
                            {
                                enemyHitByMelee = true;
                                Console.WriteLine("Kazumi hit " + Multiplayer.Players[i] + " by MeleeAttack");
                                Multiplayer.hudArray[i].healthLeft -= 8;
                            }
                        }
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
                                Sounds.fireball.Play();

                                soundPlayedRanged = true;
                            }

                            #region enemyList1
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                            {
                                if (attackList.Count > 0)
                                {
                                    if (attackList[i].boundary.Intersects(enemy.boundary))
                                    {
                                        enemy.hp -= 2;
                                        enemyHit = true;
                                        soundPlayedEnemy = false;
                                        if (!soundPlayedEnemy)
                                        {
                                            Sounds.punch.Play();

                                            soundPlayedEnemy = true;
                                        }
                                        Console.WriteLine("Kazumi hit enemy by RangedAttack");
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
                                        soundPlayedEnemy = false;
                                        if (!soundPlayedEnemy)
                                        {
                                            Sounds.punch.Play();

                                            soundPlayedEnemy = true;
                                        }
                                        Console.WriteLine("Kazumi hit enemy by RangedAttack");
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
                                Sounds.fireball.Play();

                                soundPlayedRanged = true;
                            }

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
                        //Feuerball fliegt nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfFireBall))
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
                                Sounds.fireball.Play();

                                soundPlayedRanged = true;
                            }
                            for (int j = 0; j < GameControls.ConnectedControllers; j++)
                            {
                                if (attackList[i].boundary.Intersects(Multiplayer.Players[j].boundary) && Multiplayer.Players[j].CharacterType != "Kazumi")
                                {
                                    enemyHit = true;
                                    Console.WriteLine("Kazumi hit "+ Multiplayer.Players[j] +" by RangedAttack");
                                    Multiplayer.hudArray[j].healthLeft -= 40;
                                }
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
                Sounds.claws.Play();

                soundPlayed = true;
            }
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
                    Entity fireBall = new Entity(this.position, "kazumisFireball", Singleplayer.cont);
                    fireBall.viewingDirection = viewingDirection;
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -6, -3), this.position + new Vector3(3, 6, 3));
                    attackList.Add(fireBall);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    Entity fireBall = new Entity(this.position, "kazumisFireball", Arena.cont);
                    fireBall.viewingDirection = viewingDirection;
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-2, -6, -5f), this.position + new Vector3(2, 6, 5f));
                    attackList.Add(fireBall);
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    Entity fireBall = new Entity(this.position, "kazumisFireball", Multiplayer.cont);
                    fireBall.viewingDirection = viewingDirection;
                    fireBall.boundary = new BoundingBox(this.position + new Vector3(-2, -6, -5f), this.position + new Vector3(2, 6, 5f));
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
                soundPlayedSpecial = false;
                if (!soundPlayedSpecial)
                {
                    Sounds.fireball.Play();

                    soundPlayedSpecial = true;
                }
                timeSpan = TimeSpan.FromMilliseconds(270);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    danceOfFireFox = new Entity(this.Position, "kazumisAoE", Singleplayer.cont);
                    danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -10, -20), this.position + new Vector3(20, 40, 20));
                    aoeList.Add(danceOfFireFox);

                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity kazumisDanceOfFirefox in aoeList)
                        {
                            if (kazumisDanceOfFirefox.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 3;
                                soundPlayedEnemy = false;
                                soundPlayedSpecial = false;
                                if (!soundPlayedEnemy)
                                {
                                    Sounds.punch.Play();

                                    soundPlayedEnemy = true;
                                }
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
                                soundPlayedEnemy = false;
                                soundPlayedSpecial = false;
                                if (!soundPlayedEnemy)
                                {
                                    Sounds.punch.Play();

                                    soundPlayedEnemy = true;
                                }
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
                    danceOfFireFox = new Entity(this.Position, "kazumisAoE", Arena.cont);
                    danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -10, -20), this.position + new Vector3(20, 60, 20));
                    aoeList.Add(danceOfFireFox);

                    foreach (Entity kazumisDanceOfFirefox in aoeList)
                    {
                        if (kazumisDanceOfFirefox.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.fractusBossHUD.healthLeft -= 50;
                            soundPlayedSpecial = false;
                            Console.WriteLine("Kazumi hit Boss by AoE");
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    danceOfFireFox = new Entity(this.Position, "kazumisAoE", Multiplayer.cont);
                    danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -10, -20), this.position + new Vector3(20, 60, 20));
                    aoeList.Add(danceOfFireFox);

                    foreach (Entity kazumisDanceOfFirefox in aoeList)
                    {
                        for (int i = 0; i < GameControls.ConnectedControllers; i++)
                        {
                            if (kazumisDanceOfFirefox.boundary.Intersects(Multiplayer.Players[i].boundary) && Multiplayer.Players[i].CharacterType != "Kazumi")
                            {
                                Console.WriteLine("Kazumi hit " + Multiplayer.Players[i] + " by Special");
                                Multiplayer.hudArray[i].healthLeft -= 50;
                            }
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
                {
                    foreach (Entity claw in meleeAttackList)
                    {
                        claw.drawIn2DWorld(new Vector3(0.75f, 0.75f, 0.75f), 0, viewingDirection.X, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity claw in meleeAttackList)
                    {
                        float clawBlickwinkel = (float)Math.Atan2(-viewingDirection.Z, viewingDirection.X);
                        claw.drawInArena(new Vector3(0.6f, 0.6f, 0.6f), 0, MathHelper.ToRadians(90) + clawBlickwinkel, 0);
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
                        ball.drawIn2DWorld(new Vector3(0.6f, 0.6f, 0.6f), 0, MathHelper.ToRadians(90) * ball.viewingDirection.X, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity ball in attackList)
                    {
                        float ballBlickwinkel = (float)Math.Atan2(-ball.viewingDirection.Z, ball.viewingDirection.X);
                        ball.drawInArena(new Vector3(0.35f, 0.35f, 0.35f), 0, MathHelper.ToRadians(90) + ballBlickwinkel, 0);
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
