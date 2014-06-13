using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CR4VE.GameLogic.Controls;

namespace CR4VE
{
    class HUD
    {
        #region Attributes
        private GraphicsDeviceManager graphics;

        private Texture2D opheliaHealthContainer, redLiquid, opheliaPower;
        private SpriteFont font;
        public Color opheliaPowerColor;

        private Vector2 opheliaBarPosition, trialsPosition, liquidposition;
        private bool powerIsDown = false;

        public int healthLeft, fullHealth;
        public int trialsLeft;
        #endregion

        #region Constructor
        public HUD(ContentManager content, GraphicsDeviceManager manager)
        {
            graphics = manager;
            LoadContent(content);

            fullHealth = redLiquid.Height;
            healthLeft = fullHealth;

            opheliaPowerColor = new Color(198,226,255, 0);

            opheliaBarPosition = new Vector2(0, graphics.PreferredBackBufferHeight - 90);
            liquidposition = opheliaBarPosition + new Vector2(178.5f,-86);
            trialsPosition = new Vector2(graphics.PreferredBackBufferWidth - 200, 10);

            trialsLeft = 3;
        }
        #endregion

        #region Methods
        public void Update()
        {
            // fuer erste Tests
            if(CR4VE.GameLogic.Controls.KeyboardControls.isPressed(Microsoft.Xna.Framework.Input.Keys.Down))
                healthLeft -= (int)(fullHealth*0.01);
            // Fadingeffekt
            if(opheliaPowerColor.A < 255 && powerIsDown == false)
                opheliaPowerColor.A += 1;
            else if (opheliaPowerColor.A == 255)
            {
                powerIsDown = true;
                opheliaPowerColor = new Color(0, 0, 0, 0);
            }
        }

        private void LoadContent(ContentManager content)
        {
            opheliaHealthContainer = content.Load<Texture2D>("Assets/Sprites/OpheliaHPBar");
            redLiquid = content.Load<Texture2D>("Assets/Sprites/HealthLiquidPrismatic");
            opheliaPower = content.Load<Texture2D>("Assets/Sprites/OpheliaPower");

            font = content.Load<SpriteFont>("Assets/Fonts/HUDfont");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Healthbar von Ophelia
            // 3.Argument ist SourceRectangle -> null = ganzes Sprite wird gezeichnet
            // Teil der ausgeblendet wird hart reingecodet ->zu optimieren ! 
            spriteBatch.Draw(redLiquid, liquidposition, new Rectangle(0, 0, redLiquid.Width, healthLeft), Color.White, MathHelper.ToRadians(180), opheliaBarPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(opheliaHealthContainer,opheliaBarPosition,null,Color.White,0f,opheliaBarPosition,0.3f,SpriteEffects.None,0);
            spriteBatch.Draw(opheliaPower, opheliaBarPosition+new Vector2(0,3), null, opheliaPowerColor, 0f, opheliaBarPosition, 0.3f, SpriteEffects.None, 0);
            
            spriteBatch.DrawString(font, "Continues left: " + trialsLeft.ToString(), trialsPosition, Color.White);
        }
        #endregion
    }
}
