using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.Characters
{
    class CharacterOphelia : Character
    {
        #region Attributes
        public static new float manaLeft = 3;
        Entity speer, doppelgaenger, holyThunder;

        Vector3 offset = new Vector3(8, 8, 8);
        float speedDoppel = 1;
        bool enemyHit = false;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterOphelia() : base() { }
        public CharacterOphelia(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public CharacterOphelia(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        #region Methods
        public override void Update(GameTime time)
        {
            #region UpdateDoppelgaenger
            if (launchedRanged)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    //Doppelganger movement
                    GamePadState curGamepad = GamePad.GetState(PlayerIndex.One);
                    KeyboardState curKeyboard = Keyboard.GetState();

                    Vector3 moveVecDoppelPad = new Vector3(curGamepad.ThumbSticks.Right, 0);
                    Vector3 moveVecDoppelBoard = Vector3.Zero;

                    if (curKeyboard.IsKeyDown(Keys.Up)) moveVecDoppelBoard += new Vector3(0, speedDoppel, 0);
                    if (curKeyboard.IsKeyDown(Keys.Left)) moveVecDoppelBoard += new Vector3(-speedDoppel, 0, 0);
                    if (curKeyboard.IsKeyDown(Keys.Down)) moveVecDoppelBoard += new Vector3(0, -speedDoppel, 0);
                    if (curKeyboard.IsKeyDown(Keys.Right)) moveVecDoppelBoard += new Vector3(speedDoppel, 0, 0);

                    if (moveVecDoppelBoard == Vector3.Zero)
                        doppelgaenger.move(moveVecDoppelPad);
                    else
                        doppelgaenger.move(moveVecDoppelBoard);

                    //Doppelgaenger schnellt hervor und verschwindet
                    //nach 50 Einheiten oder wenn er mit etwas kollidiert
                    //Effekt kann noch veraendert werden
                    if (doppelgaenger.position != this.position + 50 * viewingDirection)
                    {
                        launchedRanged = true;
                        if (enemyHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(doppelgaenger);
                        }
                        else
                        {
                            foreach (Enemy enemy in Singleplayer.enemyList)
                            {
                                if (enemyHit)
                                {
                                    launchedRanged = false;
                                    attackList.Remove(doppelgaenger);
                                }
                                else
                                {
                                    foreach (Entity opheliaDoppelgaenger in attackList)
                                    {
                                        if (opheliaDoppelgaenger.boundary.Intersects(enemy.boundary))
                                        {
                                            enemy.health -= 1;
                                            enemyHit = true;
                                            Console.WriteLine("Ophelia hit enemy by RangedAttack");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        launchedRanged = false;
                        attackList.Remove(doppelgaenger);
                    }
                    enemyHit = false;
                }
                #endregion
                #region Arena
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    //Doppelganger movement
                    GamePadState curGamepad = GamePad.GetState(PlayerIndex.One);
                    KeyboardState curKeyboard = Keyboard.GetState();

                    Vector3 moveVecDoppelPad = new Vector3(curGamepad.ThumbSticks.Right, 0);
                    Vector3 moveVecDoppelBoard = Vector3.Zero;

                    if (curKeyboard.IsKeyDown(Keys.Up)) moveVecDoppelBoard += new Vector3(0, 0, -speedDoppel);
                    if (curKeyboard.IsKeyDown(Keys.Left)) moveVecDoppelBoard += new Vector3(-speedDoppel, 0, 0);
                    if (curKeyboard.IsKeyDown(Keys.Down)) moveVecDoppelBoard += new Vector3(0, 0, speedDoppel);
                    if (curKeyboard.IsKeyDown(Keys.Right)) moveVecDoppelBoard += new Vector3(speedDoppel, 0, 0);

                    if (moveVecDoppelBoard == Vector3.Zero)
                        doppelgaenger.move(moveVecDoppelPad);
                    else
                        doppelgaenger.move(moveVecDoppelBoard);

                    //Doppelgaenger schnellt hervor und verschwindet
                    //nach 50 Einheiten oder wenn er mit etwas kollidiert
                    //Effekt kann noch veraendert werden
                    if (doppelgaenger.position != this.position + 50 * viewingDirection)
                    {
                        launchedRanged = true;
                        if (enemyHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(doppelgaenger);
                        }
                        else
                        {
                            if (enemyHit)
                            {
                                launchedRanged = false;
                                attackList.Remove(doppelgaenger);
                            }
                            else
                            {
                                foreach (Entity opheliaDoppelgaenger in attackList)
                                {
                                    if (opheliaDoppelgaenger.boundary.Intersects(Arena.boss.boundary))
                                    {
                                        Arena.seraphinBossHUD.healthLeft -= 2;
                                        enemyHit = true;
                                        Console.WriteLine("Ophelia hit Boss by RangedAttack");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        launchedRanged = false;
                        attackList.Remove(doppelgaenger);
                    }
                    enemyHit = false;
                }
                #endregion
            }
            #endregion
        }

        public override void MeleeAttack(GameTime time)
        {
            launchedMelee = true;

            #region Singleplayer
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Vector3 speerPosition = Singleplayer.player.Position + viewingDirection * offset;
                speer = new Entity(speerPosition, "5x5x5Box1", Singleplayer.cont);
                speer.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                attackList.Add(speer);

                //Kollision mit Attacke
                foreach (Enemy enemy in Singleplayer.enemyList)
                {
                    foreach (Entity opheliaSpeer in attackList)
                    {
                        if (opheliaSpeer.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            Console.WriteLine("Ophelia hit enemy by MeleeAttack");
                        }
                    }
                }
                attackList.Remove(speer);
            }
            #endregion
            #region Arena
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Vector3 speerPosition = Arena.player.Position + viewingDirection * offset;
                speer = new Entity(speerPosition, "5x5x5Box1", Arena.cont);
                speer.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -2.5f, -2.5f) + viewingDirection * offset, this.position + new Vector3(2.5f, 2.5f, 2.5f) + viewingDirection * offset);
                attackList.Add(speer);

                //Kollision mit Attacke
                foreach (Entity opheliaSpeer in attackList)
                {
                    if (opheliaSpeer.boundary.Intersects(Arena.boss.boundary))
                    {
                        Arena.seraphinBossHUD.healthLeft -= 3;
                        Console.WriteLine("Ophelia hit Boss by MeleeAttack");
                    }
                }
                attackList.Remove(speer);
            }
            #endregion
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft > 0)
            {
                manaLeft -= 1;
                launchedRanged = true;

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    doppelgaenger = new Entity(this.position, "Enemies/skull", Singleplayer.cont);
                    doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(doppelgaenger);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    doppelgaenger = new Entity(this.position, "Enemies/skull", Arena.cont);
                    doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(doppelgaenger);
                }
                #endregion
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            if (manaLeft >= 2)
            {
                manaLeft -= 2;
                launchedSpecial = true;

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    holyThunder = new Entity(this.Position, "Terrain/10x10x10Box1", Singleplayer.cont);
                    holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                    attackList.Add(holyThunder);

                    foreach (Enemy enemy in Singleplayer.enemyList)
                    {
                        if (holyThunder.boundary.Intersects(enemy.boundary))
                        {
                            enemy.health -= 1;
                            Console.WriteLine("Ophelia hit enemy by AoE");
                        }
                    }
                    attackList.Remove(holyThunder);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    holyThunder = new Entity(this.Position, "Terrain/10x10x10Box1", Arena.cont);
                    holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                    attackList.Add(holyThunder);

                    if (holyThunder.boundary.Intersects(Arena.boss.boundary))
                    {
                        Arena.seraphinBossHUD.healthLeft -= 50;
                        Console.WriteLine("Ophelia hit Boss by AoE");
                    }
                    attackList.Remove(holyThunder);
                }
                #endregion
            }
        }

        public override void DrawAttacks()
        {
            #region DrawMelee
            if (launchedMelee)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    speer.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    speer.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
                launchedMelee = false;
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    doppelgaenger.drawIn2DWorld(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    doppelgaenger.drawInArena(new Vector3(0.01f, 0.01f, 0.01f), 0, 0, 0);
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    holyThunder.drawIn2DWorld(Vector3.One, 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    holyThunder.drawInArena(Vector3.One, 0, 0, 0);
                launchedSpecial = false;
            }
            #endregion
        }
        #endregion
    }
}
