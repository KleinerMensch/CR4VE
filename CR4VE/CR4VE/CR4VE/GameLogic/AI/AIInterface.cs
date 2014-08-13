using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    interface AIInterface
    {
        void UpdateSingleplayer(GameTime gameTime);
        void UpdateArena(GameTime gameTime);
        void Draw(GameTime gameTime);
        void DrawInArena(GameTime gameTime);
    }
}
