using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.HeadUpDisplay
{
    class BossHellHUD : HUD
    {
        #region Attributes
        private Texture2D seraphinContinue, seraphinHealthContainer, seraphinPowerRightEye, seraphinPowerLeftEye, seraphinPowerLowerEye;
        private Vector2 seraphinContinue1Position, seraphinContinue2Position, seraphinContinue3Position, seraphinHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public BossHellHUD(ContentManager content, GraphicsDeviceManager manager) : base(content, manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            seraphinContinue = content.Load<Texture2D>("Assets/Sprites/ContinueSeraphin");
            seraphinHealthContainer = content.Load<Texture2D>("Assets/Sprites/SeraphinHPBar");
            seraphinPowerLeftEye = content.Load<Texture2D>("Assets/Sprites/SeraphinPower_LeftEye");
            seraphinPowerRightEye = content.Load<Texture2D>("Assets/Sprites/SeraphinPower_RightEye");
            seraphinPowerLowerEye = content.Load<Texture2D>("Assets/Sprites/SeraphinPower_LowerEye");
            #endregion

            seraphinHealthContainerPosition = new Vector2(graphics.PreferredBackBufferWidth - (seraphinHealthContainer.Width * spriteScale), 0);

            seraphinContinue1Position = new Vector2(graphics.PreferredBackBufferWidth - seraphinHealthContainer.Width * spriteScale - seraphinContinue.Width * continueSpriteScale, yOffset);
            seraphinContinue2Position = new Vector2(seraphinContinue1Position.X - seraphinContinue.Width * continueSpriteScale, yOffset);
            seraphinContinue3Position = new Vector2(seraphinContinue2Position.X - seraphinContinue.Width * continueSpriteScale, yOffset);

            trialsLeft = 0;
        }

        public override void UpdateLiquidPositionByResolution()
        {
            if ((graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 16 / 9) || (graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 4 / 3))
                liquidPosition = seraphinHealthContainerPosition + new Vector2(65, 83.5f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float percentagedHealthLeft = (float)healthLeft / (float)fullHealth;

            // Origin ist der Punkt, um den rotiert wird !
            spriteBatch.Draw(redLiquid, liquidPosition, new Rectangle(0, 0, redLiquid.Width, (int)(percentagedHealthLeft * redLiquid.Height)), Color.White, MathHelper.ToRadians(180), new Vector2(redLiquid.Width / 2, redLiquid.Height / 2), spriteScale, SpriteEffects.None, 0);
            spriteBatch.Draw(seraphinHealthContainer, seraphinHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);

            #region Drawing current amount of Mana
            if (CharacterSeraphin.manaLeft == 3 || BossHell.manaLeft == 3)
            {
                spriteBatch.Draw(seraphinPowerRightEye, seraphinHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinPowerLeftEye, seraphinHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinPowerLowerEye, seraphinHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterSeraphin.manaLeft > 1 && CharacterSeraphin.manaLeft < 3 || BossHell.manaLeft > 1 && BossHell.manaLeft < 3)
            {
                spriteBatch.Draw(seraphinPowerLeftEye, seraphinHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinPowerLowerEye, seraphinHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterSeraphin.manaLeft <= 1 && CharacterSeraphin.manaLeft > 0 || BossHell.manaLeft <= 1 && BossHell.manaLeft > 0)
            {
                spriteBatch.Draw(seraphinPowerLowerEye, seraphinHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            #endregion

            #region Drawing current amount of Continues
            if (trialsLeft == 3)
            {
                spriteBatch.Draw(seraphinContinue, seraphinContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinContinue, seraphinContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinContinue, seraphinContinue3Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 2)
            {
                spriteBatch.Draw(seraphinContinue, seraphinContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinContinue, seraphinContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 1)
            {
                spriteBatch.Draw(seraphinContinue, seraphinContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            #endregion
        }
        #endregion
    }
}
