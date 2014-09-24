using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class BossHell : CharacterSeraphin
    {
        #region Attributes
        public new static List<Entity> minionList = new List<Entity>();
        public new List<Entity> meleeAttackList = new List<Entity>();
        Entity algaWhip;
        BoundingSphere rangeOfLaser;

        Vector3 currentCharacterPosition;
        Vector3 algaWhipPosition;
        Vector3 offset = new Vector3(5, 0, 5);

        bool playerHit = false;
        bool playerHitByMelee = false;
        bool listContainsAlgaWhip = false;

        float laserSpeed = 2;
        float moveSpeed = 0.2f;
        float minionSpeed = 0.5f;

        private static readonly TimeSpan meleeAttackTimer = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan rangedAttackTimer = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan specialAttackTimer = TimeSpan.FromSeconds(3);

        TimeSpan timeSpanForSpawningMinions = TimeSpan.FromSeconds(5);
        TimeSpan timeSpanForResettingMana = TimeSpan.FromSeconds(10);
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);
        private TimeSpan lastAttack;
        private bool soundPlayedDeath = false;
        private bool soundPlayed = false;
        private bool soundPlayedRanged = false;
        private bool soundPlayedSpecial = false;

        
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public BossHell() : base() { }
        public BossHell(Vector3 pos, String modelName, ContentManager cm) : base(pos,modelName,cm){ }
        public BossHell(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        #region Methods
        public override void Update(GameTime time)
        {
            #region Handling Death Of Boss
            if (Arena.seraphinBossHUD.isDead)
            {
                if (!soundPlayedDeath)
                {
                    Sounds.SeraphinScream.Play();

                    soundPlayedDeath = true;
                }

                this.model = Arena.cont.Load<Model>("Assets/Models/Players/skull");
                this.boundary = new BoundingBox(Vector3.Zero, Vector3.Zero);
                this.viewingDirection = new Vector3(0, 0, 1);

                #region Attacks Minions When Boss Is Dead
                launchedMelee = false;
                launchedRanged = false;
                launchedSpecial = false;
                attackList.Clear();
                meleeAttackList.Clear();
                #endregion
            }
            #endregion

            #region Boss while living
            else
            {
                //boss always looking at player
                viewingDirection = Arena.player.Position - this.Position;
                viewingDirection.Normalize();
                Arena.blickwinkelBoss = (float)Math.Atan2(-Arena.boss.viewingDirection.Z, Arena.boss.viewingDirection.X);

                #region UpdateContinuesFromBoss
                if (Arena.seraphinBossHUD.healthLeft < 0)
                    Arena.seraphinBossHUD.trialsLeft -= 1;
                #endregion

                ////soll global angelegt werden, damit von allen Klassen genutzt werden kann
                ////von Maria
                //Random r = new Random();
                //double help = r.NextDouble() * Math.PI * 2.0;
                //Vector3 tmp = new Vector3((float)Math.Sin(help), (float)Math.Cos(help), 0);

                #region ResetMana
                timeSpanForResettingMana -= time.ElapsedGameTime;
                if (timeSpanForResettingMana <= TimeSpan.Zero)
                {
                    manaLeft = 3;
                    timeSpanForResettingMana = TimeSpan.FromSeconds(10);
                }
                #endregion

                #region Boss follows Player if Player too far away
                if (!Arena.sphere.Intersects(this.boundary))
                {
                    Vector3 playerPos = Arena.player.position;
                    //Richtungsvektor = Zielposition - jetzige position
                    Vector3 direction = playerPos - this.position;
                    float distance = direction.Length();

                    if (distance < 100)
                    {
                        direction.Normalize();
                        direction = moveSpeed * direction;
                        this.move(direction);
                    }
                }
                #endregion

                #region MeleeAttack
                #region Timeupdate for DrawAttacks
                timeSpan -= time.ElapsedGameTime;
                if (timeSpan <= TimeSpan.Zero)
                {
                    timeSpan = TimeSpan.FromMilliseconds(270);
                    meleeAttackList.Clear();

                    launchedMelee = false;
                    listContainsAlgaWhip = false;
                }
                #endregion

                if (launchedMelee)
                {
                    algaWhipPosition = this.position + viewingDirection * offset;
                    algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Arena.cont);
                    algaWhip.boundary = new BoundingBox(algaWhipPosition + new Vector3(-2.5f, -2.5f, -2.5f), algaWhipPosition + new Vector3(2.5f, 2.5f, 2.5f));

                    if (!listContainsAlgaWhip)
                    {
                        meleeAttackList.Add(algaWhip);
                        listContainsAlgaWhip = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(algaWhip);
                    }

                    foreach (Entity seraphinsWhip in meleeAttackList)
                    {
                        if (playerHitByMelee)
                            break;
                        if (seraphinsWhip.boundary.Intersects(Arena.player.boundary))
                        {
                            Arena.opheliaHud.healthLeft -= 5;
                            playerHitByMelee = true;
                            Console.WriteLine("Boss hit Player by MeleeAttack");
                        }
                    }
                }
                //launching melee if player gets too near
                if (lastAttack + meleeAttackTimer < time.TotalGameTime && Arena.rangeOfMeleeFromBoss.Intersects(Arena.player.Boundary))
                {
                    this.MeleeAttack(time);
                    lastAttack = time.TotalGameTime;
                }
                #endregion

                #region Updating Minions from Ranged Attack
                #region Minions verschwinden nach 10 Sekunden
                timeSpanForSpawningMinions -= time.ElapsedGameTime;
                if (timeSpanForSpawningMinions <= TimeSpan.Zero && minionList.Count > 0)
                {
                    minionList.RemoveAt(0);
                    // Re initializes the timespan for the next time
                    // minion vanishes after 10 seconds
                    timeSpanForSpawningMinions = TimeSpan.FromSeconds(10);
                    launchedRanged = false;
                }
                #endregion

                foreach (Entity minion in minionList)
                {
                    #region Marias Code
                    //minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    //if (rTimer != 0)
                    //{
                    //    minion.position += tmp * minionSpeed;
                    //    rTimer -= (float)time.ElapsedGameTime.TotalSeconds;
                    //}

                    //if (chaseTimer != 0)
                    //{
                    //    Vector3 direction = minion.position - Arena.player.position;
                    //    direction.Normalize();
                    //    direction = moveSpeed * direction;
                    //    minion.position += direction * minionSpeed;
                    //    chaseTimer -= (float)time.ElapsedGameTime.TotalSeconds;
                    //}

                    //if (minion.boundary.Intersects(Arena.player.boundary))
                    //{
                    //    Arena.opheliaHud.healthLeft -= 1;
                    //    Console.WriteLine("Boss hit Player by RangedAttack");
                    //}
                    #endregion

                    minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    Vector3 direction = Arena.player.position - minion.position;
                    direction.Normalize();
                    direction = minionSpeed * direction;
                    minion.viewingDirection = direction;
                    minion.move(direction);

                    if (minion.boundary.Intersects(Arena.player.boundary))
                    {
                        Arena.opheliaHud.healthLeft -= 1;
                        Console.WriteLine("Boss hit enemy by RangedAttack");
                    }
                }

                //Minion nur gespawned, wenn noch keiner da ist
                if (manaLeft > 0 && minionList.Count == 0)
                {
                    if (lastAttack + lastAttack + rangedAttackTimer < time.TotalGameTime)
                    {
                        this.RangedAttack(time);
                        lastAttack = time.TotalGameTime;
                        manaLeft -= 1;
                    }
                }
                #endregion

                #region SpecialAttack
                #region UpdateLaserFromSpecialAttack
                if (launchedSpecial)
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Laser schießt nach vorn
                        attackList[i].move(laserSpeed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfLaser))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedSpecial = false;
                            break;
                        }
                        else
                        {
                            launchedSpecial = true;

                            if (attackList[i].boundary.Intersects(Arena.player.boundary))
                            {
                                Arena.opheliaHud.healthLeft -= 40;
                                playerHit = true;
                                Console.WriteLine("Boss hit Player by SpecialAttack");
                            }
                            //verschwindet auch bei Kollision
                            if (playerHit)
                            {
                                if (attackList.Count == 0)
                                    launchedSpecial = false;
                                attackList.Remove(attackList[i]);
                            }

                        }
                        playerHit = false;
                    }
                }
                #endregion

                if (manaLeft == 3 && lastAttack + specialAttackTimer < time.TotalGameTime)
                {
                    this.SpecialAttack(time);
                    lastAttack = time.TotalGameTime;
                    manaLeft -= 3;
                }
                #endregion
            }
            #endregion
        }

        public override void MeleeAttack(GameTime time)
        {
            launchedMelee = true;
            timeSpan = TimeSpan.FromMilliseconds(270);
            playerHitByMelee = false;
            soundPlayed = false;
            if (!soundPlayed)
            {
                Sounds.whip.Play();

                soundPlayed = true;
            }
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft > 0)
            {
                manaLeft -= 1;
                launchedRanged = true;
                timeSpanForSpawningMinions = TimeSpan.FromSeconds(5);
                soundPlayedRanged = false;
                if (!soundPlayedRanged)
                {
                    Sounds.spawn.Play();

                    soundPlayedRanged = true;
                }

                minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Arena.cont));

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
                currentCharacterPosition = this.position;
                rangeOfLaser = new BoundingSphere(currentCharacterPosition, 50);
                soundPlayedSpecial = false;
                if (!soundPlayedSpecial)
                {
                    Sounds.laser.Play();

                    soundPlayedSpecial = true;
                }

                Entity laser = new Entity(this.position, "Enemies/skull", Arena.cont);
                laser.viewingDirection = this.viewingDirection;
                laser.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                attackList.Add(laser);
            }
        }

        public override void DrawAttacks()
        {
            #region DrawMelee
            if (launchedMelee)
            {
                foreach (Entity whip in meleeAttackList)
                {
                    whip.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                }
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                foreach (Entity minion in minionList)
                {
                    float minionBlickwinkel = (float)Math.Atan2(-minion.viewingDirection.Z, minion.viewingDirection.X);
                    minion.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, MathHelper.ToRadians(90) + minionBlickwinkel, 0);
                }
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                foreach (Entity lazer in attackList)
                {
                    float laserBlickwinkel = (float)Math.Atan2(-lazer.viewingDirection.Z, lazer.viewingDirection.X);
                    lazer.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90) + laserBlickwinkel, 0);
                }
            }
            #endregion
        }
        #endregion
    }
}
