using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.Characters
{
    class CharacterFractus : Character
    {
        #region Attributes
        public static new float manaLeft = 3;
        public static List<Entity> crystalList = new List<Entity>();
        Enemy nearestEnemy = new Enemy();

        float speed = 1;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterFractus() : base() { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        #region Methods
        public override void Update(GameTime time)
        {
            foreach (Entity crystal in crystalList)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    crystal.boundary = new BoundingBox(crystal.position + new Vector3(-2, -2, -2), crystal.position + new Vector3(2, 2, 2));
                    float minDistance = float.MaxValue;

                    foreach (Enemy enemy in Singleplayer.enemyList)
                    {
                        float distance = (crystal.position - enemy.position).Length();
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }

                    foreach (Entity healthAbsorbingCrystal in crystalList)
                    {
                        //absorbs health of nearest enemy if enemy is in range of effect
                        if ((healthAbsorbingCrystal.position - nearestEnemy.position).Length() < 50 && nearestEnemy.health > 0)
                        {
                            Console.WriteLine(nearestEnemy.health + " Fractus hit enemy by SpecialAttack");
                            nearestEnemy.health -= 0.01f;

                            //transfers health to Fractus
                            if (Singleplayer.hud.trialsLeft <= 3 && Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                                Singleplayer.hud.healthLeft += (int)(Singleplayer.hud.fullHealth * 0.01f);
                        }
                    }
                }
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    crystal.boundary = new BoundingBox(crystal.position + new Vector3(-2, -2, -2), crystal.position + new Vector3(2, 2, 2));
                    float minDistance = float.MaxValue;

                    //foreach (Enemy enemy in Arena.bossList)
                    //{
                    //    float distance = (crystal.position - enemy.position).Length();
                    //    if (distance < minDistance)
                    //    {
                    //        minDistance = distance;
                    //        nearestEnemy = enemy;
                    //    }
                    //}

                    foreach (Entity healthAbsorbingCrystal in crystalList)
                    {
                        //absorbs health of nearest enemy if enemy is in range of effect
                        if ((healthAbsorbingCrystal.position - nearestEnemy.position).Length() < 50 && nearestEnemy.health > 0)
                        {
                            Console.WriteLine(nearestEnemy.health + " Fractus hit enemy by SpecialAttack");
                            nearestEnemy.health -= 0.01f;

                            //transfers health to Fractus in Arena
                            if (Arena.opheliaHud.trialsLeft <= 3 && Arena.opheliaHud.healthLeft < Arena.opheliaHud.fullHealth)
                                Arena.opheliaHud.healthLeft += (int)(Arena.opheliaHud.fullHealth * 0.01f);
                        }
                    }
                }
            }
        }

        public override void MeleeAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Entity crystalShield = new Entity(this.position, "5x5x5Box1", Singleplayer.cont);
                crystalShield.boundary = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));
                crystalShield.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (crystalShield.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Fractus hit enemy by crystalShield");
                    }
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Entity crystalShield = new Entity(this.position, "5x5x5Box1", Arena.cont);
                crystalShield.boundary = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));
                crystalShield.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                //foreach (Enemy enemy in Arena.bossList)
                //{
                //    if (crystalShield.boundary.Intersects(enemy.boundary))
                //    {
                //        enemy.health -= 1;
                //        Console.WriteLine("Fractus hit enemy by crystalShield");
                //    }
                //}
            }
        }

        public override void RangedAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Entity crystals = new Entity(this.position, "skull", Singleplayer.cont);
                crystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                while (crystals.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    foreach (Enemy enemy in Singleplayer.enemyList)
                    {
                        if (enemyHit) break;
                        if (crystals.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            enemyHit = true;
                            Console.WriteLine("Fractus hit enemy by RangedAttack");
                        }
                    }
                    crystals.position += speed * viewingDirection;
                    crystals.boundary.Min += speed * viewingDirection;
                    crystals.boundary.Max += speed * viewingDirection;
                    crystals.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Entity crystals = new Entity(this.position, "skull", Arena.cont);
                crystals.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                while (crystals.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    //foreach (Enemy enemy in Arena.bossList)
                    //{
                    //    if (enemyHit) break;
                    //    if (crystals.boundary.Intersects(enemy.boundary))
                    //    {
                    //        enemy.health -= 1;
                    //        enemyHit = true;
                    //        Console.WriteLine("Fractus hit enemy by RangedAttack");
                    //    }
                    //}
                    crystals.position += speed * viewingDirection;
                    crystals.boundary.Min += speed * viewingDirection;
                    crystals.boundary.Max += speed * viewingDirection;
                    crystals.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                crystalList.Add(new Entity(this.position, "enemySpinningNoAnim", CR4VE.GameLogic.GameStates.Singleplayer.cont));
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                crystalList.Add(new Entity(this.position, "enemySpinningNoAnim", CR4VE.GameLogic.GameStates.Arena.cont));
            }

            //mehr als 2 Kristalle nicht moeglich
            if (crystalList.Count > 2)
                crystalList.RemoveAt(0);
        }
        #endregion
    }
}
