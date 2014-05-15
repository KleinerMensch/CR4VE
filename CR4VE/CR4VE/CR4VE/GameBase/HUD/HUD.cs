using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CR4VE
{
    class HUD
    {
        #region Attributes
        private GraphicsDeviceManager graphics;

        private Texture2D healthContainer, healthBar;
        private Texture2D powerContainer, powerBar;
        private SpriteFont font;

        private Vector2 healthBarPosition, powerBarPosition;
        private Vector2 trialsPosition;

        public float fullHealth, currentHealth;
        public float fullPower, currentPower;

        public int trialsLeft;
        #endregion

        #region Constructor
        public HUD(ContentManager content, GraphicsDeviceManager manager)
        {
            graphics = manager;
            LoadContent(content);

            healthBarPosition = new Vector2(10, 10);
            powerBarPosition = new Vector2(10, 10 + healthBar.Height + 5);
            trialsPosition = new Vector2(graphics.PreferredBackBufferWidth - 200, 10);

            fullHealth = healthBar.Width;
            currentHealth = fullHealth;
            fullPower = powerBar.Width;
            currentPower = fullPower;

            trialsLeft = 3;
        }
        #endregion

        #region Methods
        // Anzeige bisher nicht veraendert
        public void Update() { }

        private void LoadContent(ContentManager content)
        {
            healthContainer = content.Load<Texture2D>("Assets/Sprites/LifeContainer");
            healthBar = content.Load<Texture2D>("Assets/Sprites/LifeBar");

            powerContainer = content.Load<Texture2D>("Assets/Sprites/LifeContainer");
            powerBar = content.Load<Texture2D>("Assets/Sprites/LifeBar");

            font = content.Load<SpriteFont>("Assets/Fonts/HUDfont");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(healthBar, healthBarPosition, new Rectangle((int)healthBarPosition.X, (int)healthBarPosition.Y, (int)currentHealth, (int)healthBar.Height), Color.Red);
            spriteBatch.Draw(healthContainer, healthBarPosition, Color.White);

            spriteBatch.Draw(powerBar, powerBarPosition, new Rectangle((int)powerBarPosition.X, (int)powerBarPosition.Y, (int)currentPower, (int)powerBar.Height), Color.DarkTurquoise);
            spriteBatch.Draw(powerContainer, powerBarPosition, Color.White);

            spriteBatch.DrawString(font, "Continues left: " + trialsLeft.ToString(), trialsPosition, Color.White);
        }
        #endregion
    }
}
