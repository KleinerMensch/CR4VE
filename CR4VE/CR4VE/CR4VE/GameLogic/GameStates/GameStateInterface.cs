using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.GameStates
{
    interface GameStateInterface : IDisposable
    {
        void Initialize(ContentManager content);
        CR4VE.Game1.EGameState Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void Unload();
    }
}
