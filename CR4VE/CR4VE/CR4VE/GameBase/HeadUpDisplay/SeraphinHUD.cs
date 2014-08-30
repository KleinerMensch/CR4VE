using CR4VE.GameBase.Objects;
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
        private Vector2 seraphinLiquidPosition;
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

            //Positionen hart gecoded -> hier anpassen !
            seraphinHealthContainerPosition = new Vector2(graphics.PreferredBackBufferWidth + 80, graphics.PreferredBackBufferHeight - graphics.PreferredBackBufferHeight);
            seraphinLiquidPosition = seraphinHealthContainerPosition + new Vector2(-399.5f, +147.5f);
        }

        public override void UpdateMana()
        {
            int ai1 = Singleplayer.activeIndex1;
            int ai2 = Singleplayer.activeIndex2;

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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(redLiquid, seraphinLiquidPosition, new Rectangle(0, 0, redLiquid.Width, healthLeft), Color.White, MathHelper.ToRadians(180), seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(seraphinHealthContainer, seraphinHealthContainerPosition, null, Color.White, 0f, seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            
            #region Drawing current amount of Mana
            if (CharacterSeraphin.manaLeft == 3)
            {
                spriteBatch.Draw(seraphinPowerRightEye, seraphinHealthContainerPosition, null, Color.White, 0f, seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinPowerLeftEye, seraphinHealthContainerPosition, null, Color.White, 0f, seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinPowerLowerEye, seraphinHealthContainerPosition, null, Color.White, 0f, seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            }
            if (CharacterSeraphin.manaLeft > 1 && CharacterSeraphin.manaLeft < 3)
            {
                spriteBatch.Draw(seraphinPowerLeftEye, seraphinHealthContainerPosition, null, Color.White, 0f, seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
                spriteBatch.Draw(seraphinPowerLowerEye, seraphinHealthContainerPosition, null, Color.White, 0f, seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            }
            if (CharacterSeraphin.manaLeft <= 1 && CharacterSeraphin.manaLeft > 0)
            {
                spriteBatch.Draw(seraphinPowerLowerEye, seraphinHealthContainerPosition, null, Color.White, 0f, seraphinHealthContainerPosition, 0.3f, SpriteEffects.None, 0);
            }
            #endregion
        }
        #endregion
    }
}
