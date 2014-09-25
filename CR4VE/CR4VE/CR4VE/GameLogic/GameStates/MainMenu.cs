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

        //Textures
        Texture2D selection;
        Texture2D selection_Fractus;
        Texture2D selection_Kazumi;
        Texture2D selection_Ophelia;
        Texture2D selection_Seraphin;

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
            selection_Fractus = content.Load<Texture2D>("Assets/Sprites/Fractus_Selection");
            selection_Kazumi = content.Load<Texture2D>("Assets/Sprites/Kazumi_SelectionMP");
            selection_Ophelia = content.Load<Texture2D>("Assets/Sprites/Ophelia_SelectionMP");
            selection_Seraphin = content.Load<Texture2D>("Assets/Sprites/Seraphin_Selection");

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

            //Sounds
            Sounds.Initialize(content);

            //Multiplayer character selection
            ResetMultiplayerSelection();
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            //Background music
            Sounds.menu.Play();

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
            fontPos_MPPlayer2 = new Vector2(W * 3 / 4 - font.MeasureString(playableChars[select2Index]).X / 2, H / 2 - font.MeasureString("K").Y);
            fontPos_MPPlayer3 = new Vector2(W / 4 - font.MeasureString(playableChars[select3Index]).X / 2, H - font.MeasureString("K").Y - 2);
            fontPos_MPPlayer4 = new Vector2(W * 3 / 4 - font.MeasureString(playableChars[select4Index]).X / 2, H - font.MeasureString("K").Y - 2);
            #endregion

            return nextState;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            #region Sprites (background)
            spriteBatch.Begin();

            int H = Game1.graphics.PreferredBackBufferHeight;
            int W = Game1.graphics.PreferredBackBufferWidth;

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
                            spriteBatch.DrawString(font, "Enter/A - Continue", new Vector2(font.MeasureString(" ").X, H - font.MeasureString("E").Y), Color.White);
                            spriteBatch.DrawString(font, "Down - Navigate", new Vector2(W - font.MeasureString("Down - Navigate ").X, H - font.MeasureString("D").Y), Color.White);
                        }
                    } break;
                #endregion

                #region Multiplayer
                case 2:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Enter/A - Continue", new Vector2(font.MeasureString(" ").X, H - font.MeasureString("E").Y), Color.White);
                            spriteBatch.DrawString(font, "Up/Down - Navigate", new Vector2(W - font.MeasureString("Up/Down - Navigate ").X, H - font.MeasureString("D").Y), Color.White);
                        }
                    } break;
                #endregion

                #region Options
                case 3:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Enter/A - Continue", new Vector2(font.MeasureString(" ").X, H - font.MeasureString("E").Y), Color.White);
                            spriteBatch.DrawString(font, "Up - Navigate", new Vector2(W - font.MeasureString("Up - Navigate ").X, H - font.MeasureString("D").Y), Color.White);                            
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

                            spriteBatch.DrawString(font, "ESC/B - Get Back", new Vector2(font.MeasureString(" ").X, H - font.MeasureString("E").Y), Color.White);
                            spriteBatch.DrawString(font, "Left/Right - Toggle", new Vector2(W - font.MeasureString("Left/Right - Toggle ").X, H - font.MeasureString("L").Y), Color.White);

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

                                        int x1 = (int)font.MeasureString(" ").X;
                                        spriteBatch.DrawString(font, "Player 1", new Vector2(x1, 0), Color.White);
                                        break;

                                    case 1:
                                        spriteBatch.DrawString(font, playableChars[select2Index], fontPos_MPPlayer2, Color.White);

                                        int x2 = W - (int) font.MeasureString("Player 2 ").X;
                                        spriteBatch.DrawString(font, "Player 2", new Vector2(x2, 0), Color.White);
                                        break;

                                    case 2:
                                        spriteBatch.DrawString(font, playableChars[select3Index], fontPos_MPPlayer3, Color.White);

                                        int x3 = (int)font.MeasureString(" ").X;
                                        int y3 = H - (int)font.MeasureString("Player 3").Y;
                                        spriteBatch.DrawString(font, "Player 3", new Vector2(x3, y3), Color.White);
                                        break;

                                    case 3:
                                        spriteBatch.DrawString(font, playableChars[select4Index], fontPos_MPPlayer4, Color.White);

                                        int x4 = W - (int)font.MeasureString("Player 4 ").X;
                                        int y4 = H - (int)font.MeasureString("Player 4").Y;
                                        spriteBatch.DrawString(font, "Player 4", new Vector2(x4, y4), Color.White);
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
                                spriteBatch.Draw(selection, new Rectangle(0, 0, W / 2, H), Color.White);
                            else
                                spriteBatch.Draw(selection, new Rectangle(W / 2, 0, W / 2, H), Color.White);
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
                #region Singleplayer
                case 1:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            spriteBatch.DrawString(font, "Play Tutorial?", fontPos_tutorial, Color.White);
                            spriteBatch.DrawString(font, "< " + Singleplayer.isTutorial.ToString() + " >", fontPos_tutorialValue, Color.White);
                        }
                    } break;
                #endregion

                #region more detailed options (Multiplayer)
                case 5:
                    {
                        if (!GameControls.isMenuMoving)
                        {
                            Vector2 pos1 = new Vector2(W / 2 - font.MeasureString("RB - Next Character").X / 2, H - font.MeasureString("RB - Next Character").Y);
                            Vector2 pos2 = new Vector2(W / 2 - font.MeasureString("ENTER - Start Game").X / 2, H - font.MeasureString("ENTER - Start Game").Y - 25);
                            Vector2 pos3 = new Vector2(W / 2 - font.MeasureString("ESC - Back").X / 2, H - font.MeasureString("ESC - Back").Y - 50);

                            spriteBatch.DrawString(font, "RB - Next Character", pos1, Color.White);
                            spriteBatch.DrawString(font, "ENTER - Start Game", pos2, Color.White);
                            spriteBatch.DrawString(font, "ESC - Back", pos3, Color.White);

                            for (int i = 0; i < GameControls.ConnectedControllers; i++)
                            {
                                switch (MPSelection[i])
                                {
                                    #region Fractus
                                    case "Fractus":
                                        {
                                            if (i == 0) spriteBatch.Draw(selection_Fractus, new Rectangle(W / 4 - W * 5 / 48, 0, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 1) spriteBatch.Draw(selection_Fractus, new Rectangle(W * 3 / 4 - W * 5 / 48, 0, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 2) spriteBatch.Draw(selection_Fractus, new Rectangle(W / 4 - W * 5 / 48, H / 2, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 3) spriteBatch.Draw(selection_Fractus, new Rectangle(W * 3 / 4 - W * 5 / 48, H / 2, W * 5 / 24, H * 133 / 270), Color.White);
                                        } break;
                                    #endregion

                                    #region Kazumi
                                    case "Kazumi":
                                        {
                                            if (i == 0) spriteBatch.Draw(selection_Kazumi, new Rectangle(W/4 - W*5/48, 0, W*5/24, H*133/270), Color.White);
                                            else if (i == 1) spriteBatch.Draw(selection_Kazumi, new Rectangle(W*3/4 - W*5/48, 0, W*5/24, H*133/270), Color.White);
                                            else if (i == 2) spriteBatch.Draw(selection_Kazumi, new Rectangle(W/4 - W*5/48, H/2, W*5/24, H*133/270), Color.White);
                                            else if (i == 3) spriteBatch.Draw(selection_Kazumi, new Rectangle(W*3/4 - W*5/48, H/2, W*5/24, H*133/270), Color.White);

                                            //new Vector2(960, 540),
                                            //new Vector2(1280, 720),
                                        } break;
                                    #endregion

                                    #region Ophelia
                                    case "Ophelia":
                                        {
                                            if (i == 0) spriteBatch.Draw(selection_Ophelia, new Rectangle(W / 4 - W * 5 / 48, 0, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 1) spriteBatch.Draw(selection_Ophelia, new Rectangle(W * 3 / 4 - W * 5 / 48, 0, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 2) spriteBatch.Draw(selection_Ophelia, new Rectangle(W / 4 - W * 5 / 48, H / 2, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 3) spriteBatch.Draw(selection_Ophelia, new Rectangle(W * 3 / 4 - W * 5 / 48, H / 2, W * 5 / 24, H * 133 / 270), Color.White);
                                        } break;
                                    #endregion

                                    #region Seraphin
                                    case "Seraphin":
                                        {
                                            if (i == 0) spriteBatch.Draw(selection_Seraphin, new Rectangle(W / 4 - W * 5 / 48, 0, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 1) spriteBatch.Draw(selection_Seraphin, new Rectangle(W * 3 / 4 - W * 5 / 48, 0, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 2) spriteBatch.Draw(selection_Seraphin, new Rectangle(W / 4 - W * 5 / 48, H / 2, W * 5 / 24, H * 133 / 270), Color.White);
                                            else if (i == 3) spriteBatch.Draw(selection_Seraphin, new Rectangle(W * 3 / 4 - W * 5 / 48, H / 2, W * 5 / 24, H * 133 / 270), Color.White);
                                        } break;
                                    #endregion

                                    default:
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
                            Vector2 pos1 = new Vector2(W / 2 - font.MeasureString("Left/Right - Choose Level").X / 2, 0);
                            Vector2 pos2 = new Vector2(W / 2 - font.MeasureString("ENTER/A - Start Game").X / 2, 25);
                            Vector2 pos3 = new Vector2(W / 2 - font.MeasureString("ESC/B - Back").X / 2, 50);

                            spriteBatch.DrawString(font, "Left/Right - Choose Level", pos1, Color.White);
                            spriteBatch.DrawString(font, "ENTER/A - Start Game", pos2, Color.White);
                            spriteBatch.DrawString(font, "ESC/B - Back", pos3, Color.White);

                            spriteBatch.Draw(selection_Kazumi, new Rectangle(W/4 - W/6, H/5, W/3, H * 3/4), Color.White);
                            spriteBatch.Draw(selection_Ophelia, new Rectangle(W * 3/4 - W/9, H/5, W/3, H * 3/4), Color.White);
                        }
                    } break;
                #endregion
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
        public static bool checkMultiplayerConditions()
        { 
            for (int i = 0; i < GameControls.ConnectedControllers; i++)
			{
			    if (MPSelection[i] == "none")
                    return false;
                else
                {
                    for (int j = 0; j < GameControls.ConnectedControllers; j++)
			        {
                        if (MPSelection[i] == MPSelection[j] && i != j)
                            return false;
			        }                    
                }
			}

            return true;
        }

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