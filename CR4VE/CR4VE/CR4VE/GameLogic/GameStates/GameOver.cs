﻿using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.GameStates
{
    class GameOver : GameStateInterface
    {
        #region Konstruktor
        public GameOver() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
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