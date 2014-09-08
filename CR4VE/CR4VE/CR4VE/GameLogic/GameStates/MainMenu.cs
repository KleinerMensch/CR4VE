using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameBase.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Controls;

namespace CR4VE.GameLogic.GameStates
{
    class MainMenu : GameStateInterface
    {
        #region Attribute
        GraphicsDevice graphics;
        SpriteBatch spriteBatch;

        Texture2D background;

        SpriteFont font;
        Vector2 fontPos_press;
        Vector2 fontPos_start;
        Vector2 fontPos_resolution;
        Vector2 fontPos_resolutionValue;
        Vector2 fontPos_fullscreen;

        String resolutionString;

        //Entities
        public static Entity sword;
        #endregion

        #region Konstruktor
        public MainMenu() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            //Game1 Attributes
            graphics = CR4VE.Game1.graphics.GraphicsDevice;
            spriteBatch = CR4VE.Game1.spriteBatch;

            //Camera
            CameraMenu.Initialize(Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);

            //Sprites
            background = content.Load<Texture2D>("Assets/Sprites/doge");

            //Fonts
            font = content.Load<SpriteFont>("Assets/Fonts/GameOverFlo");
            fontPos_press = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("PRESS").X / 2, 50);
            fontPos_start = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("START").X / 2, 100);

            fontPos_fullscreen = new Vector2(Game1.graphics.PreferredBackBufferWidth / 4 - font.MeasureString("Fullscreen:").X, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolution = new Vector2(Game1.graphics.PreferredBackBufferWidth * 3 / 4, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolutionValue = fontPos_resolution + new Vector2(0, 50);

            resolutionString = "< " + Game1.graphics.PreferredBackBufferWidth + " x " + Game1.graphics.PreferredBackBufferHeight;

            //Entities
            sword = new Entity(new Vector3(0, -238f, -10), "mainmenu_sword", content);
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            //get next Gamestate based on player input
            Game1.EGameState nextState = GameControls.updateMainMenu();

            GameControls.updateVibration(gameTime);

            //Fonts
            fontPos_press = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("PRESS").X / 2, 50);
            fontPos_start = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("START").X / 2, 100);

            fontPos_fullscreen = new Vector2(Game1.graphics.PreferredBackBufferWidth / 4 - font.MeasureString("Fullscreen:").X, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolution = new Vector2(Game1.graphics.PreferredBackBufferWidth * 3/4, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolutionValue = fontPos_resolutionValue = fontPos_resolution + new Vector2(0, 50);

            resolutionString = "< " + Game1.graphics.PreferredBackBufferWidth + " x " + Game1.graphics.PreferredBackBufferHeight;

            return nextState;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            #region Sprites (background)
            spriteBatch.Begin();

            graphics.Clear(Color.Black);
            
            //Fonts
            switch (GameControls.menuPosIndex)
            {
                case 0:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "PRESS", fontPos_press, Color.White);
                            spriteBatch.DrawString(font, "START", fontPos_start, Color.White);
                        }
                    } break;
                case 4:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            if (GameControls.fullscreenPossible)
                            {
                                spriteBatch.DrawString(font, "Fullscreen:", fontPos_fullscreen, Color.White);
                                spriteBatch.DrawString(font, Game1.graphics.IsFullScreen.ToString() + " >", fontPos_fullscreen + new Vector2(0, 50), Color.White);
                            }

                            spriteBatch.DrawString(font, "Resolution:", fontPos_resolution, Color.White);
                            spriteBatch.DrawString(font, resolutionString, fontPos_resolutionValue, Color.White);
                        }
                    } break;
            }
            
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.BlendState = BlendState.Opaque;
            graphics.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            sword.drawInMainMenu(new Vector3(1f, 1f, 1f), 0, 0, 0);
            #endregion
        }
        #endregion

        public void Unload()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

