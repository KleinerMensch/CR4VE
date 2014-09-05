﻿using CR4VE.GameLogic.Characters;
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
    class FractusHUD : HUD
    {
        #region Attributes
        private Texture2D fractusHealthContainer, fractusPowerLeftestCrystal, fractusPowerRightestCrystal, fractusPowerMiddleCrystalLeft, fractusPowerMiddleCrystalRight;
        private Vector2 fractusHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public FractusHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            fractusHealthContainer = content.Load<Texture2D>("Assets/Sprites/FractusHPBar");
            fractusPowerLeftestCrystal = content.Load<Texture2D>("Assets/Sprites/FractusPowerLeftestCrystal");
            fractusPowerRightestCrystal = content.Load<Texture2D>("Assets/Sprites/FractusPowerRightestCrystal");
            fractusPowerMiddleCrystalLeft = content.Load<Texture2D>("Assets/Sprites/FractusPowerMiddleCrystalLeft");
            fractusPowerMiddleCrystalRight = content.Load<Texture2D>("Assets/Sprites/FractusPowerMiddleCrystalRight");
            #endregion

            fractusHealthContainerPosition = new Vector2(0,0);
        }

        public override void UpdateMana()
        {
            #region Singleplayer
            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region active Tilemap1
                for (int i = 0; i < Singleplayer.tileMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.tileMaps[ai1].PowerupList[i].type == "mana")
                    {
                        if (CharacterFractus.manaLeft < 3)
                        {
                            if (Singleplayer.tileMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.tileMaps[ai1].PowerupList.Remove(Singleplayer.tileMaps[ai1].PowerupList.ElementAt(i));
                                CharacterFractus.manaLeft += 1;
                            }
                        }
                    }
                }
                #endregion
                #region active Tilemap2
                for (int i = 0; i < Singleplayer.tileMaps[ai2].PowerupList.Count; i++)
                {
                    if (Singleplayer.tileMaps[ai2].PowerupList[i].type == "mana")
                    {
                        if (CharacterFractus.manaLeft < 3)
                        {
                            if (Singleplayer.tileMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.tileMaps[ai2].PowerupList.Remove(Singleplayer.tileMaps[ai2].PowerupList.ElementAt(i));
                                CharacterFractus.manaLeft += 1;
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
        }

        public override void UpdateLiquidPositionByResolution()
        {
            if ((graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 16 / 9) || (graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 4 / 3))
                liquidPosition = fractusHealthContainerPosition + new Vector2(114, 83.5f);
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
        }
        #endregion
    }
}
