using CR4VE.GameBase.Objects;
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
    class SeraphinHUD : HUD
    {
        #region Attributes
        private Texture2D seraphinHealthContainer, seraphinPowerRightEye, seraphinPowerLeftEye, seraphinPowerLowerEye;
        private Vector2 seraphinHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public SeraphinHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            seraphinHealthContainer = content.Load<Texture2D>("Assets/Sprites/SeraphinHPBar");
            seraphinPowerLeftEye = content.Load<Texture2D>("Assets/Sprites/SeraphinPower_LeftEye");
            seraphinPowerRightEye = content.Load<Texture2D>("Assets/Sprites/SeraphinPower_RightEye");
            seraphinPowerLowerEye = content.Load<Texture2D>("Assets/Sprites/SeraphinPower_LowerEye");
            #endregion

            seraphinHealthContainerPosition = new Vector2(graphics.PreferredBackBufferWidth - (seraphinHealthContainer.Width * spriteScale), 0);
        }

        public override void UpdateMana()
        {
            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region activeTilemap1
                for (int i = 0; i < Singleplayer.tileMaps[ai1].PowerupList.Count; i++)
                {
                    if (CharacterOphelia.manaLeft < 3)
                    {
                        if (Singleplayer.tileMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                        {
                            Singleplayer.tileMaps[ai1].PowerupList.Remove(Singleplayer.tileMaps[ai1].PowerupList.ElementAt(i));
                            CharacterOphelia.manaLeft += 1;
                        }
                    }
                }
                #endregion
                #region activeTilemap2
                for (int i = 0; i < Singleplayer.tileMaps[ai2].PowerupList.Count; i++)
                {
                    if (CharacterOphelia.manaLeft < 3)
                    {
                        if (Singleplayer.tileMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                        {
                            Singleplayer.tileMaps[ai2].PowerupList.Remove(Singleplayer.tileMaps[ai2].PowerupList.ElementAt(i));
                            CharacterOphelia.manaLeft += 1;
                        }
                    }
                }
                #endregion
            }
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
            spriteBatch.Draw(redLiquid, liquidPosition, new Rectangle(0, 0, redLiquid.Width, (int) (percentagedHealthLeft*redLiquid.Height)), Color.White, MathHelper.ToRadians(180), new Vector2(redLiquid.Width / 2, redLiquid.Height / 2), spriteScale, SpriteEffects.None, 0);
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
        }
        #endregion
    }
}
