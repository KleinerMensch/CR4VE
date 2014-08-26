using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class Boss : Character
    {
        #region Attributes
        Vector3 offset = new Vector3(8, 8, 8);
        float speed = 1;
        float moveSpeed = -0.2f;
        float spawn = 0;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public Boss() : base() { }
        public Boss(Vector3 pos, String modelName, ContentManager cm) : base(pos,modelName,cm){ }
        public Boss(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound):base(pos,modelName,cm,bound) { }
        #endregion

        public override void Update(GameTime time)
        {
            
            Vector3 playerPos = Arena.player.position;
            Vector3 direction = this.position- playerPos;
            float distance = direction.Length();

            if (distance < 80)
            {
                
                direction.Normalize();
                direction = moveSpeed * direction;
                this.viewingDirection = direction;
                Arena.blickwinkelBoss = (float)Math.Atan2(-Arena.boss.viewingDirection.Z, Arena.boss.viewingDirection.X);
                this.position += direction;
                
            }
           
            this.MeleeAttack(time);
        }
    }
}
