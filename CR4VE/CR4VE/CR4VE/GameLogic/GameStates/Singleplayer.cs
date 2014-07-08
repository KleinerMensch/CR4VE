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
        #region Attributes
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Tilemap terrainMap;
        public static Entity player;

        Texture2D background;
        Texture2D testTex;

        HUD hud;
        #endregion

        #region Constructors
        // im Konstruktor braucht nichts veraendert werden
        public Singleplayer() { }
        #endregion

        #region Methods
        public void Initialize(ContentManager content)
        {
            //Terrain
            terrainMap = new Tilemap();
            Tile.Content = content;
            terrainMap.Generate(new int[,] {
                {2,0,0,2,0},
                {0,0,2,0,2},
                {0,0,0,0,0},
                {2,0,0,0,0},
                {2,2,2,2,0}}, 10);

            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //initialize Camera Class
            Camera2D.WorldRectangle = new Rectangle(0, 0, 1920, 1080);
            Camera2D.ViewPortWidth = 800;
            Camera2D.ViewPortHeight = 600;

            //load textures
            background = content.Load<Texture2D>("Assets/Sprites/1397342868251");
            testTex = content.Load<Texture2D>("Assets/Sprites/doge");

            //load models
            //(BoundingBox ist noch hard gecoded auf Durchmesser = 10)
            player = new Entity(new Vector3(0, 0, 0), "sphereR5", content, new BoundingBox(new Vector3(-5, -5, -5), new Vector3(5, 5, 5)));

            //HUD
            hud = new HUD(content, graphics);
        }

        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardControls.updateSingleplayer(gameTime);
            hud.Update();

            //player.pos debug
            Console.WriteLine(player.Position);

            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            #region background
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);

            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            player.drawIn2DWorld(new Vector3(0.4f, 0.4f, 0.4f), 0, 0, 0);
            terrainMap.Draw(new Vector3(1, 1, 1), 0, 0, 0);
            #endregion

            #region HUD
            spriteBatch.Begin();

            hud.Draw(spriteBatch);

            spriteBatch.End();
            #endregion
        }
        

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
        #endregion
    }
}
