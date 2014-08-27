using CR4VE.GameBase.Camera;
using CR4VE.GameBase.HeadUpDisplay;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.AI;
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

        public static HUD hud;

        //Terrain
        static Entity terrain;
        static Entity lava;
        static BoundingSphere arenaBound;

        //moveable Entities
        public static Character player;

        public static List<Enemy> enemyList = new List<Enemy>();

        public static float blickWinkel;
        #endregion

        #region Konstruktor
        public Arena() { }
        #endregion

        #region Methoden
        public void Initialize(ContentManager content)
        {
            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            CameraArena.Initialize(800, 600);

            //Terrain
            terrain = new Entity(new Vector3(4, -20, -5), "Terrain/arena_hell", content);
            lava = new Entity(new Vector3(0, -50, -30), "Terrain/lavafloor", content);

            arenaBound = new BoundingSphere(new Vector3(3, 0, 12.5f), 41f);

            //moveable Entities
            player = new CharacterSeraphin(new Vector3(0, 0, 0), "sphereD5", content);

            #region Loading AI
            EnemyRedEye redEye;
            redEye = new EnemyRedEye(new Vector3(60, 0, 0), "EnemyEye", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            //fill list with enemies
            enemyList.Add(redEye);
            #endregion

            //HUD
            hud = new OpheliaHUD(content, graphics);
            cont = content;
        }

        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateArena(gameTime);

            player.Update(gameTime);

            //DEBUG
            Console.Clear();
            Console.WriteLine(player.Position);
            Console.WriteLine(player.Boundary.Intersects(arenaBound));

            #region Updating HUD
            hud.Update();
            /*hud.UpdateMana();
            if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }*/
            #endregion

            //Updating Enemies
            foreach (Enemy enemy in enemyList)
            {
                enemy.UpdateArena(gameTime);
            }

            //aktualisieren der lebenden Gegner
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList.ElementAt(i).health <= 0)
                {
                    enemyList.ElementAt(i).Destroy();
                    enemyList.Remove(enemyList.ElementAt(i));
                }
            }

            return Game1.EGameState.Arena;
        }

        public void Draw(GameTime gameTime)
        {
            //Terrain
            lava.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
            terrain.drawInArena(new Vector3(0.4f, 0.4f, 0.4f), 0, MathHelper.ToRadians(30), 0);

            //enemies
            foreach (AIInterface enemy in enemyList)
            {
                enemy.DrawInArena(gameTime);
            }

            //minions etc.
            foreach (Entity laser in CR4VE.GameLogic.AI.EnemyRedEye.laserList)
            {
                laser.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(-90) * laser.viewingDirection.X);
            }
            foreach (Entity minion in CharacterSeraphin.minionList)
            {
                minion.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);
            }
            foreach (Entity crystal in CharacterFractus.crystalList)
            {
                crystal.drawInArena(new Vector3(0.1f, 0.1f, 0.1f), 0, 0, 0);
            }

            player.drawInArenaWithoutBones(new Vector3(0.75f, 0.75f, 0.75f), 0, MathHelper.ToRadians(90) + blickWinkel, 0);

            #region HUD
            spriteBatch.Begin();

            hud.DrawGenerals(spriteBatch);
            hud.Draw(spriteBatch);

            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion
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
