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
using CR4VE.GameLogic.AI;

namespace CR4VE.GameLogic.GameStates
{
    class Singleplayer : GameStateInterface
    {
        #region Attribute
        ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D background;
        Texture2D testTex;

        public static Tilemap terrainMap;
        public static Entity player;

        EnemyRedEye redEye;
        EnemySkull skull;
        EnemySpinningCrystal spinningCrystal;

        public static HUD hud;
        #endregion

        #region Konstruktor
        // im Konstruktor braucht nichts veraendert werden
        public Singleplayer() { }
        #endregion

        // Contents werden hier reingeladen
        #region Init
        public void Initialize(ContentManager content)
        {
            #region Terrain
            //Terrain
            terrainMap = new Tilemap();
            Tile.Content = content;
            terrainMap.Generate(new int[,] {
                {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,3,3},
                {0,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,2,2},
                {3,2,2,2,2,3,0,0,3,3,0,0,3,3,3,3,3,3,3,3,0,0,0,0,3,3,3,3,3,3,2,2,2,1,1},
                {2,1,1,1,1,2,4,4,2,2,4,4,2,2,2,2,2,2,2,2,4,4,4,4,2,2,2,2,2,2,2,1,1,4,4},
                {2,4,4,4,4,2,4,4,2,2,4,4,2,2,2,2,2,1,1,1,4,4,4,4,1,1,1,1,1,1,1,4,4,4,4}}, 10);
            #endregion

            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //initialize Camera Class
            Camera2D.WorldRectangle = new Rectangle(0, 0, 1920, 1080);
            Camera2D.ViewPortWidth = 800;
            Camera2D.ViewPortHeight = 600;

            //load textures
            background = content.Load<Texture2D>("Assets/Sprites/stone");
            testTex = content.Load<Texture2D>("Assets/Sprites/doge");

            //load models
            player = new Entity(new Vector3(0,0,0), "protoSphere", content);

            #region Loading AI
            //set Positions
            redEye = new EnemyRedEye(new Vector3(60, 0, 0));          
            skull = new EnemySkull(new Vector3(260, 0, 0));
            spinningCrystal = new EnemySpinningCrystal(new Vector3(120, 0, 0));
            
            //initialize Models
            redEye.Initialize(content);
            skull.Initialize(content);
            spinningCrystal.Initialize(content);

            //defining bounding boxes of entities
            redEye.enemy.boundary = new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3));
            skull.enemy.boundary = new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3));
            spinningCrystal.enemy.boundary = new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3));
            #endregion

            //HUD
            hud = new HUD(content, graphics);
            this.cont = content;
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardControls.updateSingleplayer(gameTime);

            #region Updating HUD
            hud.Update();
            if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }
            #endregion

            //updating laserList
            redEye.LoadEnemies(redEye.enemy.position, cont);

            #region Updating Enemies
            redEye.Update(gameTime);
            skull.Update(gameTime);
            spinningCrystal.Update(gameTime);
            #endregion

            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            #region draw background
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);
            //spriteBatch.Draw(testTex, Camera2D.transform2D(new Vector2(200, 200)), Color.White);

            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            player.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, 0);
            //terrainMap.Draw(new Vector3(1, 1, 1), 0, 0, 0);
            redEye.Draw(gameTime);
            skull.Draw(gameTime);
            spinningCrystal.Draw(gameTime);

            foreach (Entity laser in CR4VE.GameLogic.AI.EnemyRedEye.laserList)
            {
                laser.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(90));
            }
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
