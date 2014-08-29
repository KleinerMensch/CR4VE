using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        float minionSpeed = 100;
        float spawn = 0;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public BossHell() : base() { }
        public BossHell(Vector3 pos, String modelName, ContentManager cm) : base(pos,modelName,cm){ }
        public BossHell(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound):base(pos,modelName,cm,bound) { }
        #endregion

        public override void Update(GameTime time)
        {
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

                Vector3 direction =  minion.position - Arena.player.position;
                direction.Normalize();
                direction = moveSpeed * direction;
                minion.position += direction*minionSpeed;
                
                if (minion.boundary.Intersects(Arena.player.boundary))
                {
                    Arena.hud.healthLeft -= (int) 0.01f;
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

            this.RangedAttack(time);

            ////Minion nur gespawned, wenn noch keiner da ist
            //if (manaLeft > 0 && minionList.Count == 0)
            //    this.RangedAttack(time);
        }

        public override void RangedAttack(GameTime time) {
            launchedRanged = true;
            minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Arena.cont));

            if (minionList.Count > 3)
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
