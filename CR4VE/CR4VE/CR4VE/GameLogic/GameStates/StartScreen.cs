﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameBase.Camera;

namespace CR4VE.GameLogic.GameStates
{
    class StartScreen : GameStateInterface
    {
        #region Attribute
        SpriteBatch spriteBatch;
        Texture2D background;
        #endregion

        #region Konstruktor
        public StartScreen() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            spriteBatch = CR4VE.Game1.spriteBatch;
            background = content.Load<Texture2D>("Assets/Sprites/Startscreen");
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            return Game1.EGameState.StartScreen;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);
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

