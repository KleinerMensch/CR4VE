using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Controls;
using CR4VE.GameBase.Terrain;

namespace CR4VE.GameLogic.GameStates
{
    class Singleplayer : GameStateInterface
    {
        #region Attribute
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Tilemap map;

        Texture2D background;
        Texture2D testTex;

        public static Entity player;
        Entity terrain;

        HUD hud;
        #endregion

        #region Konstruktor
        // im Konstruktor braucht nichts veraendert werden
        public Singleplayer() { }
        #endregion

        // Contents werden hier reingeladen
        #region Init
        public void Initialize(ContentManager content)
        {
            //terrain
            map = new Tilemap();
            Tiles.Content = content;
            map.Generate(new int[,]{
                {0,0,0,4,0,0,0,0,0,0,0,0,0},
                {0,4,4,3,4,0,0,4,4,4,4,0,0},
                {4,3,3,3,3,4,4,3,3,3,3,4,4},
                {1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,2,1,1,2,2,1,1},
                {1,1,1,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2},
            }, 64);

            // Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //initializing Camera Class
            Camera2D.WorldRectangle = new Rectangle(0, 0, 1920, 1080);
            Camera2D.ViewPortWidth = 800;
            Camera2D.ViewPortHeight = 600;

            //load textures
            background = content.Load<Texture2D>("Assets/Sprites/1397342868251");
            testTex = content.Load<Texture2D>("Assets/Sprites/doge");

            //load models
            player = new Entity(new Vector3(0, 0, 0), "protoSphere", content);
            terrain = new Entity(new Vector3(0, 0, 0), "protoTerrain", content);

            //HUD
            hud = new HUD(content, graphics);
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardControls.updateSingleplayer(gameTime);
            hud.Update();

            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            #region draw background
            spriteBatch.Begin();
           // map.Draw(spriteBatch);
            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);
            //spriteBatch.Draw(testTex, Camera2D.transform2D(new Vector2(200, 200)), Color.White);
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            player.drawIn2DWorld(new Vector3(1, 1, 1));
            //terrain.drawIn2DWorld(new Vector3(1, 1, 1));
            #endregion

            #region draw HUD
            spriteBatch.Begin();
            hud.Draw(spriteBatch);
            spriteBatch.End();
            #endregion
        }
        #endregion

        // loeschen aller grafischen Elemente
        public void Unload()
        {
            //throw new NotImplementedException();
        }

        // Freigabe der restlichen Standardressourcen
        // automatischer Aufruf des GarbageCollector beim Beenden des Spiels
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

