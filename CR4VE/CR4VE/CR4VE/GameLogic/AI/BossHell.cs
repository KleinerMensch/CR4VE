using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
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
        TimeSpan timeSpan = TimeSpan.FromSeconds(10);
        Vector3 offset = new Vector3(8, 8, 8);

        float moveSpeed = -0.2f;
        float minionSpeed = 3;
        float spawn = 0;

        private static readonly TimeSpan attackTimer = TimeSpan.FromSeconds(5);
        private TimeSpan lastAttack;

        //für die minionbewegung
        static float rTimer = 10.0f;
        static float chaseTimer = 5.0f;
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
            }
            else
            {
                //Update Leben vom Boss
                if (Arena.seraphinBossHUD.healthLeft < 0)
                    Arena.seraphinBossHUD.trialsLeft -= 1;

                // soll global angelegt werden, damit von allen klassen genutzt werden kann
                Random r = new Random();
                double help = r.NextDouble() * Math.PI * 2.0;
                Vector3 tmp = new Vector3((float)Math.Sin(help), (float)Math.Cos(help), 0);


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

                //else if (lastAttack + attackTimer < time.TotalGameTime)
                //{
                //    this.MeleeAttack(time);
                //    lastAttack = time.TotalGameTime;
                //}
                //Minion nur gespawned, wenn noch keiner da ist
                if (manaLeft > 0 && minionList.Count == 0)
                {
                    if (lastAttack + attackTimer < time.TotalGameTime)
                    {
                        this.RangedAttack(time);
                        lastAttack = time.TotalGameTime;
                        manaLeft -= 1;
                    }

                }
            }
        }

        public override void RangedAttack(GameTime time) {
            launchedRanged = true;
            minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Arena.cont));

            if (minionList.Count > 2)
                minionList.RemoveAt(0);
        }

        public override void DrawAttacks()
        {
            #region DrawRanged
            if (launchedRanged)
            {
                foreach (Entity minion in minionList)
                {
                    minion.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);
                }
            }
            #endregion
        }
    }
}
