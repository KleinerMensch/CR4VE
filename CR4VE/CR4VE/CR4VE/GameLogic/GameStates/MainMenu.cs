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

        //Buttons
        Button playButton;
        Button exitButton;
        //Button creditsButton;

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

            //Buttons
            playButton = new Button(content.Load<Texture2D>("Assets/Sprites/ButtonPlay"), graphics);
            exitButton = new Button(content.Load<Texture2D>("Assets/Sprites/ButtonExit"), graphics);
            //creditsButton = new Button(content.Load<Texture2D>("Assets/Sprites/ButtonExit"), graphics);

            playButton.setPosition(new Vector2(350, 300));
            exitButton.setPosition(new Vector2(350, 330));
            //creditsButton.setPosition(new Vector2(350, 360));

            //Entities
            sword = new Entity(new Vector3(0, -238f, -10), "mainmenu_sword", content);
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateMainMenu();
            Console.WriteLine(sword.Position);
            MouseState mouseState = Mouse.GetState();

            //Buttons
            playButton.Update(mouseState);
            exitButton.Update(mouseState);
            //creditsButton.Update(mouseState);

            if (playButton.isClicked)
                return Game1.EGameState.Singleplayer;

            if (exitButton.isClicked)
                //Gamestate Nothing beendet einfach das Spiel
                return Game1.EGameState.Nothing;

            //if (creditsButton.isClicked == true)
            //    return Game1.EGameState.Credits;

            //Entities
            //sword

            return Game1.EGameState.MainMenu;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            #region Sprites (background)
            spriteBatch.Begin();

            //spriteBatch.Draw(background, new Vector2(0,0), CameraMenu.BackgroundRectangle, Color.White);
            
            playButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
            //creditsButton.Draw(spriteBatch);

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

