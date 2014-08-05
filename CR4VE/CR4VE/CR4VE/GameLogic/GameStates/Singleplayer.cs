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

        public static Tilemap terrainMap;

        Texture2D background;
        Texture2D testTex;

        public static Entity player;
        public List<Entity> laserList = new List<Entity>();
        float spawn = 0;

        EnemyRedEye redEye;
        EnemySkull skull;
        EnemySpinningCrystal spinningCrystal;

        //BoundingSpheres noch durch BoundingBoxes zu ersetzen
        BoundingSphere eyeBS, skullBS, playerBS, crystalBS;
        Vector3 eyeBSpos, skullBSpos, playerBSpos, crystalBSpos;

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
            #region Terrain
            terrainMap = new Tilemap();

            Tile.Content = content;

            int[,] layout = new int[,] {
                {1,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};

            int boxSize = 10;

            terrainMap.Generate(layout, boxSize);
            #endregion

            // Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //initialize Camera Class
            Camera2D.WorldRectangle = new Rectangle(0, 0, 1920, 1080);
            Camera2D.ViewPortWidth = 800;
            Camera2D.ViewPortHeight = 600;

            Matrix camView = Matrix.CreateLookAt(Camera2D.CamPosition3D, Camera2D.CamTarget, Vector3.Up);
            Matrix camProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), (float)8 / 6, 1f, 1000);

            Camera2D.BoundFrustum = new BoundingFrustum(camView * camProjection);

            //load 2D textures
            background = content.Load<Texture2D>("Assets/Sprites/stone");

            //initialize player entity
            player = new Entity(new Vector3(0, 0, 0), "sphereD5", content);

            // AI
            // auch hier die Werte fuer den Meilenstein angepasst
            redEye = new EnemyRedEye(new Vector3(60, 0, 0));
            redEye.Initialize(content);
            skull = new EnemySkull(new Vector3(260, 0, 0));
            skull.Initialize(content);
            spinningCrystal = new EnemySpinningCrystal(new Vector3(120, 0, 0));
            spinningCrystal.Initialize(content);

            playerBSpos = player.position;
            eyeBSpos = redEye.enemyPosition;
            skullBSpos = skull.enemyPosition;
            crystalBSpos = spinningCrystal.enemyPosition;

            playerBS = new BoundingSphere(playerBSpos,4);
            eyeBS = new BoundingSphere(eyeBSpos, 3);
            skullBS = new BoundingSphere(skullBSpos, 3);
            crystalBS = new BoundingSphere(crystalBSpos, 3);

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

            List<Tile> test = terrainMap.getVisibleTiles();
            
            Console.WriteLine("Anzahl sichtbarer Tiles = " + test.Count);
            Console.WriteLine("CamTarget = " +  Camera2D.CamTarget);
            Console.WriteLine("Frustum-Player-Intersect: " + Camera2D.BoundFrustum.Intersects(player.Boundary));
            Console.WriteLine(Camera2D.BoundFrustum.Left);
            Console.WriteLine(Camera2D.BoundFrustum.Right);

            spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Entity laser in laserList)
            {
                BoundingSphere laserBS = new BoundingSphere(laser.position, 2);

                if (playerBS.Intersects(laserBS))
                {
                    hud.healthLeft -= (int)(hud.fullHealth * 0.01);
                }
                laser.position.X -= 0.7f;
            }
            LoadEnemies(redEye.enemy.position, cont);

            hud.Update();
            if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }

            playerBS.Center = player.position;
            redEye.Update(gameTime);
            eyeBS.Center = redEye.enemy.position;
            skull.Update(gameTime);
            skullBS.Center = skull.enemy.position;
            spinningCrystal.Update(gameTime);
            crystalBS.Center = spinningCrystal.enemy.position;
            
            //noch wird Kollision mit Gegnern per BoundingSphere berechnet
            //noch zu aendern in Bounding Box
            //if(player.boundary.Intersects(redEye.enemy.boundary) || player.boundary.Intersects(skull.enemy.boundary))
            if (playerBS.Intersects(eyeBS) || playerBS.Intersects(skullBS) || playerBS.Intersects(crystalBS))
            {
                hud.healthLeft -= (int) (hud.fullHealth * 0.01);
            }

            /*if (player.position.X >= 310)
                return Game1.EGameState.Arena;*/

            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }
        #endregion

        #region Laser vom Auge
        //bisher nur einzelne Nadeln gespawnt
        public void LoadEnemies(Vector3 EyePosition, ContentManager content)
        {
            if (spawn > 1)
            {
                spawn = 0;
                if (laserList.Count() < 10)
                    laserList.Add(new Entity(EyePosition, "ImaFirinMahLaserr", content));
            }
            for (int i = 0; i < laserList.Count; i++)
            {
                // wenn laser zu weit weg, dann verschwindet er
                // 'laser' verschwindet nocht nicht, wenn er den Spieler beruehrt
                if (laserList[i].position.X < 50 || laserList[i].position.X > 150)
                {
                    laserList.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            #region Background
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);

            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            //Player
            player.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, 0);
            
            //Terrain
            terrainMap.Draw(new Vector3(1, 1, 1), 0, 0, 0);

            //Enemies
            redEye.Draw(gameTime);
            skull.Draw(gameTime);
            spinningCrystal.Draw(gameTime);

            foreach (Entity laser in laserList)
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
