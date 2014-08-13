using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public static ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public static Character player;
        Entity terrain;

        float blickWinkel;
        #endregion

        #region Konstruktor
        public Arena() { }
        #endregion

        #region Methoden
        public void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            CameraArena.Initialize(800, 600);

            player = new CharacterOphelia(new Vector3(0, 0, 0), "skull", content);

            terrain = new Entity(new Vector3(0, 0, 0), "protoTerrain1", content);
        }

        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardControls.updateArena(gameTime);

            //Winkel zwischen 2 Vektoren
            //Anfangsblickrichtung
            Vector3 startViewingDirection = new Vector3(1, 0, 0);
            float a = Vector3.Dot(startViewingDirection, player.viewingDirection);
            startViewingDirection.Normalize();
            player.viewingDirection.Normalize();
            float b = Vector3.Dot(startViewingDirection, player.viewingDirection);
            blickWinkel = a / b;

            return Game1.EGameState.Arena;
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            terrain.drawInArenaWithoutBones(Vector3.One, 0, 0, 0);

            //graphics.GraphicsDevice.Clear(Color.Black);
            player.drawInArena(new Vector3(1, 1, 1), 0, 0/*blickWinkel*/, 0);
        }

        public void Unload()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
