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
        Entity algaWhip, laser;
        Vector3 currentViewingDirection;
        TimeSpan timeSpan = TimeSpan.FromSeconds(10);
        TimeSpan timeSpanForResettingMana = TimeSpan.FromSeconds(15);
        Vector3 offset = new Vector3(50, 50, 50);
        bool playerHit = false;

        float laserSpeed = 0.5f;
        float moveSpeed = -0.2f;
        float minionSpeed = 3;
        float spawn = 0;

        private static readonly TimeSpan meleeAttackTimer = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan rangedAttackTimer = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan specialAttackTimer = TimeSpan.FromSeconds(3);

        private TimeSpan lastAttack;

        //fuer die Minionbewegung
        static float rTimer = 10.0f;
        static float chaseTimer = 5.0f;

        public Vector3 scaleForDrawMethod = new Vector3(0.75f, 0.75f, 0.75f);
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public BossHell() : base() { }
        public BossHell(Vector3 pos, String modelName, ContentManager cm) : base(pos,modelName,cm){ }
        public BossHell(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        public override void Update(GameTime time)
        {
            if (Arena.seraphinBossHUD.isDead)
            {
                this.model = Arena.cont.Load<Model>("Assets/Models/Players/skull");
                this.boundary = new BoundingBox(Vector3.Zero, Vector3.Zero);
                scaleForDrawMethod = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else
            {
                //Update Leben vom Boss
                if (Arena.seraphinBossHUD.healthLeft < 0)
                    Arena.seraphinBossHUD.trialsLeft -= 1;

                // soll global angelegt werden, damit von allen Klassen genutzt werden kann
                Random r = new Random();
                double help = r.NextDouble() * Math.PI * 2.0;
                Vector3 tmp = new Vector3((float)Math.Sin(help), (float)Math.Cos(help), 0);

                #region ResetMana
                timeSpanForResettingMana -= time.ElapsedGameTime;
                // If the timespan is equal or smaller time "0"
                if (timeSpanForResettingMana <= TimeSpan.Zero)
                {
                    manaLeft = 3;
                    timeSpanForResettingMana = TimeSpan.FromSeconds(5);
                }
                #endregion

                #region UpdateLaserFromSpecialAttack
                if (launchedSpecial)
                {
                    laser.position += laserSpeed * currentViewingDirection;
                    laser.boundary.Min += laserSpeed * currentViewingDirection;
                    laser.boundary.Max += laserSpeed * currentViewingDirection;

                    //Laser verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                    if (laser.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedSpecial = true;
                        if (playerHit)
                        {
                            launchedSpecial = false;
                            attackList.Remove(laser);
                        }
                        else
                        {
                            if (playerHit)
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
                                        Arena.opheliaHud.healthLeft -= 1;
                                        playerHit = true;
                                        Console.WriteLine("Boss hit Player by SpecialAttack");
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
                    playerHit = false;
                }
                #endregion

                #region Minions verschwinden nach 10 Sekunden
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

                #region Updating Minionsposition
                foreach (Entity minion in minionList)
                {
                    minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    if (rTimer != 0)
                    {
                        minion.position += tmp * minionSpeed;
                        rTimer -= (float)time.ElapsedGameTime.TotalSeconds;
                    }

                    if (chaseTimer != 0)
                    {
                        Vector3 direction = minion.position - Arena.player.position;
                        direction.Normalize();
                        direction = moveSpeed * direction;
                        minion.position += direction * minionSpeed;
                        chaseTimer -= (float)time.ElapsedGameTime.TotalSeconds;
                    }

                    if (minion.boundary.Intersects(Arena.player.boundary))
                    {
                        Arena.opheliaHud.healthLeft -= 1;
                        Console.WriteLine("Boss hit Player by RangedAttack");
                    }
                }
                #endregion

                #region Boss follows Player
                if (!Arena.sphere.Intersects(Arena.boss.boundary))
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

                else if (lastAttack + meleeAttackTimer < time.TotalGameTime)
                {
                    this.MeleeAttack(time);
                    lastAttack = time.TotalGameTime;
                }

                if (manaLeft == 3 && lastAttack + specialAttackTimer < time.TotalGameTime)
                {
                    this.SpecialAttack(time);
                    lastAttack = time.TotalGameTime;
                    manaLeft -= 3;
                }

                //Minion nur gespawned, wenn noch keiner da ist
                if (manaLeft > 0 && minionList.Count == 0)
                {
                    if (lastAttack + rangedAttackTimer < time.TotalGameTime)
                    {
                        this.RangedAttack(time);
                        lastAttack = time.TotalGameTime;
                        manaLeft -= 1;
                    }
                }
            }
        }

        public override void MeleeAttack(GameTime time)
        {
            launchedMelee = true;

            Vector3 algaWhipPosition = this.position + viewingDirection * offset;
            algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Arena.cont);
            algaWhip.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3) + viewingDirection * offset, this.position + new Vector3(3, 3, 3) + viewingDirection * offset);
            attackList.Add(algaWhip);

            foreach (Entity seraphinsWhip in attackList)
            {
                if (seraphinsWhip.boundary.Intersects(Arena.player.boundary))
                {
                    Arena.opheliaHud.healthLeft -= 1;
                    Console.WriteLine("Boss hit Player by MeleeAttack");
                }
            }
        }

        public override void RangedAttack(GameTime time) {
            launchedRanged = true;
            minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Arena.cont));

            if (minionList.Count > 2)
                minionList.RemoveAt(0);
        }

        public override void SpecialAttack(GameTime time)
        {
            //Laser kann nur bei vollem Mana benutzt werden
            if (manaLeft == 3)
            {
                manaLeft -= 3;
                launchedSpecial = true;

                currentViewingDirection = Arena.player.Position - this.position;
                laser = new Entity(this.position, "Enemies/skull", Arena.cont);
                laser.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                attackList.Add(laser);
            }
        }

        public override void DrawAttacks()
        {
            #region DrawMelee
            if (launchedMelee)
            {
                algaWhip.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                launchedMelee = false;
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                foreach (Entity minion in minionList)
                {
                    minion.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);
                }
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                laser.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
            }
            #endregion
        }
    }
}
