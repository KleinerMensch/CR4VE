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
        private Texture2D kazumiContinue, kazumiHealthContainer, kazumiPowerLeftTail, kazumiPowerMiddleTail, kazumiPowerRightTail;
        private Vector2 kazumiContinue1Position, kazumiContinue2Position, kazumiContinue3Position, kazumiHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public KazumiHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            kazumiContinue = content.Load<Texture2D>("Assets/Sprites/ContinueKazumi");
            kazumiHealthContainer = content.Load<Texture2D>("Assets/Sprites/KazumiHPBar");
            kazumiPowerLeftTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_LeftTail");
            kazumiPowerMiddleTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_MiddleTail");
            kazumiPowerRightTail = content.Load<Texture2D>("Assets/Sprites/KazumiPower_RightTail");
            #endregion

            kazumiHealthContainerPosition = new Vector2(graphics.PreferredBackBufferWidth - (kazumiHealthContainer.Width * spriteScale), graphics.PreferredBackBufferHeight - (kazumiHealthContainer.Height * spriteScale));

            kazumiContinue1Position = new Vector2(kazumiHealthContainerPosition.X - kazumiContinue.Width * continueSpriteScale, graphics.PreferredBackBufferHeight - kazumiContinue.Height * continueSpriteScale - yOffset);
            kazumiContinue2Position = new Vector2(kazumiContinue1Position.X - kazumiContinue.Width*continueSpriteScale, graphics.PreferredBackBufferHeight - kazumiContinue.Height * continueSpriteScale - yOffset);
            kazumiContinue3Position = new Vector2(kazumiContinue2Position.X - kazumiContinue.Width * continueSpriteScale, graphics.PreferredBackBufferHeight - kazumiContinue.Height * continueSpriteScale - yOffset);
        }

        public override void UpdateMana()
        {
            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region active Tilemap1
                for (int i = 0; i < Singleplayer.gameMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.gameMaps[ai1].PowerupList[i].type == "mana")
                    {
                        if (CharacterKazumi.manaLeft < 3)
                        {
                            if (Singleplayer.gameMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.gameMaps[ai1].PowerupList.Remove(Singleplayer.gameMaps[ai1].PowerupList.ElementAt(i));
                                CharacterKazumi.manaLeft += 1;
                            }
                        }
                    }
                }
                #endregion
                #region active Tilemap2
                for (int i = 0; i < Singleplayer.gameMaps[ai2].PowerupList.Count; i++)
                {
                    if (Singleplayer.gameMaps[ai2].PowerupList[i].type == "mana")
                    {
                        if (CharacterKazumi.manaLeft < 3)
                        {
                            if (Singleplayer.gameMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.gameMaps[ai2].PowerupList.Remove(Singleplayer.gameMaps[ai2].PowerupList.ElementAt(i));
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
                for (int i = 0; i < Singleplayer.gameMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.gameMaps[ai1].PowerupList[i].type == "health")
                    {
                        if (Singleplayer.hud.healthLeft <= Singleplayer.hud.fullHealth)
                        {
                            if (Singleplayer.gameMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                if (Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                {
                                    Singleplayer.hud.healthLeft += Singleplayer.gameMaps[ai1].PowerupList[i].amount;

                                    if (Singleplayer.hud.fullHealth < Singleplayer.hud.healthLeft)
                                        Singleplayer.hud.healthLeft = Singleplayer.hud.fullHealth;

                                    Singleplayer.gameMaps[ai1].PowerupList.Remove(Singleplayer.gameMaps[ai1].PowerupList.ElementAt(i));
                                }
                            }
                        }
                    }
                }
                #endregion
                #region active Tilemap2
                for (int i = 0; i < Singleplayer.gameMaps[ai2].PowerupList.Count; i++)
                {
                    if (Singleplayer.gameMaps[ai2].PowerupList[i].type == "health")
                    {
                        if (Singleplayer.hud.healthLeft <= Singleplayer.hud.fullHealth)
                        {
                            if (Singleplayer.gameMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                if (Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                {
                                    Singleplayer.hud.healthLeft += Singleplayer.gameMaps[ai2].PowerupList[i].amount;

                                    if (Singleplayer.hud.fullHealth < Singleplayer.hud.healthLeft)
                                        Singleplayer.hud.healthLeft = Singleplayer.hud.fullHealth;

                                    Singleplayer.gameMaps[ai2].PowerupList.Remove(Singleplayer.gameMaps[ai2].PowerupList.ElementAt(i));
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
                spriteBatch.Draw(kazumiPowerRightTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(kazumiPowerMiddleTail, kazumiHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
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

            #region Drawing current amount of Continues
            if (trialsLeft == 3)
            {
                spriteBatch.Draw(kazumiContinue, kazumiContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(kazumiContinue, kazumiContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(kazumiContinue, kazumiContinue3Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 2)
            {
                spriteBatch.Draw(kazumiContinue, kazumiContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(kazumiContinue, kazumiContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 1)
            {
                spriteBatch.Draw(kazumiContinue, kazumiContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            #endregion
        }
        #endregion
    }
}
