using CR4VE.GameLogic.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.HeadUpDisplay
{
    class BossCrystalHUD : HUD
    {
        #region Attributes
        private Texture2D fractusContinue, fractusHealthContainer, fractusPowerLeftestCrystal, fractusPowerRightestCrystal, fractusPowerMiddleCrystalLeft, fractusPowerMiddleCrystalRight;
        private Vector2 fractusContinue1Position, fractusContinue2Position, fractusContinue3Position, fractusHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public BossCrystalHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            fractusContinue = content.Load<Texture2D>("Assets/Sprites/ContinueFractus");
            fractusHealthContainer = content.Load<Texture2D>("Assets/Sprites/FractusHPBar");
            fractusPowerLeftestCrystal = content.Load<Texture2D>("Assets/Sprites/FractusPowerLeftestCrystal");
            fractusPowerRightestCrystal = content.Load<Texture2D>("Assets/Sprites/FractusPowerRightestCrystal");
            fractusPowerMiddleCrystalLeft = content.Load<Texture2D>("Assets/Sprites/FractusPowerMiddleCrystalLeft");
            fractusPowerMiddleCrystalRight = content.Load<Texture2D>("Assets/Sprites/FractusPowerMiddleCrystalRight");
            #endregion

            fractusHealthContainerPosition = new Vector2(0,0);
            fractusContinue1Position = new Vector2(fractusHealthContainer.Width*spriteScale, yOffset);
            fractusContinue2Position = new Vector2(fractusContinue1Position.X + fractusContinue.Width * continueSpriteScale, yOffset);
            fractusContinue3Position = new Vector2(fractusContinue2Position.X + fractusContinue.Width * continueSpriteScale, yOffset);

            trialsLeft = 0;
        }

        public override void UpdateLiquidPositionByResolution()
        {
            if ((graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 16 / 9) || (graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 4 / 3))
                liquidPosition = fractusHealthContainerPosition + new Vector2(115f, 83.7f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(redLiquid, liquidPosition, new Rectangle(0, 0, redLiquid.Width, healthLeft), Color.White, MathHelper.ToRadians(180), new Vector2(redLiquid.Width / 2, redLiquid.Height / 2), spriteScale, SpriteEffects.None, 0);
            spriteBatch.Draw(fractusHealthContainer, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);

            #region Drawing current amount of Mana
            if (CharacterFractus.manaLeft == 3)
            {
                spriteBatch.Draw(fractusPowerLeftestCrystal, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerMiddleCrystalRight , fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerMiddleCrystalLeft, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerRightestCrystal, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterFractus.manaLeft > 2 && CharacterFractus.manaLeft < 3)
            {
                spriteBatch.Draw(fractusPowerLeftestCrystal, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerMiddleCrystalLeft, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerMiddleCrystalRight, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterFractus.manaLeft == 2)
            {
                spriteBatch.Draw(fractusPowerMiddleCrystalRight, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerMiddleCrystalLeft, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerRightestCrystal, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterFractus.manaLeft > 1 && CharacterFractus.manaLeft < 2)
            {
                spriteBatch.Draw(fractusPowerMiddleCrystalRight, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusPowerMiddleCrystalLeft, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterFractus.manaLeft == 1)
            {
                spriteBatch.Draw(fractusPowerMiddleCrystalLeft, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterFractus.manaLeft < 1 && CharacterFractus.manaLeft > 0)
            {
                spriteBatch.Draw(fractusPowerMiddleCrystalRight, fractusHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            #endregion

            #region Drawing current amount of Continues
            if (trialsLeft == 3)
            {
                spriteBatch.Draw(fractusContinue, fractusContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusContinue, fractusContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusContinue, fractusContinue3Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 2)
            {
                spriteBatch.Draw(fractusContinue, fractusContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(fractusContinue, fractusContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 1)
            {
                spriteBatch.Draw(fractusContinue, fractusContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            #endregion
        }
        #endregion
    }
}
