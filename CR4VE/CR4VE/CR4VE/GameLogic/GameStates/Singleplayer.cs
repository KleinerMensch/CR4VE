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
        //List<EnemyRedEye> redEyeList;
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

            // Zugriff auf Attribute der Game1 Klasse
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
            player = new Entity(new Vector3(1,1,1), "protoSphere", content);
            // -35 war fuer den Meilenstein wichtig
            player.position = new Vector3(5, -35, 0);
            //terrain = new Entity(new Vector3(0, 0, 0), "protoTerrain1", content);

            // AI
            // auch hier die Werte fuer den Meilenstein angepasst
            redEye = new EnemyRedEye(new Vector3(60, 3-45, 0));
            redEye.Initialize(content);
            skull = new EnemySkull(new Vector3(260, 2-45, 0));
            skull.Initialize(content);
            spinningCrystal = new EnemySpinningCrystal(new Vector3(120, 2-45, 0));
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
            KeyboardControls.updateSingleplayer(gameTime);

            spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Entity laser in laserList)
            {
                BoundingSphere laserBS = new BoundingSphere(laser.position, 2);

                if (playerBS.Intersects(laserBS))
                {
                    hud.healthLeft -= (int)(hud.fullHealth * 0.01);
                    Console.WriteLine("intersection with laser");
                }
                laser.position.X -= 0.7f;
            }
            LoadEnemies(redEye.enemy.position, cont);

            // fuer meilenstein3
            Console.WriteLine(player.position.X);

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

            foreach (Tile t in terrainMap.TilesList)
            {
                if (playerBS.Intersects(t.boundary))
                    Console.WriteLine("terrainintersection");
            }
            
            //noch wird Kollision mit Gegnern per BoundingSphere berechnet
            //noch zu aendern in Bounding Box
            //if(player.boundary.Intersects(redEye.enemy.boundary) || player.boundary.Intersects(skull.enemy.boundary))
            if (playerBS.Intersects(eyeBS) || playerBS.Intersects(skullBS) || playerBS.Intersects(crystalBS))
            {
                Console.WriteLine("intersection");
                hud.healthLeft -= (int) (hud.fullHealth * 0.01);
            }

            if (player.position.X >= 310)
                return Game1.EGameState.Arena;

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
            terrainMap.Draw(new Vector3(1, 1, 1), 0, 0, 0);
            redEye.Draw(gameTime);
            skull.Draw(gameTime);
            spinningCrystal.Draw(gameTime);

            foreach (Entity laser in laserList)
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
