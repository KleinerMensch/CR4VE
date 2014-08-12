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
        public static List<Entity> crystalList = new List<Entity>();
        Enemy nearestEnemy = new Enemy();

        float speed = 1;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterFractus():base() { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm):base(pos, modelName, cm) { }
        public CharacterFractus(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
        #endregion

        #region Methods
        public override void Update(GameTime time)
        {
            foreach (Entity crystal in crystalList)
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
                        Console.WriteLine(nearestEnemy.health +" Fractus hit enemy by SpecialAttack");
                        nearestEnemy.health -= 0.01f;

                        //transfers health to Fractus
                        if(Singleplayer.hud.trialsLeft <= 3 && Singleplayer.hud.healthLeft < Singleplayer.hud.fullHealth)
                            Singleplayer.hud.healthLeft += (int)(Singleplayer.hud.fullHealth*0.01f);
                    }
                }
            }
        }

        public override void MeleeAttack(GameTime time)
        {
            BoundingBox crystalShield = new BoundingBox(this.position + new Vector3(-10, -3, -10), this.position + new Vector3(10, 3, 10));
            foreach (Enemy enemy in Singleplayer.enemyList)
            {
                if (crystalShield.Intersects(enemy.boundary))
                {
                    enemy.health -= 1;
                    Console.WriteLine("Fractus hit enemy by crystalShield");
                }
            }
        }

        public override void RangedAttack(GameTime time)
        {
            //bisher nur fuer den Singleplayer
            BoundingBox crystals = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

            //Kristallschauer verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
            while (crystals.Min.X != this.position.X + 50*viewingDirection.X)
            {
                if (enemyHit) break;
                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (enemyHit) break;
                    if (crystals.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        enemyHit = true;
                        Console.WriteLine("Fractus hit enemy by RangedAttack");
                    }
                }
                crystals.Min += new Vector3(speed*viewingDirection.X, 0, 0);
                crystals.Max += new Vector3(speed*viewingDirection.X, 0, 0);
            }
            enemyHit = false;
        }

        public override void SpecialAttack(GameTime time)
        {
            crystalList.Add(new Entity(this.position, "enemySpinningNoAnim", CR4VE.GameLogic.GameStates.Singleplayer.cont));

            //mehr als 2 Kristalle nicht moeglich
            if (crystalList.Count > 2)
                crystalList.RemoveAt(0);
        }
        #endregion
    }
}
