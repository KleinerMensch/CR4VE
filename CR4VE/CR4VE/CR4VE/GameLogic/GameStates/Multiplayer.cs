using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.Characters;

namespace CR4VE.GameLogic.GameStates
{
    // besser: uebergeordnete PlayMode Klasse fuer Single- und Multiplayer !
    class Multiplayer : GameStateInterface
    {
        #region Attribute
        public static ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Character player1;
        public static Character player2;
        public static Character player3;
        public static Character player4;

        public static float blickWinkelPlayer1;

        public static Entity terrain;
        public static Entity lava;
        #endregion

        #region Konstruktor
        public Multiplayer() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            CameraArena.Initialize(800,600);

            player1 = new CharacterKazumi(new Vector3(0, -12.5f, 0), "Kazumi", content);

            terrain = new Entity(new Vector3(4, -20, -5), "Terrain/arena_hell", content);

            lava = new Entity(new Vector3(0, -50, -30), "Terrain/lavafloor", content);
            
            //fuer Attacken wichtig
            cont = content;
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateMultiplayer(gameTime);

            player1.Update(gameTime);

            return Game1.EGameState.Multiplayer;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            //terrain
            lava.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

            terrain.drawInArena(new Vector3(0.4f, 0.4f, 0.4f), 0, MathHelper.ToRadians(30), 0);

            //players
            player1.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + blickWinkelPlayer1, 0);
            player1.DrawAttacks();
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