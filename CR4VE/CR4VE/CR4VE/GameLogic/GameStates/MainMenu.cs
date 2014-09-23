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

        Texture2D selection;

        //Text parameters
        SpriteFont font;
        Vector2 fontPos_press;
        Vector2 fontPos_start;
        Vector2 fontPos_tutorial;
        Vector2 fontPos_tutorialValue;
        Vector2 fontPos_resolution;
        Vector2 fontPos_resolutionValue;
        Vector2 fontPos_fullscreen;
        Vector2 fontPos_MPPlayer1;
        Vector2 fontPos_MPPlayer2;
        Vector2 fontPos_MPPlayer3;
        Vector2 fontPos_MPPlayer4;

        String resolutionString;

        //Entities
        public static Entity sword;

        //Multiplayer
        public static readonly String[] playableChars = {"none", "Fractus", "Kazumi", "Ophelia", "Seraphin"};

        public static int select1Index, select2Index, select3Index, select4Index;
        public static String[] MPSelection;
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
            selection = content.Load<Texture2D>("Assets/Sprites/MainMenu_selection");

            #region Fonts
            int H = Game1.graphics.PreferredBackBufferHeight;
            int W = Game1.graphics.PreferredBackBufferWidth;

            font = content.Load<SpriteFont>("Assets/Fonts/HUDfont");
            fontPos_press = new Vector2(W / 2 - font.MeasureString("PRESS").X / 2, 50);
            fontPos_start = new Vector2(W / 2 - font.MeasureString("START").X / 2, 100);

            fontPos_tutorial = new Vector2(W /2 - font.MeasureString("Play Tutorial?").X/2, H - font.MeasureString("Play Tutorial?").Y - 100);
            fontPos_tutorialValue = new Vector2(W / 2 - font.MeasureString("< " + Singleplayer.isTutorial.ToString() + " >").X / 2, fontPos_tutorial.Y + 50);

            fontPos_fullscreen = new Vector2(W / 4 - font.MeasureString("Fullscreen:").X, H / 2);
            fontPos_resolution = new Vector2(W * 3 / 4, H / 2);
            fontPos_resolutionValue = fontPos_resolution + new Vector2(0, 50);

            resolutionString = "< " + W + " x " + H;

            fontPos_MPPlayer1 = new Vector2(W / 4 - font.MeasureString(playableChars[select1Index]).X / 2, H / 2 - font.MeasureString("K").Y);
            fontPos_MPPlayer2 = new Vector2(W * 3 / 4 - font.MeasureString(playableChars[select2Index]).X / 2, H / 2 - font.MeasureString("K").Y - 10);
            fontPos_MPPlayer3 = new Vector2(W / 4 - font.MeasureString(playableChars[select3Index]).X / 2, H - font.MeasureString("K").Y);
            fontPos_MPPlayer4 = new Vector2(W * 3 / 4 - font.MeasureString(playableChars[select4Index]).X / 2, H - font.MeasureString("K").Y - 10);
            #endregion

            //Entities
            sword = new Entity(new Vector3(0, -238f, -10), "mainmenu_sword", content);

            //Multiplayer character selection
            ResetMultiplayerSelection();
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            //get next GameState based on player input
            Game1.EGameState nextState = GameControls.updateMainMenu();

            GameControls.updateVibration(gameTime);

            #region Fonts
            int H = Game1.graphics.PreferredBackBufferHeight;
            int W = Game1.graphics.PreferredBackBufferWidth;

            fontPos_press = new Vector2(W / 2 - font.MeasureString("PRESS").X / 2, 50);
            fontPos_start = new Vector2(W / 2 - font.MeasureString("START  or  ENTER").X / 2, 100);

            fontPos_tutorial = new Vector2(W / 2 - font.MeasureString("Play Tutorial?").X / 2, H - font.MeasureString("Play Tutorial?").Y - 100);
            fontPos_tutorialValue = new Vector2(W / 2 - font.MeasureString("< " + Singleplayer.isTutorial.ToString() + " >").X / 2, fontPos_tutorial.Y + 50);

            fontPos_fullscreen = new Vector2(W / 4 - font.MeasureString("Fullscreen:").X, H / 2);
            fontPos_resolution = new Vector2(W * 3/4, H / 2);
            fontPos_resolutionValue = fontPos_resolutionValue = fontPos_resolution + new Vector2(0, 50);

            resolutionString = "< " + W + " x " + H;

            fontPos_MPPlayer1 = new Vector2(W / 4 - font.MeasureString(playableChars[select1Index]).X / 2, H / 2 - font.MeasureString("K").Y);
            fontPos_MPPlayer2 = new Vector2(W * 3 / 4 - font.MeasureString(playableChars[select2Index]).X / 2, H / 2 - font.MeasureString("K").Y - 10);
            fontPos_MPPlayer3 = new Vector2(W / 4 - font.MeasureString(playableChars[select3Index]).X / 2, H - font.MeasureString("K").Y);
            fontPos_MPPlayer4 = new Vector2(W * 3 / 4 - font.MeasureString(playableChars[select4Index]).X / 2, H - font.MeasureString("K").Y - 10);
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
                #region Start
                case 0:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "PRESS", fontPos_press, Color.White);
                            spriteBatch.DrawString(font, "START  or  ENTER", fontPos_start, Color.White);
                        }
                    } break;
                #endregion

                #region Singleplayer
                case 1:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Enter/A - Continue", new Vector2(0, 0), Color.White);
                            spriteBatch.DrawString(font, "Down - Navigate", new Vector2(0, 25), Color.White);
                        }
                    } break;
                #endregion

                #region Multiplayer
                case 2:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Enter/A - Continue", new Vector2(0, 0), Color.White);
                            spriteBatch.DrawString(font, "Up/Down - Navigate", new Vector2(0, 25), Color.White);
                        }
                    } break;
                #endregion

                #region Options
                case 3:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Enter/A - Continue", new Vector2(0, 0), Color.White);
                            spriteBatch.DrawString(font, "Up - Navigate", new Vector2(0, 25), Color.White);                            
                        }
                    } break;
                #endregion

                #region detailed options
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

                            spriteBatch.DrawString(font, "ESC/B - Get Back", new Vector2(0, 0), Color.White);
                            spriteBatch.DrawString(font, "Left/Right - Toggle", new Vector2(0, 25), Color.White);

                            spriteBatch.DrawString(font, "Resolution:", fontPos_resolution, Color.White);
                            spriteBatch.DrawString(font, resolutionString, fontPos_resolutionValue, Color.White);
                        }
                    } break;
                #endregion

                #region more detailed options (Multiplayer)
                case 5:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            for (int i = 0; i < GameControls.ConnectedControllers; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        spriteBatch.DrawString(font, playableChars[select1Index], fontPos_MPPlayer1, Color.White);
                                        break;

                                    case 1:
                                        spriteBatch.DrawString(font, playableChars[select2Index], fontPos_MPPlayer2, Color.White);
                                        break;

                                    case 2:
                                        spriteBatch.DrawString(font, playableChars[select3Index], fontPos_MPPlayer3, Color.White);
                                        break;

                                    case 3:
                                        spriteBatch.DrawString(font, playableChars[select4Index], fontPos_MPPlayer4, Color.White);
                                        break;
                                }
                            }
                        }
                    } break;
                #endregion

                #region more detailed options (Singleplayer)
                case 6:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Kazumi", fontPos_MPPlayer3, Color.White);
                            spriteBatch.DrawString(font, "Ophelia", fontPos_MPPlayer4, Color.White);

                            if (Singleplayer.isCrystal)
                                spriteBatch.Draw(selection, new Rectangle(0, 0, Game1.graphics.PreferredBackBufferWidth / 2, Game1.graphics.PreferredBackBufferHeight), Color.White);
                            else
                                spriteBatch.Draw(selection, new Rectangle(Game1.graphics.PreferredBackBufferWidth / 2, 0, Game1.graphics.PreferredBackBufferWidth / 2, Game1.graphics.PreferredBackBufferHeight), Color.White);
                        }
                    }
                    break;
                #endregion
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


        #region Help Methods
        public static void ResetMultiplayerSelection()
        {
            select1Index = 0;
            select2Index = 0;
            select3Index = 0;
            select4Index = 0;

            MPSelection = new String[4];
            MPSelection[0] = playableChars[select1Index];
            MPSelection[1] = playableChars[select2Index];
            MPSelection[2] = playableChars[select3Index];
            MPSelection[3] = playableChars[select4Index];
        }
        #endregion
    }
}