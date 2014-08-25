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
        
        public static Character player;
        public static List<Enemy> enemyList = new List<Enemy>();

        public static float blickWinkel;
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

            player = new CharacterFractus(new Vector3(0, 0, 0), "skull", content);

            EnemyRedEye redEye;
            redEye = new EnemyRedEye(new Vector3(10, 0, 0), "EnemyEye", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            enemyList.Add(redEye);

            //HUD
            hud = new OpheliaHUD(content, graphics);
            cont = content;
        }

        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GameControls.updateArena(gameTime);
            player.Update(gameTime);

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

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //enemies
            foreach (AIInterface enemy in enemyList)
            {
                enemy.DrawInArena(gameTime);
            }

            //minions etc.
            foreach (Entity laser in CR4VE.GameLogic.AI.EnemyRedEye.laserList)
            {
                laser.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(90));
            }
            foreach (Entity minion in CR4VE.GameLogic.Characters.CharacterSeraphin.minionList)
            {
                minion.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);
            }
            foreach (Entity crystal in CR4VE.GameLogic.Characters.CharacterFractus.crystalList)
            {
                crystal.drawInArena(new Vector3(0.1f, 0.1f, 0.1f), 0, 0, 0);
            }

            player.drawInArena(new Vector3(0.05f, 0.05f, 0.05f), 0, MathHelper.ToRadians(90) + blickWinkel, 0);

            #region HUD
            spriteBatch.Begin();

            hud.DrawGenerals(spriteBatch);
            hud.Draw(spriteBatch);

            spriteBatch.End();
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
