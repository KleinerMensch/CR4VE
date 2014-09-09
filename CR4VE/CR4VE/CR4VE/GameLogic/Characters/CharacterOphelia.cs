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
        Vector3 currentViewingDirection;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);

        Vector3 offset = new Vector3(12, 12, 12);
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
            #region Timeupdate for DrawAttacks
            // Decrements the timespan
            timeSpan -= time.ElapsedGameTime;
            // If the timespan is equal or smaller time "0"
            if (timeSpan <= TimeSpan.Zero)
            {
                timeSpan = TimeSpan.FromMilliseconds(270);
                attackList.Remove(speer);
                attackList.Remove(holyThunder);
                launchedMelee = false;
                launchedSpecial = false;
            }
            #endregion

            #region UpdateDoppelgaenger
            if (launchedRanged)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    ////Doppelganger movement
                    //GamePadState curGamepad = GamePad.GetState(PlayerIndex.One);

                    //Vector3 moveVecDoppelPad = new Vector3(curGamepad.ThumbSticks.Right, 0);

                    //if (moveVecDoppelPad != Vector3.Zero)
                    //    doppelgaenger.move(moveVecDoppelPad);
                    //else
                    //{
                    //    KeyboardState curKeyboard = Keyboard.GetState();

                    //    Vector3 moveVecDoppelBoard = Vector3.Zero;

                    //    if (curKeyboard.IsKeyDown(Keys.Up)) moveVecDoppelBoard += new Vector3(0, speedDoppel, 0);
                    //    if (curKeyboard.IsKeyDown(Keys.Left)) moveVecDoppelBoard += new Vector3(-speedDoppel, 0, 0);
                    //    if (curKeyboard.IsKeyDown(Keys.Down)) moveVecDoppelBoard += new Vector3(0, -speedDoppel, 0);
                    //    if (curKeyboard.IsKeyDown(Keys.Right)) moveVecDoppelBoard += new Vector3(speedDoppel, 0, 0);

                    //    if (moveVecDoppelBoard.Length() > 1) moveVecDoppelBoard.Normalize();

                    //    doppelgaenger.move(moveVecDoppelBoard);
                    //}

                    doppelgaenger.move(new Vector3(speedDoppel, 0, 0) * currentViewingDirection);

                    //Doppelgaenger schnellt hervor und verschwindet
                    //nach 50 Einheiten oder wenn er mit etwas kollidiert
                    //Effekt kann noch veraendert werden
                    if (doppelgaenger.position != this.position + 50 * currentViewingDirection)
                    {
                        launchedRanged = true;
                        if (enemyHit)
                        {
                            launchedRanged = false;
                            attackList.Remove(doppelgaenger);
                        }
                        else
                        {
                            #region enemyList1
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
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
                                            enemy.hp -= 2;
                                            enemyHit = true;
                                            //Console.WriteLine("Ophelia hit enemy by RangedAttack");
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region enemyList2
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
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
                                            enemy.hp -= 2;
                                            enemyHit = true;
                                            //Console.WriteLine("Ophelia hit enemy by RangedAttack");
                                        }
                                    }
                                }
                            }
                            #endregion
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

                    Vector3 moveVecDoppelPad = new Vector3(curGamepad.ThumbSticks.Right.X, 0, -curGamepad.ThumbSticks.Right.Y);

                    if (moveVecDoppelPad != Vector3.Zero)
                        doppelgaenger.move(moveVecDoppelPad);
                    else
                    {
                        KeyboardState curKeyboard = Keyboard.GetState();

                        Vector3 moveVecDoppelBoard = Vector3.Zero;

                        if (curKeyboard.IsKeyDown(Keys.Up)) moveVecDoppelBoard += new Vector3(0, 0, -speedDoppel);
                        if (curKeyboard.IsKeyDown(Keys.Left)) moveVecDoppelBoard += new Vector3(-speedDoppel, 0, 0);
                        if (curKeyboard.IsKeyDown(Keys.Down)) moveVecDoppelBoard += new Vector3(0, 0, speedDoppel);
                        if (curKeyboard.IsKeyDown(Keys.Right)) moveVecDoppelBoard += new Vector3(speedDoppel, 0, 0);

                        if (moveVecDoppelBoard.Length() > 1) moveVecDoppelBoard.Normalize();

                        doppelgaenger.move(moveVecDoppelBoard);
                    }

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
                            if (doppelgaenger.boundary.Intersects(Arena.boss.boundary))
                            {
                                Arena.seraphinBossHUD.healthLeft -= 40;
                                enemyHit = true;
                                //Console.WriteLine("Ophelia hit Boss by RangedAttack");
                            }
                        }
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
            timeSpan = TimeSpan.FromMilliseconds(270);
            currentViewingDirection = viewingDirection;

            #region Singleplayer
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                Vector3 speerPosition = Singleplayer.player.Position + currentViewingDirection * offset;
                speer = new Entity(speerPosition, "OpheliasSpeer", Singleplayer.cont);
                speer.boundary = new BoundingBox(this.position + new Vector3(-12f, -2.5f, -2.5f) + currentViewingDirection * offset, this.position + new Vector3(12f, 2.5f, 2.5f) + currentViewingDirection * offset);
                attackList.Add(speer);

                //Kollision mit Attacke
                #region enemyList1
                foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                {
                    foreach (Entity opheliaSpeer in attackList)
                    {
                        if (opheliaSpeer.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 1;
                            Console.WriteLine("Ophelia hit enemy by MeleeAttack");
                        }
                    }
                }
                #endregion
                #region enemyList2
                foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                {
                    foreach (Entity opheliaSpeer in attackList)
                    {
                        if (opheliaSpeer.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 1;
                            Console.WriteLine("Ophelia hit enemy by MeleeAttack");
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region Arena
            else if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                Vector3 speerPosition = Arena.player.Position + currentViewingDirection * offset;
                speer = new Entity(speerPosition, "5x5x5Box1", Arena.cont);
                speer.boundary = new BoundingBox(this.position + new Vector3(-12f, -2.5f, -2.5f) + currentViewingDirection * offset, this.position + new Vector3(12f, 2.5f, 2.5f) + currentViewingDirection * offset);
                attackList.Add(speer);

                //Kollision mit Attacke
                foreach (Entity opheliaSpeer in attackList)
                {
                    if (opheliaSpeer.boundary.Intersects(Arena.boss.boundary))
                    {
                        Arena.seraphinBossHUD.healthLeft -= 5;
                        Console.WriteLine("Ophelia hit Boss by MeleeAttack");
                    }
                }
            }
            #endregion
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft > 0)
            {
                manaLeft -= 1;
                launchedRanged = true;
                currentViewingDirection = viewingDirection;

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    Vector3 doppelPos = new Vector3(this.position.X, this.Position.Y, 5);

                    doppelgaenger = new Entity(doppelPos, "Players/Ophelia", Singleplayer.cont);
                    doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -9f, -2.5f), this.position + new Vector3(2.5f, 9f, 2.5f));
                    attackList.Add(doppelgaenger);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    doppelgaenger = new Entity(this.position, "Players/Ophelia", Arena.cont);
                    doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -9f, -2.5f), this.position + new Vector3(2.5f, 9f, 2.5f));
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
                timeSpan = TimeSpan.FromMilliseconds(270);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    holyThunder = new Entity(this.Position, "Terrain/10x10x10Box1", Singleplayer.cont);
                    holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                    attackList.Add(holyThunder);

                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity opheliasHolyThunder in attackList)
                        {
                            if (opheliasHolyThunder.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 3;
                                Console.WriteLine("Ophelia hit enemy by AoE");
                            }
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        foreach (Entity opheliasHolyThunder in attackList)
                        {
                            if (opheliasHolyThunder.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 3;
                                Console.WriteLine("Ophelia hit enemy by AoE");
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    holyThunder = new Entity(this.Position, "Terrain/10x10x10Box1", Arena.cont);
                    holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -3, -20), this.position + new Vector3(20, 3, 20));
                    attackList.Add(holyThunder);

                    foreach (Entity opheliasHolyThunder in attackList)
                    {
                        if (opheliasHolyThunder.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.seraphinBossHUD.healthLeft -= 50;
                            Console.WriteLine("Ophelia hit Boss by AoE");
                        }
                    }
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
                    speer.drawIn2DWorld(new Vector3(0.02f, 0.02f, 0.02f), 0, 0, MathHelper.ToRadians(-90) * currentViewingDirection.X);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    speer.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    doppelgaenger.drawIn2DWorld(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) * currentViewingDirection.X, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    doppelgaenger.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + Arena.blickWinkel, 0);
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                    holyThunder.drawIn2DWorld(Vector3.One, 0, 0, 0);
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                    holyThunder.drawInArena(Vector3.One, 0, 0, 0);
            }
            #endregion
        }
        #endregion
    }
}
