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
    class OpheliaHUD : HUD
    {
        #region Attributes
        private Texture2D opheliaContinue, opheliaHealthContainer, opheliaPower;
        private Vector2 opheliaContinue1Position, opheliaContinue2Position, opheliaContinue3Position, opheliaHealthContainerPosition;
        public Color opheliaPowerColor;
        #endregion

        #region inherited Constructor
        public OpheliaHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            opheliaContinue = content.Load<Texture2D>("Assets/Sprites/ContinueOphelia");
            opheliaHealthContainer = content.Load<Texture2D>("Assets/Sprites/OpheliaHPBar");
            opheliaPower = content.Load<Texture2D>("Assets/Sprites/OpheliaPower");
            #endregion

            opheliaPowerColor = new Color(198, 226, 255, 0);

            opheliaHealthContainerPosition = new Vector2(0, graphics.PreferredBackBufferHeight - (opheliaHealthContainer.Height * spriteScale));
            opheliaContinue1Position = new Vector2(opheliaHealthContainer.Width * spriteScale, graphics.PreferredBackBufferHeight - (opheliaContinue.Height * continueSpriteScale) - yOffset);
            opheliaContinue2Position = new Vector2(opheliaContinue1Position.X + opheliaContinue.Width*continueSpriteScale, graphics.PreferredBackBufferHeight - (opheliaContinue.Height * continueSpriteScale) - yOffset);
            opheliaContinue3Position = new Vector2(opheliaContinue2Position.X + opheliaContinue.Width * continueSpriteScale, graphics.PreferredBackBufferHeight - (opheliaContinue.Height * continueSpriteScale) - yOffset);

            //set parameters
            this.healthLeft = fullHealth;

            this.isBurning = false;
            this.isSwimming = false;
        }

        public override void UpdateMana()
        {
            //Fadingeffekte
            //Alphawert 255 => komplett transparent
            //Alphawert 0 => nicht transparent
            //if (opheliaPowerColor.A < 255 && powerIsDown == false)
            //    opheliaPowerColor.A += 1;
            //else if (opheliaPowerColor.A == 255)
            //{
            //    powerIsDown = true;
            //    opheliaPowerColor = new Color(0, 0, 0, 0);
            //}

            if (CharacterOphelia.manaLeft == 3)
                opheliaPowerColor = new Color(198, 226, 255, 0);
            else if (CharacterOphelia.manaLeft == 2)
                opheliaPowerColor = new Color(198, 226, 255, 85);
            else if (CharacterOphelia.manaLeft == 1)
                opheliaPowerColor = new Color(198, 226, 255, 170);
            else if (CharacterOphelia.manaLeft == 0)
                opheliaPowerColor = new Color(0, 0, 0, 0);

            if (Game1.currentState == Game1.EGameState.Singleplayer)
            {
                int ai1 = Singleplayer.activeIndex1;
                int ai2 = Singleplayer.activeIndex2;

                #region active Tilemap1
                for (int i = 0; i < Singleplayer.gameMaps[ai1].PowerupList.Count; i++)
                {
                    if (Singleplayer.gameMaps[ai1].PowerupList[i].type == "mana")
                    {
                        if (CharacterOphelia.manaLeft < 3)
                        {
                            if (Singleplayer.gameMaps[ai1].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.gameMaps[ai1].PowerupList.Remove(Singleplayer.gameMaps[ai1].PowerupList.ElementAt(i));
                                CharacterOphelia.manaLeft += 1;
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
                        if (CharacterOphelia.manaLeft < 3)
                        {
                            if (Singleplayer.gameMaps[ai2].PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                            {
                                Singleplayer.gameMaps[ai2].PowerupList.Remove(Singleplayer.gameMaps[ai2].PowerupList.ElementAt(i));
                                CharacterOphelia.manaLeft += 1;
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
                //check for Powerups
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

                //calculate damage by environment
                if (isSwimming)
                    this.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.003);

                if (isBurning)
                    this.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }
        }

        public override void UpdateLiquidPositionByResolution()
        {
            if ((graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 16 / 9) || (graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight == 4 / 3))
                liquidPosition = opheliaHealthContainerPosition + new Vector2(114.5f, 157);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float percentagedHealthLeft = (float)healthLeft / (float)fullHealth;

            // 3.Argument ist SourceRectangle -> null = ganzes Sprite wird gezeichnet
            spriteBatch.Draw(redLiquid, liquidPosition, new Rectangle(0, 0, redLiquid.Width, (int) (percentagedHealthLeft*redLiquid.Height)), Color.White, MathHelper.ToRadians(180), new Vector2(redLiquid.Width / 2, redLiquid.Height / 2), spriteScale, SpriteEffects.None, 0);
            spriteBatch.Draw(opheliaHealthContainer, opheliaHealthContainerPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            spriteBatch.Draw(opheliaPower, opheliaHealthContainerPosition+ new Vector2(0,4), null, opheliaPowerColor, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);

            #region Drawing current amount of Continues
            if (trialsLeft == 3)
            {
                spriteBatch.Draw(opheliaContinue, opheliaContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(opheliaContinue, opheliaContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(opheliaContinue, opheliaContinue3Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 2)
            {
                spriteBatch.Draw(opheliaContinue, opheliaContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
                spriteBatch.Draw(opheliaContinue, opheliaContinue2Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            else if (trialsLeft == 1)
            {
                spriteBatch.Draw(opheliaContinue, opheliaContinue1Position, null, Color.White, 0, Vector2.Zero, continueSpriteScale, SpriteEffects.None, 0);
            }
            #endregion
        }
        #endregion
    }
}
