using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.HeadUpDisplay
{
    class KazumiHUD : HUD
    {
        #region Attributes
        private Texture2D kazumiHealthContainer, kazumiPowerLeftTail, kazumiPowerMiddleTail, kazumiPowerRightTail;
        private Vector2 kazumiHealthContainerPosition;
        private Vector2 kazumiLiquidPosition;
        #endregion

        #region inherited Constructor
        public KazumiHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void UpdateMana()
        {
            base.UpdateMana();
        }

        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            kazumiHealthContainer = content.Load<Texture2D>("Assets/Sprites/KazumiHPBar");
            kazumiPowerLeftTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_LeftTail");
            kazumiPowerMiddleTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_MiddleTail");
            kazumiPowerRightTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_RightTail");
            #endregion

            kazumiHealthContainerPosition = new Vector2(graphics.PreferredBackBufferWidth + 80, graphics.PreferredBackBufferHeight - 90);
            kazumiLiquidPosition = kazumiHealthContainerPosition + new Vector2(-399.5f, -86);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //kazumis HUD
            spriteBatch.Draw(redLiquid, kazumiLiquidPosition, new Rectangle(0, 0, redLiquid.Width, healthLeft), Color.White, MathHelper.ToRadians(180), kazumiHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(kazumiHealthContainer, kazumiHealthContainerPosition, null, Color.White, 0f, kazumiHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(kazumiPowerLeftTail, kazumiHealthContainerPosition, null, Color.White, 0f, kazumiHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(kazumiPowerMiddleTail, kazumiHealthContainerPosition, null, Color.White, 0f, kazumiHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(kazumiPowerRightTail, kazumiHealthContainerPosition, null, Color.White, 0f, kazumiHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
        }
        #endregion
    }
}
