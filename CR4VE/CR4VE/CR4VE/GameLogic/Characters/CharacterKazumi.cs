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
    class CharacterKazumi : Character
    {
        #region Attributes
        float offset = 8;
        float speed = 1;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterKazumi():base() { }
        public CharacterKazumi(Vector3 pos, String modelName, ContentManager cm):base(pos, modelName, cm) { }
        public CharacterKazumi(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
        #endregion

        #region Methods
        public override void MeleeAttack(GameTime time)
        {
            BoundingBox claws = new BoundingBox(this.position + new Vector3(-1+offset*viewingDirection.X,-1,-1),this.position+new Vector3(1+offset*viewingDirection.X,1,1));
            foreach (Enemy enemy in Singleplayer.enemyList)
            {
                if (claws.Intersects(enemy.boundary))
                {
                    enemy.health -= 1;
                    Console.WriteLine("Kazumi hit enemy by MeleeAttack");
                }
            }
        }

        public override void RangedAttack(GameTime time)
        {
            //bisher nur fuer den Singleplayer
            BoundingBox fireBall = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
            
            //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
            while (fireBall.Min.X != this.position.X+50*viewingDirection.X)
            {
                if (enemyHit) break;
                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (enemyHit) break;
                    if (fireBall.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        enemyHit = true;
                        Console.WriteLine("Kazumi hit enemy by RangedAttack");
                    }
                }
                fireBall.Min += new Vector3(speed*viewingDirection.X, 0, 0);
                fireBall.Max += new Vector3(speed*viewingDirection.X, 0, 0);
            }
            enemyHit = false;
        }

        public override void SpecialAttack(GameTime time)
        {
            BoundingBox danceOfFireFox = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
            foreach (Enemy enemy in Singleplayer.enemyList)
            {
                if (danceOfFireFox.Intersects(enemy.boundary))
                {
                    enemy.health -= 1;
                    Console.WriteLine("Kazumi hit enemy by AoE");
                }
            }
        }
        #endregion
    }
}
