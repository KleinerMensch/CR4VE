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
    class CharacterOphelia : Character
    {
        #region Attributes
        Vector3 offset = new Vector3(8, 8, 8);
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
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Vector3 speerPosition = Singleplayer.player.Position + viewingDirection * offset;
                Entity speer = new Entity(speerPosition, "5x5x5Box1", Singleplayer.cont);
                speer.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                speer.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (speer.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Ophelia hit enemy by MeleeAttack");
                    }
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Vector3 speerPosition = Arena.player.Position + viewingDirection * offset;
                Entity speer = new Entity(speerPosition, "5x5x5Box1", Arena.cont);
                speer.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                speer.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                foreach (Enemy enemy in Arena.enemyList)
                {
                    if (speer.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Ophelia hit enemy by MeleeAttack");
                    }
                }
            }
        }

        public override void RangedAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Entity doppelgaenger = new Entity(this.position, "skull", Arena.cont);
                doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Doppelgaenger schnellt hervor und verschwindet
                //nach 50 Einheiten oder wenn er mit etwas kollidiert
                //Effekt kann noch veraendert werden
                while (doppelgaenger.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    foreach (Enemy enemy in Singleplayer.enemyList)
                    {
                        if (enemyHit) break;
                        if (doppelgaenger.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            enemyHit = true;
                            Console.WriteLine("Ophelia hit enemy by RangedAttack");
                        }
                    }
                    doppelgaenger.position += speed * viewingDirection;
                    doppelgaenger.boundary.Min += speed * viewingDirection;
                    doppelgaenger.boundary.Max += speed * viewingDirection;
                    doppelgaenger.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Entity doppelgaenger = new Entity(this.position, "skull", Singleplayer.cont);
                doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Doppelgaenger schnellt hervor und verschwindet
                //nach 50 Einheiten oder wenn er mit etwas kollidiert
                //Effekt kann noch veraendert werden
                while (doppelgaenger.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    foreach (Enemy enemy in Singleplayer.enemyList)
                    {
                        if (enemyHit) break;
                        if (doppelgaenger.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            enemyHit = true;
                            Console.WriteLine("Ophelia hit enemy by RangedAttack");
                        }
                    }
                    doppelgaenger.position += speed * viewingDirection;
                    doppelgaenger.boundary.Min += speed * viewingDirection;
                    doppelgaenger.boundary.Max += speed * viewingDirection;
                    doppelgaenger.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Entity holyThunder = new Entity(this.Position, "10x10x10Box1", Arena.cont);
                holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                holyThunder.drawInArena(Vector3.One, 0, 0, 0);

                foreach (Enemy enemy in Arena.enemyList)
                {
                    if (holyThunder.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Ophelia hit enemy by AoE");
                    }
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Entity holyThunder = new Entity(this.Position, "10x10x10Box1", Singleplayer.cont);
                holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                holyThunder.drawInArena(Vector3.One, 0, 0, 0);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (holyThunder.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Ophelia hit enemy by AoE");
                    }
                }
            }
        }
        #endregion
    }
}
