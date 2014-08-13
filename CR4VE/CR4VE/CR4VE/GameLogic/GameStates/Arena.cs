using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.Controls;
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
        #region Attribute
        public static Character player;
        float blickWinkel;
        #endregion

        public Arena() { }

        public void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            player = new CharacterOphelia(new Vector3(0, 0, 0), "skull", content);
        }

        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardControls.updateMultiplayer(gameTime);
            blickWinkel = (float) Math.Atan2(player.viewingDirection.Z, player.viewingDirection.X);
            Console.WriteLine(player.position + " " + player.viewingDirection + " " + blickWinkel);
            //throw new NotImplementedException();
            return Game1.EGameState.Arena;
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //graphics.GraphicsDevice.Clear(Color.Black);
            player.drawInArena(new Vector3(1, 1, 1), 0, blickWinkel, 0);
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
