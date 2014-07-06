using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using CR4VE.GameLogic.Controls;
using CR4VE.GameBase.Objects;

namespace CR4VE.GameLogic.GameStates
{
    // besser: uebergeordnete PlayMode Klasse fuer Single- und Multiplayer !
    class Multiplayer : GameStateInterface
    {
        #region Attribute
        public static Entity player;
        Entity terrain;
        #endregion

        #region Konstruktor
        public Multiplayer() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            player = new Entity(new Vector3(0, 0, 0), "protoSphere", content);
            terrain = new Entity(new Vector3(0, 0, 0), "protoTerrain1", content);
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardControls.updateMultiplayer(gameTime);

            return Game1.EGameState.Multiplayer;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            player.drawInArena(new Vector3(1, 1, 1));
            terrain.drawInArena(new Vector3(1, 1, 1));
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