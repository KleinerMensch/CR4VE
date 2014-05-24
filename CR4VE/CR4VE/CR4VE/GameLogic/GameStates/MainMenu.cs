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

namespace CR4VE.GameLogic.GameStates
{
    class MainMenu : GameStateInterface
    {
        #region Attribute
        GraphicsDevice graphics;
        SpriteBatch spriteBatch;

        Texture2D background;

        Button playButton;
        Button exitButton;
        #endregion

        #region Konstruktor
        public MainMenu() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {

            graphics = CR4VE.Game1.graphics.GraphicsDevice;
            spriteBatch = CR4VE.Game1.spriteBatch;

            background = content.Load<Texture2D>("Assets/Sprites/doge");

            playButton = new Button(content.Load<Texture2D>("Assets/Sprites/ButtonPlay"), graphics);
            exitButton = new Button(content.Load<Texture2D>("Assets/Sprites/ButtonExit"), graphics);

            playButton.setPosition(new Vector2(350, 300));
            exitButton.setPosition(new Vector2(350, 330));
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            playButton.Update(mouseState);
            exitButton.Update(mouseState);

            if (playButton.isClicked == true)
                return Game1.EGameState.Singleplayer;
            if (exitButton.isClicked == true)
                return Game1.EGameState.StartScreen;
            return Game1.EGameState.MainMenu;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);
            playButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
            spriteBatch.End();
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

