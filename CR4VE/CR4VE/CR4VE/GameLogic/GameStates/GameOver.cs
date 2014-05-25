using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameBase.Camera;

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
            background = content.Load<Texture2D>("Assets/Sprites/FloFace");
            font = content.Load<SpriteFont>("Assets/Fonts/GameOverFlo");
            fontPosition = new Vector2(graphics.PreferredBackBufferWidth - 200, graphics.PreferredBackBufferHeight -50);
            //throw new NotImplementedException();
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            return Game1.EGameState.GameOver;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);
            spriteBatch.DrawString(font, "Game Over Flo", fontPosition, Color.White);
            spriteBatch.End();
            
            //throw new NotImplementedException();
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
