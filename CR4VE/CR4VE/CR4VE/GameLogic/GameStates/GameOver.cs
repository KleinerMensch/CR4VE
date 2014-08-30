using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameBase.Camera;
using CR4VE.GameLogic.Controls;

namespace CR4VE.GameLogic.GameStates
{
    class GameOver : GameStateInterface
    {
        SpriteBatch spriteBatch;
        Texture2D background;
        private SpriteFont font;
        private Vector2 fontPosition;
        private GraphicsDeviceManager graphics;

        #region Konstruktor
        public GameOver() 
        {
           
        }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            graphics = CR4VE.Game1.graphics;
            spriteBatch = CR4VE.Game1.spriteBatch;
            background = content.Load<Texture2D>("Assets/Sprites/TryAgain");
            font = content.Load<SpriteFont>("Assets/Fonts/GameOverFlo");
            fontPosition = new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2);
            //throw new NotImplementedException();
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            Game1.EGameState nextState = GameControls.updateGameOver();

            return nextState;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "GAME OVER"/*"Game Over Flo"*/, fontPosition, Color.White);
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
