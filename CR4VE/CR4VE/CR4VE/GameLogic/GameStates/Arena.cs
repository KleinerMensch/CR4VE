using CR4VE.GameBase.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.GameStates
{
    class Arena : GameStateInterface
    {
        SpriteBatch spriteBatch;
        Texture2D background;
        private SpriteFont font;
        private Vector2 fontPosition;
        private GraphicsDeviceManager graphics;

        public Arena() { }

        public void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            graphics = CR4VE.Game1.graphics;
            spriteBatch = CR4VE.Game1.spriteBatch;

            font = content.Load<SpriteFont>("Assets/Fonts/HUDfont");
            fontPosition = new Vector2(graphics.PreferredBackBufferWidth - 300, graphics.PreferredBackBufferHeight - 150);
        }

        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //throw new NotImplementedException();
            return Game1.EGameState.Arena;
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "..ENTERING ARENA", fontPosition, Color.White);
            spriteBatch.End();
        }

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
