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
    class FractusHUD : HUD
    {
        #region Attributes
        private Texture2D fractusContinue, fractusHealthContainer, fractusPowerLeftestCrystal, fractusPowerRightestCrystal, fractusPowerMiddleCrystalLeft, fractusPowerMiddleCrystalRight;
        private Vector2 fractusContinue1Position, fractusContinue2Position, fractusContinue3Position, fractusHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public FractusHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
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
        }

        public override void UpdateMana()
        {
            Console.WriteLine(CharacterFractus.manaLeft);
            #region Singleplayer
            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region active Tilemap1
                for (int i = 0; i < Singleplayer.currentMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.currentMaps[ai1].PowerupList[i].type == "mana")
                    {
                        if (CharacterFractus.manaLeft == 2.5f)
                        {
                            if (Singleplayer.currentMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.currentMaps[ai1].PowerupList.Remove(Singleplayer.currentMaps[ai1].PowerupList.ElementAt(i));
                                CharacterFractus.manaLeft += 0.5f;
                            }
                        }
                        else if (CharacterFractus.manaLeft < 3)
                        {
                            if (Singleplayer.currentMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.currentMaps[ai1].PowerupList.Remove(Singleplayer.currentMaps[ai1].PowerupList.ElementAt(i));
                                CharacterFractus.manaLeft += 1;
                            }
                        }
                    }
                }
                #endregion
                #region active Tilemap2
                for (int i = 0; i < Singleplayer.currentMaps[ai2].PowerupList.Count; i++)
                {
                    if (Singleplayer.currentMaps[ai2].PowerupList[i].type == "mana")
                    {
                        if (CharacterFractus.manaLeft == 2.5f)
                        {
                            if (Singleplayer.currentMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.currentMaps[ai2].PowerupList.Remove(Singleplayer.currentMaps[ai2].PowerupList.ElementAt(i));
                                CharacterFractus.manaLeft += 0.5f;
                            }
                        }
                        else if (CharacterFractus.manaLeft < 3)
                        {
                            if (Singleplayer.currentMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.currentMaps[ai2].PowerupList.Remove(Singleplayer.currentMaps[ai2].PowerupList.ElementAt(i));
                                CharacterFractus.manaLeft += 1;
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
        }

        public override void UpdateHealth()
        {
            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region active Tilemap1
                for (int i = 0; i < Singleplayer.currentMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.currentMaps[ai1].PowerupList[i].type == "health")
                    {
                        if (Singleplayer.hud.healthLeft <= Singleplayer.hud.fullHealth)
                        {
                            if (Singleplayer.currentMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                if (Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                {
                                    Singleplayer.hud.healthLeft += Singleplayer.currentMaps[ai1].PowerupList[i].amount;

                                    if (Singleplayer.hud.fullHealth < Singleplayer.hud.healthLeft)
                                        Singleplayer.hud.healthLeft = Singleplayer.hud.fullHealth;

                                    Singleplayer.currentMaps[ai1].PowerupList.Remove(Singleplayer.currentMaps[ai1].PowerupList.ElementAt(i));
                                }
                            }
                        }
                    }
                }
                #endregion
                #region active Tilemap2
                for (int i = 0; i < Singleplayer.currentMaps[ai2].PowerupList.Count; i++)
                {
                    if (Singleplayer.currentMaps[ai2].PowerupList[i].type == "health")
                    {
                        if (Singleplayer.hud.healthLeft <= Singleplayer.hud.fullHealth)
                        {
                            if (Singleplayer.currentMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                if (Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                {
                                    Singleplayer.hud.healthLeft += Singleplayer.currentMaps[ai2].PowerupList[i].amount;

                                    if (Singleplayer.hud.fullHealth < Singleplayer.hud.healthLeft)
                                        Singleplayer.hud.healthLeft = Singleplayer.hud.fullHealth;

                                    Singleplayer.currentMaps[ai2].PowerupList.Remove(Singleplayer.currentMaps[ai2].PowerupList.ElementAt(i));
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
