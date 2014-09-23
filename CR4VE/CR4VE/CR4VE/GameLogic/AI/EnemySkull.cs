using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class EnemySkull : Enemy
    {
        #region Attributes
        Vector3 startPosition;
        Random random = new Random();
        float moveSpeed = -0.5f;

        float rotationX = 0.4f;
        float rotationY = MathHelper.ToRadians(0);
        bool checkedStartpositionOnce = false;
        
        #endregion

        #region Properties
        public override bool isDead
        {
            get { return this.hp <= 0; }
        }
        public override float Health
        {
            get { return this.hp; }
            set { this.hp = value; }
        }
        #endregion

        #region inherited Constructors
        public EnemySkull() : base() { }
        public EnemySkull(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public EnemySkull(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        public override void UpdateSingleplayer(GameTime gameTime)
        {
            if (!checkedStartpositionOnce)
            {
                startPosition = this.Position;
                checkedStartpositionOnce = true;
            }

            //Fake Rotationsanimation
            if (this.position.X > Singleplayer.player.position.X)
            {
                rotationY = MathHelper.ToRadians(-90);
                rotationX += 0.1f;
            }
            else
            {
                rotationY = MathHelper.ToRadians(90);
                rotationX -= 0.1f;
            }

            Vector3 playerPos = Singleplayer.player.position;
            Vector3 direction = this.position - playerPos; // für Richtungsvektor
            float distance = direction.Length();

            if (distance < 50 && !GameControls.isGhost)
            {
                direction.Normalize();
                direction = moveSpeed * direction;
                this.move(direction);
            }
            else
            {
                //do nothing
            }

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
                
            }
        }

        public override void Draw()
        {
            this.drawIn2DWorld(new Vector3(0.05f, 0.05f, 0.05f), 0, rotationY , rotationX);
        }
    }
}
