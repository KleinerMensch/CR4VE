using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.HeadUpDisplay
{
    class OpheliaHUD : HUD
    {
        #region Attributes
        private Texture2D opheliaHealthContainer, opheliaPower;
        public Color opheliaPowerColor;

        private Vector2 opheliaHealthContainerPosition;
        private Vector2 opheliaLiquidPosition;
        #endregion

        #region inherited Constructor
        public OpheliaHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void UpdateMana()
        {
            // Fadingeffekt
            if (opheliaPowerColor.A < 255 && powerIsDown == false)
                opheliaPowerColor.A += 1;
            else if (opheliaPowerColor.A == 255)
            {
                powerIsDown = true;
                opheliaPowerColor = new Color(0, 0, 0, 0);
            }
        }

        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            opheliaHealthContainer = content.Load<Texture2D>("Assets/Sprites/OpheliaHPBar");
            opheliaPower = content.Load<Texture2D>("Assets/Sprites/OpheliaPower");
            #endregion

            opheliaPowerColor = new Color(198, 226, 255, 0);

            opheliaHealthContainerPosition = new Vector2(0, graphics.PreferredBackBufferHeight - 90);
            opheliaLiquidPosition = opheliaHealthContainerPosition + new Vector2(178.5f, -86);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Healthbar von Ophelia
            // 3.Argument ist SourceRectangle -> null = ganzes Sprite wird gezeichnet
            // Teil der ausgeblendet wird hart reingecodet ->zu optimieren !
            //ophelias HUD
            spriteBatch.Draw(redLiquid, opheliaLiquidPosition, new Rectangle(0, 0, redLiquid.Width, healthLeft), Color.White, MathHelper.ToRadians(180), opheliaHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(opheliaHealthContainer, opheliaHealthContainerPosition, null, Color.White, 0f, opheliaHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(opheliaPower, opheliaHealthContainerPosition + new Vector2(0, 3), null, opheliaPowerColor, 0f, opheliaHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
        }
        #endregion
    }
}
