using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //Text parameters
        SpriteFont font;
        Vector2 fontPos_press;
        Vector2 fontPos_start;
        Vector2 fontPos_tutorial;
        Vector2 fontPos_tutorialValue;
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

            GameControls.menuPosIndex = 0;

            //Camera
            CameraMenu.Initialize(Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);

            //Sprites
            background = content.Load<Texture2D>("Assets/Sprites/doge");

            #region Fonts
            font = content.Load<SpriteFont>("Assets/Fonts/HUDfont");
            fontPos_press = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("PRESS").X / 2, 50);
            fontPos_start = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("START").X / 2, 100);

            fontPos_tutorial = new Vector2(Game1.graphics.PreferredBackBufferWidth /2 - font.MeasureString("Play Tutorial?").X/2, Game1.graphics.PreferredBackBufferHeight - font.MeasureString("Play Tutorial?").Y - 100);
            fontPos_tutorialValue = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("< " + Singleplayer.isTutorial.ToString() + " >").X / 2, fontPos_tutorial.Y + 50);

            fontPos_fullscreen = new Vector2(Game1.graphics.PreferredBackBufferWidth / 4 - font.MeasureString("Fullscreen:").X, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolution = new Vector2(Game1.graphics.PreferredBackBufferWidth * 3 / 4, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolutionValue = fontPos_resolution + new Vector2(0, 50);

            resolutionString = "< " + Game1.graphics.PreferredBackBufferWidth + " x " + Game1.graphics.PreferredBackBufferHeight;
            #endregion

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

            #region Fonts
            fontPos_press = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("PRESS").X / 2, 50);
            fontPos_start = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("START  or  ENTER").X / 2, 100);

            fontPos_tutorial = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("Play Tutorial?").X / 2, Game1.graphics.PreferredBackBufferHeight - font.MeasureString("Play Tutorial?").Y - 100);
            fontPos_tutorialValue = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 - font.MeasureString("< " + Singleplayer.isTutorial.ToString() + " >").X / 2, fontPos_tutorial.Y + 50);

            fontPos_fullscreen = new Vector2(Game1.graphics.PreferredBackBufferWidth / 4 - font.MeasureString("Fullscreen:").X, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolution = new Vector2(Game1.graphics.PreferredBackBufferWidth * 3/4, Game1.graphics.PreferredBackBufferHeight / 2);
            fontPos_resolutionValue = fontPos_resolutionValue = fontPos_resolution + new Vector2(0, 50);

            resolutionString = "< " + Game1.graphics.PreferredBackBufferWidth + " x " + Game1.graphics.PreferredBackBufferHeight;
            #endregion

            return nextState;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            #region Sprites (background)
            spriteBatch.Begin();

            graphics.Clear(Color.Black);
            
            #region Fonts
            switch (GameControls.menuPosIndex)
            {
                //Start
                case 0:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "PRESS", fontPos_press, Color.White);
                            spriteBatch.DrawString(font, "START  or  ENTER", fontPos_start, Color.White);
                        }
                    } break;

                //more detailed options
                case 4:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            if (GameControls.fullscreenPossible)
                            {
                                spriteBatch.DrawString(font, "Fullscreen:", fontPos_fullscreen, Color.White);
                                spriteBatch.DrawString(font, Game1.graphics.IsFullScreen.ToString() + " >", fontPos_fullscreen + new Vector2(0, 50), Color.White);
                            }
                            else
                            {
                                spriteBatch.DrawString(font, "Fullscreen:", fontPos_fullscreen, Color.Gray);
                                spriteBatch.DrawString(font, "(select 1280 x 720 first)", fontPos_fullscreen + new Vector2(0, 50), Color.Gray);
                            }

                            spriteBatch.DrawString(font, "Resolution:", fontPos_resolution, Color.White);
                            spriteBatch.DrawString(font, resolutionString, fontPos_resolutionValue, Color.White);
                        }
                    } break;
            }
            #endregion

            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.BlendState = BlendState.Opaque;
            graphics.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            sword.drawInMainMenu(new Vector3(1f, 1f, 1f), 0, 0, 0);
            #endregion

            #region Sprites (foreground)
            spriteBatch.Begin();

            #region Fonts
            switch (GameControls.menuPosIndex)
            {
                //Singleplayer
                case 1:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Play Tutorial?", fontPos_tutorial, Color.White);
                            spriteBatch.DrawString(font, "< " + Singleplayer.isTutorial.ToString() + " >", fontPos_tutorialValue, Color.White);
                        }
                    } break;
            }
            #endregion

            spriteBatch.End();
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