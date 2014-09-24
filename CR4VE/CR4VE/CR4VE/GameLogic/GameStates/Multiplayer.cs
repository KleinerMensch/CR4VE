using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.Characters;
using CR4VE.GameBase.HeadUpDisplay;

namespace CR4VE.GameLogic.GameStates
{
    class Multiplayer : GameStateInterface
    {
        #region Attribute
        public static ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Player parameters        
        private static Character[] playerArray;
        public static HUD[] hudArray;

        //Terrain
        public static readonly BoundingBox arenaFloorBox = new BoundingBox(new Vector3(-54, -25, -65), new Vector3(63, -15, 53));
        public static readonly BoundingSphere arenaBound = new BoundingSphere(new Vector3(5, -20, -8), 60f);

        public static Entity terrain;
        public static Entity lava;
        #endregion

        #region Properties
        public static Character[] Players
        { 
            get { return playerArray; } 
        }
        #endregion

        #region Konstruktor
        public Multiplayer() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            CameraArena.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            #region player models
            playerArray = new Character[GameControls.ConnectedControllers];
            hudArray = new HUD[GameControls.ConnectedControllers];

            for (int i = 0; i < GameControls.ConnectedControllers; i++)
            {
                switch (MainMenu.MPSelection[i])
                {
                    case "Fractus":
                        {
                            playerArray[i] = new CharacterFractus(new Vector3(0, -12.5f, 10), "Fractus", content,new BoundingBox(new Vector3(0, -12.5f, 10)+ new Vector3(-2.5f, -9, -2.5f), new Vector3(0, -12.5f, 10)+ new Vector3(2.5f, 9, 2.5f)));
                            hudArray[i] = new FractusHUD(content, graphics);
                        } break;

                    case "Kazumi":
                        {
                            playerArray[i] = new CharacterKazumi(new Vector3(0, -12.5f, -10), "Kazumi", content, new BoundingBox(new Vector3(0, -12.5f, -10) + new Vector3(-2.5f, -9, -2.5f), new Vector3(0, -12.5f, -10) + new Vector3(2.5f, 9, 2.5f)));
                            hudArray[i] = new KazumiHUD(content, graphics);
                        } break;

                    case "Ophelia":
                        {
                            playerArray[i] = new CharacterOphelia(new Vector3(-10, -12.5f, 0), "Ophelia", content, new BoundingBox(new Vector3(-10, -12.5f, 0) + new Vector3(-2.5f, -9, -2.5f), new Vector3(-10, -12.5f, 0) + new Vector3(2.5f, 9, 2.5f)));
                            hudArray[i] = new OpheliaHUD(content, graphics);
                        } break;

                    case "Seraphin":
                        {
                            playerArray[i] = new CharacterSeraphin(new Vector3(10, -12.5f, 0), "albino", content, new BoundingBox(new Vector3(10, -12.5f, 0) + new Vector3(-2.5f, -9, -2.5f), new Vector3(10, -12.5f, 0) + new Vector3(2.5f, 9, 2.5f)));
                            hudArray[i] = new SeraphinHUD(content, graphics);
                        } break;

                    default: { } break;
                }
            }
            #endregion

            //Terrain
            terrain = new Entity(new Vector3(5, -20, -10), "Terrain/arena_hell", content);
            lava = new Entity(new Vector3(0, -110, -30), "Terrain/lavafloor", content);
            
            //fuer Attacken wichtig
            cont = content;

            //Control parameters
            GameControls.initializeMultiplayer();
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateMultiplayer(gameTime);

            //Players
            for (int i = 0; i < GameControls.ConnectedControllers; i++)
            {
                if (!hudArray[i].isDead)
                {
                    Multiplayer.Players[i].Update(gameTime);
                    hudArray[i].Update();
                    hudArray[i].UpdateHealth();
                    hudArray[i].UpdateLiquidPositionByResolution();
                    hudArray[i].UpdateMana();
                }
            }

            return Game1.EGameState.Multiplayer;
        } 
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            #region 3D Objects
            //terrain
            lava.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
            terrain.drawInArena(new Vector3(0.4f, 0.4f, 0.4f), 0, MathHelper.ToRadians(30), 0);

            //players
            for (int i = 0; i < GameControls.ConnectedControllers; i++)
            {
                if (Players[i].CharacterType == "Seraphin" && !hudArray[i].isDead)
                    Multiplayer.Players[i].drawInArena(new Vector3(1f, 1f, 1f), 0, MathHelper.ToRadians(90) + Multiplayer.Players[i].blickWinkel, 0);
                else
                {
                    if (!hudArray[i].isDead)
                        Multiplayer.Players[i].drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + Multiplayer.Players[i].blickWinkel, 0);
                }
                
                Multiplayer.Players[i].DrawAttacks();
            }
            #endregion

            #region HUDs
            spriteBatch.Begin();
            for (int i = 0; i < GameControls.ConnectedControllers; i++)
            {
                if (!hudArray[i].isDead)
                {
                    hudArray[i].Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion
        }
        #endregion

        public void Unload()
        {
            for (int i = 0; i < 4; i++)
			{
			    MainMenu.MPSelection[i] = "none";
			}

            for (int i = 0; i < GameControls.ConnectedControllers; i++)
            {
                playerArray[i] = null;
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}