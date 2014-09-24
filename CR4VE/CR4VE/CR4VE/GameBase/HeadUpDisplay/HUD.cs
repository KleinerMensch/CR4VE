using System;
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

        public float spriteScale = 0.3f;
        public float continueSpriteScale = 0.05f;
        public float yOffset = 10f;
        public Texture2D redLiquid;

        public Vector2 liquidPosition;
        public bool powerIsDown = false;

        public int healthLeft, fullHealth;
        public int trialsLeft = 3;
        public bool isDead = false;
        public bool isSwimming = false;
        public bool isBurning = false;
        #endregion

        #region Constructor
        public HUD(ContentManager content, GraphicsDeviceManager manager)
        {
            graphics = manager;

            //initialize individual sprites
            Initialize(content);

            //initializing sprites that are the same for every character
            redLiquid = content.Load<Texture2D>("Assets/Sprites/HealthLiquidPrismatic");

            fullHealth = redLiquid.Height;
            healthLeft = fullHealth;
        }
        #endregion

        #region Methods
        public void Update()
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                if (healthLeft <= 0 && !GameControls.isGhost)
                {
                    Sounds.scream.Play();

                    trialsLeft -= 1;

                    GameControls.isGhost = true;

                    //refill health
                    healthLeft = fullHealth;
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                if (healthLeft <= 0 && trialsLeft > 0)
                {
                    trialsLeft -= 1;

                    //refill health
                    healthLeft = fullHealth;
                }
                else if (healthLeft <= 0 && trialsLeft == 0)
                {
                    trialsLeft -= 1;
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
            {
                if (healthLeft <= 0 && trialsLeft > 0)
                {
                    trialsLeft -= 1;

                    //refill health
                    healthLeft = fullHealth;
                }
                else if (healthLeft <= 0 && trialsLeft == 0)
                {
                    trialsLeft -= 1;
                }
            }

            if (trialsLeft < 0)
                isDead = true;
        }

        public virtual void Initialize(ContentManager content) { }
        public virtual void UpdateMana() { }
        public virtual void UpdateHealth() { }
        public virtual void UpdateLiquidPositionByResolution() { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
        #endregion
    }
}