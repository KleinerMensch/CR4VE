using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class BossCrystal : CharacterFractus
    {
        #region Attributes
        public new static List<Entity> crystalList = new List<Entity>();
        public new List<Entity> meleeAttackList = new List<Entity>();
        Entity crystalShield;
        Vector3 currentCharacterPosition;
        BoundingSphere rangeOfFlyingCrystals;

        float flyingSpeed = 10;
        float moveSpeed = 0.2f;

        bool playerHit = false;
        bool playerHitByMelee = false;
        bool listContainsCrystalShield = false;

        private static readonly TimeSpan meleeAttackTimer = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan rangedAttackTimer = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan specialAttackTimer = TimeSpan.FromSeconds(3);

        private TimeSpan timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);
        private TimeSpan timeSpanForResettingMana = TimeSpan.FromSeconds(15);
        private TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);
        private TimeSpan lastAttack;

        private bool soundPlayed = false;
        private bool soundPlayedRanged = false;
        private bool soundPlayedSpecial = false;
        private bool soundPlayedMinions = false;
        #endregion

        #region inherited Constructors
        public BossCrystal() : base() { }
        public BossCrystal(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public BossCrystal(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        #region Methods
        public override void Update(GameTime time)
        {
            #region Handling Death Of Boss
            if (Arena.fractusBossHUD.isDead)
            {
                this.model = Arena.cont.Load<Model>("Assets/Models/Players/skull");
                this.boundary = new BoundingBox(Vector3.Zero, Vector3.Zero);
                this.viewingDirection = new Vector3(0, 0, 1);

                #region Destroy Attacks When Boss Is Dead
                launchedMelee = false;
                launchedRanged = false;
                launchedSpecial = false;
                attackList.RemoveRange(0, attackList.Count);
                #endregion
            }
            #endregion

            #region Boss while living
            else
            {
                //boss is always looking at player
                viewingDirection = Arena.player.Position - this.Position;
                viewingDirection.Normalize();
                Arena.blickwinkelBoss = (float)Math.Atan2(-Arena.boss.viewingDirection.Z, Arena.boss.viewingDirection.X);

                #region UpdateContinuesFromBoss
                //Update Leben vom Boss
                if (Arena.fractusBossHUD.healthLeft < 0)
                    Arena.fractusBossHUD.trialsLeft -= 1;
                #endregion

                #region ResetMana
                timeSpanForResettingMana -= time.ElapsedGameTime;
                if (timeSpanForResettingMana <= TimeSpan.Zero)
                {
                    manaLeft = 3;
                    timeSpanForResettingMana = TimeSpan.FromSeconds(15);
                }
                #endregion

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

                #region Boss follows Player if Player too far away
                if (!Arena.sphere.Intersects(this.boundary))
                {
                    Vector3 playerPos = Arena.player.position;
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

                #region UpdateMelee
                if (launchedMelee)
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
                        if (playerHitByMelee)
                            break;
                        if (fractusShield.boundary.Intersects(Arena.player.boundary))
                        {
                            Arena.kazumiHud.healthLeft -= 5;
                            playerHitByMelee = true;
                            Console.WriteLine("Fractus hit Player by crystalShield");
                        }
                    }
                }
                //launching melee when player gets too near
                if (lastAttack + meleeAttackTimer < time.TotalGameTime && Arena.rangeOfMeleeFromBoss.Intersects(this.boundary))
                {
                    this.MeleeAttack(time);
                    lastAttack = time.TotalGameTime;
                }
                #endregion

                #region Spawning SpecialAttack
                #region Update HealthAbsorbingCrystals From Specialattack
                #region Removing Crystals After Defined Time
                timeSpanForHealthAbsorbingCrystals -= time.ElapsedGameTime;
                if (timeSpanForHealthAbsorbingCrystals <= TimeSpan.Zero && crystalList.Count > 0)
                {
                    crystalList.RemoveAt(0);
                    // Re initializes the timespan for the next time
                    // minion vanishes after 10 seconds
                    timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);
                    if (crystalList.Count == 0)
                        launchedSpecial = false;
                }
                #endregion

                foreach (Entity crystal in crystalList)
                {
                    crystal.boundary = new BoundingBox(crystal.position + new Vector3(-2, -2, -2), crystal.position + new Vector3(2, 2, 2));

                    foreach (Entity healthAbsorbingCrystal in crystalList)
                    {
                        if ((healthAbsorbingCrystal.position - Arena.player.position).Length() < 20 && Arena.kazumiHud.healthLeft > 0)
                        {
                            Console.WriteLine(" Fractus hit enemy by SpecialAttack");
                            Arena.kazumiHud.healthLeft -= 1;
                            soundPlayedMinions = false;
                            if (!soundPlayedMinions)
                            {
                                Sounds.minionsFraktus.Play();
                                soundPlayedMinions = true;
                            }

                            //transfers health to Fractus in Arena
                            if (Arena.fractusBossHUD.trialsLeft <= 3 && Arena.fractusBossHUD.healthLeft < Arena.fractusBossHUD.fullHealth)
                                Arena.fractusBossHUD.healthLeft += (int)(Arena.fractusBossHUD.fullHealth * 0.01f);
                        }
                    }
                }
                #endregion

                //if (manaLeft >= 1.5f && crystalList.Count == 0)
                //{
                //    if (lastAttack + specialAttackTimer < time.TotalGameTime)
                //    {
                //        this.SpecialAttack(time);
                //        lastAttack = time.TotalGameTime;
                //        manaLeft -= 1.5f;
                //    }
                //}
                this.SpecialAttack(time);
                #endregion

                #region Spawning RangedAttack
                #region Update Crystals From Rangedattack
                if (launchedRanged)
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Kristalle fliegen nach vorn
                        attackList[i].move(flyingSpeed * attackList[i].viewingDirection);

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

                            if (attackList[i].boundary.Intersects(Arena.player.boundary))
                            {
                                Arena.kazumiHud.healthLeft -= 3;
                                playerHit = true;
                                Console.WriteLine("Fractus hit Player by RangedAttack");
                            }
                            //verschwindet auch bei Kollision mit Gegner
                            if (playerHit)
                            {
                                if (attackList.Count == 0)
                                    launchedRanged = false;
                                attackList.Remove(attackList[i]);
                            }
                        }
                        playerHit = false;
                    }
                }
                #endregion
                if (manaLeft > 0 && lastAttack + rangedAttackTimer < time.TotalGameTime)
                {
                    this.RangedAttack(time);
                    lastAttack = time.TotalGameTime;
                    manaLeft -= 1;
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
                Sounds.freezingIce.Play();

                soundPlayed = true;
            }
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft >= 1)
            {
                manaLeft -= 1;
                launchedRanged = true;
                currentCharacterPosition = this.Position;
                rangeOfFlyingCrystals = new BoundingSphere(currentCharacterPosition, 50);
                soundPlayedRanged = false;
                if (!soundPlayedRanged)
                {
                    Sounds.freezingIce.Play();

                    soundPlayedRanged = true;
                }

                Entity flyingCrystals = new Entity(this.position, "Enemies/skull", Arena.cont);
                flyingCrystals.viewingDirection = viewingDirection;
                flyingCrystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                attackList.Add(flyingCrystals);
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (manaLeft >= 1.5f)
            {
                manaLeft -= 1.5f;
                launchedSpecial = true;
                timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);
                soundPlayedSpecial = false;
                if (!soundPlayedSpecial)
                {
                    Sounds.spawn.Play();

                    soundPlayedSpecial = true;
                }

                crystalList.Add(new Entity(this.position, "Enemies/enemySpinningNoAnim", CR4VE.GameLogic.GameStates.Arena.cont));

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
                foreach (Entity shield in meleeAttackList)
                {
                    shield.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                }
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                foreach (Entity fractusCrystals in attackList)
                {
                    float crystalBlickwinkel = (float)Math.Atan2(-fractusCrystals.viewingDirection.Z, fractusCrystals.viewingDirection.X);
                    fractusCrystals.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90) + crystalBlickwinkel, 0);
                }
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
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
}
