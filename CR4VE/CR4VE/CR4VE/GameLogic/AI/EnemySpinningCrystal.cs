﻿using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameLogic.Controls;
using Microsoft.Xna.Framework.Input;

namespace CR4VE.GameLogic.AI
{
    class EnemySpinningCrystal : Enemy
    {
        #region Attributes
        Vector3 startPosition;
        Random random = new Random();
        float moveSpeed = -0.5f;
        float rotationX = 0.4f;
        float rotationY = MathHelper.ToRadians(-90);
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
        public EnemySpinningCrystal():base() { }
        public EnemySpinningCrystal(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public EnemySpinningCrystal(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        public override void UpdateSingleplayer(GameTime gameTime)
        {
            if (!checkedStartpositionOnce)
            {
                startPosition = this.Position;
                checkedStartpositionOnce = true;
            }

            rotationX -= 0.1f;

            Vector3 playerPos = Singleplayer.player.position;
            Vector3 direction = this.position - playerPos;
            float distance = direction.Length();

            if (distance < 50)
            {
                direction.Normalize();
                direction = moveSpeed * direction;
                this.move(direction);
            } else {
                //macht nichts, wenn man zu weit weg ist
            }

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
                
            }
        }

        public override void Draw()
        {
            this.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, rotationY + rotationX, 0);
        }
    }
}
