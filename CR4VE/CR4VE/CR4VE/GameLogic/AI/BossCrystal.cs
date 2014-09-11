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
        Entity crystalShield, crystals;
        Vector3 currentViewingDirection;
        Vector3 currentCharacterPosition;
        public static Vector3 scaleForDrawMethod = new Vector3(0.03f, 0.03f, 0.03f);
        float speed = 1;
        float moveSpeed = -0.2f;
        bool playerHit = false;

        private static readonly TimeSpan meleeAttackTimer = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan rangedAttackTimer = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan specialAttackTimer = TimeSpan.FromSeconds(3);

        private TimeSpan timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);
        TimeSpan timeSpanForResettingMana = TimeSpan.FromSeconds(15);
        private TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);
        private TimeSpan lastAttack;
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
                scaleForDrawMethod = new Vector3(0.05f, 0.05f, 0.05f);
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
                viewingDirection = Arena.player.Position - this.Position;
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
                // Decrements the timespan
                timeSpan -= time.ElapsedGameTime;
                // If the timespan is equal or smaller time "0"
                if (timeSpan <= TimeSpan.Zero)
                {
                    timeSpan = TimeSpan.FromMilliseconds(270);
                    attackList.Remove(crystalShield);
                    launchedMelee = false;
                }
                #endregion

                #region Boss follows Player if Player too far away
                if (!Arena.sphere.Intersects(this.boundary))
                {
                    Vector3 playerPos = Arena.player.position;
                    Vector3 direction = this.position - playerPos;
                    float distance = direction.Length();

                    if (distance < 100)
                    {
                        direction.Normalize();
                        direction = moveSpeed * direction;
                        this.viewingDirection = direction;
                        Arena.blickwinkelBoss = (float)Math.Atan2(-Arena.boss.viewingDirection.Z, Arena.boss.viewingDirection.X);
                        this.move(direction);
                    }
                }
                #endregion

                #region UpdateMelee
                if (lastAttack + meleeAttackTimer < time.TotalGameTime && Arena.rangeOfMeleeFromBoss.Intersects(this.boundary))
                {
                    this.MeleeAttack(time);
                }
                #endregion

                #region Update Crystals From Rangedattack
                if (launchedRanged)
                {
                    crystals.move(speed * currentViewingDirection);

                    //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                    if (crystals.position != currentCharacterPosition + 50 * currentViewingDirection)
                    {
                        launchedRanged = true;
                        if (playerHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(crystals);
                        }
                        else
                        {
                            if (playerHit)
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
                                        playerHit = true;
                                        Console.WriteLine("Fractus hit Player by RangedAttack");
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
                    playerHit = false;
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
                    launchedSpecial = false;
                }
                #endregion

                foreach (Entity crystal in crystalList)
                {
                    crystal.boundary = new BoundingBox(crystal.position + new Vector3(-2, -2, -2), crystal.position + new Vector3(2, 2, 2));

                    foreach (Entity healthAbsorbingCrystal in crystalList)
                    {
                        if ((healthAbsorbingCrystal.position - Arena.player.position).Length() < 20 /*&& Arena.kazumiHud.healthleft > 0*/)
                        {
                            Console.WriteLine(/*Arena.kazumiHud.healthleft +*/ " Fractus hit enemy by SpecialAttack");
                            //Arena.kazumiHud.healthleft -= 0.01f;

                            //transfers health to Fractus in Arena
                            //if (Arena.fractusBossHud.trialsLeft <= 3 && Arena.fractusBossHud.healthLeft < Arena.fractusBossHud.fullHealth)
                            //    Arena.fractusBossHud.healthLeft += (int)(Arena.fractusBossHud.fullHealth * 0.01f);
                        }
                    }
                }
                #endregion

                #region Spawning SpecialAttack
                if (manaLeft >= 1.5f && crystalList.Count == 0)
                {
                    if ((lastAttack + lastAttack + lastAttack) + specialAttackTimer < time.TotalGameTime)
                    {
                        this.SpecialAttack(time);
                        lastAttack = time.TotalGameTime;
                        manaLeft -= 1.5f;
                    }
                }
                #endregion

                #region Spawning RangedAttack
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
            Vector3 crystalShieldPosition = this.position;
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
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft >= 1)
            {
                manaLeft -= 1;
                launchedRanged = true;

                currentCharacterPosition = this.Position;
                this.viewingDirection = Arena.player.Position - this.position;
                currentViewingDirection = this.viewingDirection;
                crystals = new Entity(this.position, "Enemies/skull", Arena.cont);
                crystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                attackList.Add(crystals);
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (manaLeft >= 1.5f)
            {
                manaLeft -= 1.5f;
                launchedSpecial = true;
                timeSpanForHealthAbsorbingCrystals = TimeSpan.FromSeconds(10);

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
                crystalShield.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                crystals.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90), 0);
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
