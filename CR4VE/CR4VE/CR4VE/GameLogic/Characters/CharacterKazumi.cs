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
    class CharacterKazumi : Character
    {
        #region Attributes
        Vector3 offset = new Vector3(8,8,8);
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
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Vector3 clawsPosition = Singleplayer.player.Position + viewingDirection * offset;
                Entity claws = new Entity(clawsPosition, "5x5x5Box1", Singleplayer.cont);
                claws.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                claws.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (claws.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Kazumi hit enemy by MeleeAttack");
                    }
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Vector3 clawsPosition = Arena.player.Position + viewingDirection * offset;
                Entity claws = new Entity(clawsPosition, "5x5x5Box1", Arena.cont);
                claws.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                claws.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

                foreach (Enemy enemy in Arena.enemyList)
                {
                    if (claws.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Kazumi hit enemy by MeleeAttack");
                    }
                }
            }
        }

        public override void RangedAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Entity fireBall = new Entity(this.position, "skull", Singleplayer.cont);
                fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                while (fireBall.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    foreach (Enemy enemy in Singleplayer.enemyList)
                    {
                        if (enemyHit) break;
                        if (fireBall.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            enemyHit = true;
                            Console.WriteLine("Kazumi hit enemy by RangedAttack");
                        }
                    }
                    fireBall.position += speed * viewingDirection;
                    fireBall.boundary.Min += speed * viewingDirection;
                    fireBall.boundary.Max += speed * viewingDirection;
                    fireBall.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Entity fireBall = new Entity(this.position, "skull", Arena.cont);
                fireBall.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));

                //Feuerball verschwindet nach 50 Einheiten oder wenn er mit etwas kollidiert
                while (fireBall.position != this.position + 50 * viewingDirection)
                {
                    if (enemyHit) break;
                    foreach (Enemy enemy in Arena.enemyList)
                    {
                        if (enemyHit) break;
                        if (fireBall.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            enemyHit = true;
                            Console.WriteLine("Kazumi hit enemy by RangedAttack");
                        }
                    }
                    fireBall.position += speed * viewingDirection;
                    fireBall.boundary.Min += speed * viewingDirection;
                    fireBall.boundary.Max += speed * viewingDirection;
                    fireBall.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                }
                enemyHit = false;
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Entity danceOfFireFox = new Entity(this.Position, "10x10x10Box1", Singleplayer.cont);
                danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                danceOfFireFox.drawInArena(Vector3.One, 0, 0, 0);

                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    if (danceOfFireFox.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Kazumi hit enemy by AoE");
                    }
                }
            }
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Entity danceOfFireFox = new Entity(this.Position, "10x10x10Box1", Arena.cont);
                danceOfFireFox.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                danceOfFireFox.drawInArena(Vector3.One, 0, 0, 0);
                
                foreach (Enemy enemy in Arena.enemyList)
                {
                    if (danceOfFireFox.boundary.Intersects(enemy.boundary))
                    {
                        enemy.health -= 1;
                        Console.WriteLine("Kazumi hit enemy by AoE");
                    }
                }
            }
        }
        #endregion
    }
}
