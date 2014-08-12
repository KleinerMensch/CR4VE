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
    class CharacterOphelia : Character
    {
        #region Attributes
        float offset = 8;
        float speed = 1;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterOphelia():base() { }
        public CharacterOphelia(Vector3 pos, String modelName, ContentManager cm):base(pos, modelName, cm) { }
        public CharacterOphelia(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
        #endregion

        #region Methods
        public override void MeleeAttack(GameTime time)
        {
            BoundingBox speer = new BoundingBox(this.position + new Vector3(-1+offset*viewingDirection.X,-1,-1),this.position+new Vector3(1+offset*viewingDirection.X,1,1));
            foreach (Enemy enemy in Singleplayer.enemyList)
            {
                if (speer.Intersects(enemy.boundary))
                {
                    enemy.health -= 1;
                    Console.WriteLine("Ophelia hit enemy by MeleeAttack");
                }
            }
        }

        public override void RangedAttack(GameTime time)
        {
            //bisher nur fuer den Singleplayer
            BoundingBox doppelgaenger = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
            
            //Doppelgaenger schnellt hervor und verschwindet
            //nach 50 Einheiten oder wenn er mit etwas kollidiert
            //Effekt kann noch veraendert werden
            while (doppelgaenger.Min.X != this.position.X+50*viewingDirection.X)
            {
                if (enemyHit) break;
                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (enemyHit) break;
                    if (doppelgaenger.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        enemyHit = true;
                        Console.WriteLine("Ophelia hit enemy by RangedAttack");
                    }
                }
                doppelgaenger.Min += new Vector3(speed*viewingDirection.X, 0, 0);
                doppelgaenger.Max += new Vector3(speed*viewingDirection.X, 0, 0);
            }
            enemyHit = false;
        }

        public override void SpecialAttack(GameTime time)
        {
            BoundingBox holyThunder = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
            foreach (Enemy enemy in Singleplayer.enemyList)
            {
                if (holyThunder.Intersects(enemy.boundary))
                {
                    enemy.health -= 1;
                    Console.WriteLine("Ophelia hit enemy by AoE");
                }
            }
        }
        #endregion
    }
}
