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
using CR4VE.GameBase.Objects.Terrain;
using CR4VE.GameLogic.Controls;
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
            terrainMap = new Tilemap();
            
            Tile.Content = content;

            int[,] layout = new int[,] {
                {0,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                {0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0}};

            int boxSize = 10;

            terrainMap.Generate(layout, boxSize);
            #endregion

            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //initialize Camera Class
            Camera2D.Initialize(800, 600);

            //load textures
            background = content.Load<Texture2D>("Assets/Sprites/stone");

            //initialize models
            player = new Entity(new Vector3(0,0,0), "sphereD5", content);
            player.Boundary = new BoundingBox(player.Position + new Vector3(-2.5f, -2.5f, -2.5f), player.Position + new Vector3(2.5f, 2.5f, 2.5f));

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
            Console.Clear();

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
            #region Background
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position2D.X, (int)Camera2D.Position2D.Y, 800, 600), Color.White);
            
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            //Terrain
            terrainMap.Draw(new Vector3(1, 1, 1), 0, 0, 0);

            //Player
            player.drawIn2DWorldWithoutBones(new Vector3(1, 1, 1), 0, 0, 0);            
            
            //Enemies
            redEye.Draw(gameTime);
            skull.Draw(gameTime);
            spinningCrystal.Draw(gameTime);

            foreach (Entity laser in CR4VE.GameLogic.AI.EnemyRedEye.laserList)
            {
                laser.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(90));
            }
            #endregion

            #region HUD
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
