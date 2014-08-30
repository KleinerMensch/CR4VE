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
        private Texture2D opheliaHealthContainer, opheliaPower;
        public Color opheliaPowerColor;

        private Vector2 opheliaHealthContainerPosition;
        #endregion

        #region inherited Constructor
        public OpheliaHUD(ContentManager content, GraphicsDeviceManager manager):base(content,manager) { }
        #endregion

        #region Methods
        public override void Initialize(ContentManager content)
        {
            #region LoadContent
            opheliaHealthContainer = content.Load<Texture2D>("Assets/Sprites/OpheliaHPBar");
            opheliaPower = content.Load<Texture2D>("Assets/Sprites/OpheliaPower");
            #endregion

            opheliaPowerColor = new Color(198, 226, 255, 0);

            opheliaHealthContainerPosition = new Vector2(0, graphics.PreferredBackBufferHeight - (opheliaHealthContainer.Height * spriteScale));
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

            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                for (int i = 0; i < Singleplayer.terrainMap.PowerupList.Count; i++)
                {
                    if (CharacterOphelia.manaLeft < 3)
                    {
                        if (Singleplayer.terrainMap.PowerupList[i].boundary.Intersects(Singleplayer.player.boundary))
                        {
                            Singleplayer.terrainMap.PowerupList.Remove(Singleplayer.terrainMap.PowerupList.ElementAt(i));
                            CharacterOphelia.manaLeft += 1;
                        }
                    }
                }
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
        }
        #endregion
    }
}
