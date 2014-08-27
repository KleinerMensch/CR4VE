﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CR4VE.GameLogic.Controls;
using CR4VE.GameBase.Camera;
using CR4VE.GameLogic;
using CR4VE.GameLogic.GameStates;
using CR4VE.GameBase.Objects.Terrain;

namespace CR4VE.GameBase.HeadUpDisplay
{
    class HUD
    {
        #region Attributes
        public GraphicsDeviceManager graphics;

        public Texture2D redLiquid;
        private SpriteFont font;

        private Vector2 trialsPosition;
        public bool powerIsDown = false;

        public int healthLeft, fullHealth;
        public int trialsLeft =3;
        public bool isDead = false;
        #endregion

        #region Constructor
        public HUD(ContentManager content, GraphicsDeviceManager manager)
        {
            graphics = manager;

            //initialize individual sprites
            Initialize(content);

            //initializing sprites that are the same for every character
            redLiquid = content.Load<Texture2D>("Assets/Sprites/HealthLiquidPrismatic");
            font = content.Load<SpriteFont>("Assets/Fonts/HUDfont");

            //trials spaeter anders visualisiert
            trialsPosition = new Vector2(graphics.PreferredBackBufferWidth - 200, 10);

            fullHealth = redLiquid.Height;
            healthLeft = fullHealth;
        }
        #endregion

        #region Methods
        public void Update()
        {
            if (healthLeft <= 0 && !GameControls.isGhost)
            {
                trialsLeft -= 1;

                GameControls.isGhost = true;

                //refill health
                healthLeft = fullHealth;
            }

            if (trialsLeft < 0)
                isDead = true;
        }

        public void DrawGenerals(SpriteBatch spriteBatch)
        {
            //for Singleplayer
            spriteBatch.DrawString(font, "Continues left: " + trialsLeft.ToString(), trialsPosition, Color.White);
        }

        public virtual void UpdateMana() { }
        public virtual void Initialize(ContentManager content) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
        #endregion
    }
}
