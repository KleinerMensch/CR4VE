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

        private Texture2D opheliaHealthContainer, opheliaHealthBar;
        private SpriteFont font;

        private Vector2 opheliaBarPosition, trialsPosition;

        public int trialsLeft;
        #endregion

        #region Constructor
        public HUD(ContentManager content, GraphicsDeviceManager manager)
        {
            graphics = manager;
            LoadContent(content);

            opheliaBarPosition = new Vector2(0,graphics.PreferredBackBufferHeight-90);
            trialsPosition = new Vector2(graphics.PreferredBackBufferWidth - 200, 10);

            trialsLeft = 3;
        }
        #endregion

        #region Methods
        // Anzeige bisher nicht veraendert
        public void Update() { }

        private void LoadContent(ContentManager content)
        {
            opheliaHealthContainer = content.Load<Texture2D>("Assets/Sprites/OpheliaHPBar");
            opheliaHealthBar = content.Load<Texture2D>("Assets/Sprites/HealthLiquid");

            font = content.Load<SpriteFont>("Assets/Fonts/HUDfont");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Healthbar von Ophelia
            // 3.Argument ist SourceRectangle -> null = ganzes Sprite wird gezeichnet
            // Teil der ausgeblendet wird noch falsch !
            spriteBatch.Draw(opheliaHealthBar, opheliaBarPosition, new Rectangle(0,0,opheliaHealthBar.Width, (int)(opheliaHealthBar.Height*0.5)), Color.White, 0f, opheliaBarPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(opheliaHealthContainer,opheliaBarPosition,null,Color.White,0f,opheliaBarPosition,0.3f,SpriteEffects.None,0);

            spriteBatch.DrawString(font, "Continues left: " + trialsLeft.ToString(), trialsPosition, Color.White);
        }
        #endregion
    }
}
