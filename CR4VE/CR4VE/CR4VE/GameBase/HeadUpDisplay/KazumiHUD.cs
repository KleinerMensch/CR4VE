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
    class KazumiHUD : HUD
    {
        #region Attributes
        private Texture2D kazumiHealthContainer, kazumiPowerLeftTail, kazumiPowerMiddleTail, kazumiPowerRightTail;
        private Vector2 kazumiHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public KazumiHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            kazumiHealthContainer = content.Load<Texture2D>("Assets/Sprites/KazumiHPBar");
            kazumiPowerLeftTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_LeftTail");
            kazumiPowerMiddleTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_MiddleTail");
            kazumiPowerRightTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_RightTail");
            #endregion

            kazumiHealthContainerPosition = new Vector2(graphics.PreferredBackBufferWidth - (kazumiHealthContainer.Width * spriteScale), graphics.PreferredBackBufferHeight - (kazumiHealthContainer.Height * spriteScale));
        }

        public override void UpdateMana()
        {
            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region active Tilemap1
                for (int i = 0; i < Singleplayer.tileMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.tileMaps[ai1].PowerupList[i].type == "mana")
                    {
                        if (CharacterKazumi.manaLeft < 3)
                        {
                            if (Singleplayer.tileMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.tileMaps[ai1].PowerupList.Remove(Singleplayer.tileMaps[ai1].PowerupList.ElementAt(i));
                                CharacterKazumi.manaLeft += 1;
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
                        if (CharacterKazumi.manaLeft < 3)
                        {
                            if (Singleplayer.tileMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.tileMaps[ai2].PowerupList.Remove(Singleplayer.tileMaps[ai2].PowerupList.ElementAt(i));
                                CharacterKazumi.manaLeft += 1;
                            }
                        }
                    }
                }
                #endregion
            }
        }

        public override void UpdateHealth()
        {
            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region active Tilemap1
                for (int i = 0; i < Singleplayer.tileMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.tileMaps[ai1].PowerupList[i].type == "health")
                    {
                        if (Singleplayer.hud.healthLeft <= Singleplayer.hud.fullHealth)
                        {
                            if (Singleplayer.tileMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                if (Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                {
                                    Singleplayer.hud.healthLeft += Singleplayer.tileMaps[ai1].PowerupList[i].amount;

                                    if (Singleplayer.hud.fullHealth < Singleplayer.hud.healthLeft)
                                        Singleplayer.hud.healthLeft = Singleplayer.hud.fullHealth;

                                    Singleplayer.tileMaps[ai1].PowerupList.Remove(Singleplayer.tileMaps[ai1].PowerupList.ElementAt(i));
                                }
                            }
                        }
                    }
                }
                #endregion
                #region active Tilemap2
                for (int i = 0; i < Singleplayer.tileMaps[ai2].PowerupList.Count; i++)
                {
                    if (Singleplayer.tileMaps[ai2].PowerupList[i].type == "health")
                    {
                        if (Singleplayer.hud.healthLeft <= Singleplayer.hud.fullHealth)
                        {
                            if (Singleplayer.tileMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                if (Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                {
                                    Singleplayer.hud.healthLeft += Singleplayer.tileMaps[ai2].PowerupList[i].amount;

                                    if (Singleplayer.hud.fullHealth < Singleplayer.hud.healthLeft)
                                        Singleplayer.hud.healthLeft = Singleplayer.hud.fullHealth;

                                    Singleplayer.tileMaps[ai2].PowerupList.Remove(Singleplayer.tileMaps[ai2].PowerupList.ElementAt(i));
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }

        public override void UpdateLiquidPositionByResolution()
        {
            if ((graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 16 / 9) || (graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 4 / 3))
                liquidPosition = kazumiHealthContainerPosition + new Vector2(65.2f, 156.5f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(redLiquid, liquidPosition, new Rectangle(0, 0, redLiquid.Width, healthLeft), Color.White, MathHelper.ToRadians(180), new Vector2(redLiquid.Width / 2, redLiquid.Height / 2), spriteScale, SpriteEffects.None, 0);
            spriteBatch.Draw(kazumiHealthContainer, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);

            #region Drawing current amount of Mana
            if (CharacterKazumi.manaLeft == 3)
            {
                spriteBatch.Draw(kazumiPowerLeftTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(kazumiPowerMiddleTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(kazumiPowerRightTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterKazumi.manaLeft > 1 && CharacterKazumi.manaLeft < 3)
            {
                spriteBatch.Draw(kazumiPowerLeftTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(kazumiPowerMiddleTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            if (CharacterKazumi.manaLeft <= 1 && CharacterKazumi.manaLeft > 0)
            {
                spriteBatch.Draw(kazumiPowerMiddleTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
            #endregion
        }
        #endregion
    }
}
