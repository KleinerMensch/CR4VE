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
        public List<Entity> meleeAttackList = new List<Entity>();
        public List<Entity> aoeList = new List<Entity>();
        Entity speer, holyThunder;
        BoundingSphere rangeOfDoppelgaenger;

        Vector3 currentCharacterPosition;
        Vector3 speerPosition;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);

        Vector3 offset = new Vector3(3, 3, 3);
        float speedDoppel = 1;
        bool enemyHit = false;
        bool enemyHitByMelee = false;
        bool listContainsSpeer = false;
        float speerRotation = 0;
        #endregion

        #region Properties
        public override String CharacterType
        {
            get { return "Ophelia"; }
        }
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
            timeSpan -= time.ElapsedGameTime;
            if (timeSpan <= TimeSpan.Zero)
            {
                timeSpan = TimeSpan.FromMilliseconds(270);
                meleeAttackList.Clear();
                aoeList.Clear();

                launchedMelee = false;
                launchedSpecial = false;
                listContainsSpeer = false;
            }
            #endregion

            #region Update Melee By Characterposition
            speerRotation += 0.15f;
            if (launchedMelee)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    speerPosition = this.Position + viewingDirection * offset;
                    speer = new Entity(speerPosition, "OpheliasSpeer", Singleplayer.cont);
                    speer.boundary = new BoundingBox(speerPosition + new Vector3(-9f, -6f, -4f), speerPosition + new Vector3(9f, 6f, 4f));
                    
                    if (!listContainsSpeer)
                    {
                        meleeAttackList.Add(speer);
                        listContainsSpeer = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(speer);
                    }

                    //Kollision mit Attacke
                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity opheliaSpeer in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (opheliaSpeer.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
                                Console.WriteLine("Ophelia hit enemy by MeleeAttack");
                            }
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        foreach (Entity opheliaSpeer in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (opheliaSpeer.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
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
                    speerPosition = this.Position + viewingDirection * offset;
                    speer = new Entity(speerPosition, "OpheliasSpeer", Arena.cont);
                    speer.boundary = new BoundingBox(speerPosition + new Vector3(-9f, -6f, -2.5f), speerPosition + new Vector3(9f, 6f, 2.5f));

                    if (!listContainsSpeer)
                    {
                        meleeAttackList.Add(speer);
                        listContainsSpeer = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(speer);
                    }

                    //Kollision mit Attacke
                    foreach (Entity opheliaSpeer in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        if (opheliaSpeer.boundary.Intersects(Arena.boss.boundary))
                        {
                            Console.WriteLine("Ophelia hit Boss By Melee");
                            Arena.seraphinBossHUD.healthLeft -= 5;
                            enemyHitByMelee = true;
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    speerPosition = this.Position + viewingDirection * offset;
                    speer = new Entity(speerPosition, "OpheliasSpeer", Multiplayer.cont);
                    speer.boundary = new BoundingBox(speerPosition + new Vector3(-6f, -6f, -4f), speerPosition + new Vector3(10f, 6f, 4f));

                    if (!listContainsSpeer)
                    {
                        meleeAttackList.Add(speer);
                        listContainsSpeer = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(speer);
                    }

                    //Kollision mit Attacke
                    foreach (Entity opheliaSpeer in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        //if (opheliaSpeer.boundary.Intersects(Multiplayer.playerX.boundary))
                        //{
                        //    Console.WriteLine("Ophelia hit Boss By Melee");
                        //    Multiplayer.playerXHUD.healthLeft -= 5;
                        //    enemyHitByMelee = true;
                        //}
                    }
                }
                #endregion
            }
            #endregion

            #region UpdateDoppelgaenger
            if (launchedRanged)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        #region Doppelganger movement von Flo
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
                        #endregion
                        //Doppelgaenger schnellt hervor
                        attackList[i].move(speedDoppel * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfDoppelgaenger))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedRanged = false;
                            break;
                        }
                        else
                        {
                            launchedRanged = true;

                            #region enemyList1
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                            {
                                if (attackList.Count > 0)
                                {
                                    if (attackList[i].boundary.Intersects(enemy.boundary))
                                    {
                                        enemy.hp -= 2;
                                        enemyHit = true;
                                        Console.WriteLine("Ophelia hit enemy by RangedAttack");
                                    }
                                    if (enemyHit)
                                    {
                                        if (attackList.Count == 0)
                                            launchedRanged = false;
                                        attackList.Remove(attackList[i]);
                                    }
                                }
                            }
                            #endregion
                            #region enemyList2
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                            {
                                if (attackList.Count > 0)
                                {
                                    if (attackList[i].boundary.Intersects(enemy.boundary))
                                    {
                                        enemy.hp -= 2;
                                        enemyHit = true;
                                        Console.WriteLine("Ophelia hit enemy by RangedAttack");
                                    }
                                    if (enemyHit)
                                    {
                                        if (attackList.Count == 0)
                                            launchedRanged = false;
                                        attackList.Remove(attackList[i]);
                                    }
                                }
                            }
                            #endregion

                        }
                        enemyHit = false;
                    }
                }
                #endregion
                #region Arena
                if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        #region Doppelganger movement von Flo
                        //GamePadState curGamepad = GamePad.GetState(PlayerIndex.One);

                        //Vector3 moveVecDoppelPad = new Vector3(curGamepad.ThumbSticks.Right.X, 0, -curGamepad.ThumbSticks.Right.Y);

                        //if (moveVecDoppelPad != Vector3.Zero)
                        //    doppelgaenger.move(moveVecDoppelPad);
                        //else
                        //{
                        //    KeyboardState curKeyboard = Keyboard.GetState();

                        //    Vector3 moveVecDoppelBoard = Vector3.Zero;

                        //    if (curKeyboard.IsKeyDown(Keys.Up)) moveVecDoppelBoard += new Vector3(0, 0, -speedDoppel);
                        //    if (curKeyboard.IsKeyDown(Keys.Left)) moveVecDoppelBoard += new Vector3(-speedDoppel, 0, 0);
                        //    if (curKeyboard.IsKeyDown(Keys.Down)) moveVecDoppelBoard += new Vector3(0, 0, speedDoppel);
                        //    if (curKeyboard.IsKeyDown(Keys.Right)) moveVecDoppelBoard += new Vector3(speedDoppel, 0, 0);

                        //    if (moveVecDoppelBoard.Length() > 1) moveVecDoppelBoard.Normalize();

                        //    doppelgaenger.move(moveVecDoppelBoard);
                        //}
                        #endregion
                        //Doppelgaenger schnellt hervor
                        attackList[i].move(speedDoppel * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfDoppelgaenger))
                        {
                            attackList.Remove(attackList[i]);
                            if(attackList.Count == 0)
                                launchedRanged = false;
                            break;
                        }
                        else
                        {
                            launchedRanged = true;

                            if (attackList[i].boundary.Intersects(Arena.boss.boundary))
                            {
                                Arena.seraphinBossHUD.healthLeft -= 40;
                                enemyHit = true;
                                Console.WriteLine("Ophelia hit Boss by RangedAttack");
                            }
                            //verschwindet auch bei Kollision mit Gegner
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                    launchedRanged = false;
                                attackList.Remove(attackList[i]);
                            }
                        }
                        enemyHit = false;
                    }
                }
                #endregion
                #region Multiplayer
                if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Doppelgaenger schnellt hervor
                        attackList[i].move(speedDoppel * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfDoppelgaenger))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedRanged = false;
                            break;
                        }
                        else
                        {
                            launchedRanged = true;

                            //if (attackList[i].boundary.Intersects(Multiplayer.playerX.boundary))
                            //{
                            //    Multiplayer.playerXHUD.healthLeft -= 40;
                            //    enemyHit = true;
                            //    Console.WriteLine("Ophelia hit PlayerX by RangedAttack");
                            //}
                            //verschwindet auch bei Kollision mit Gegner
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                    launchedRanged = false;
                                attackList.Remove(attackList[i]);
                            }
                        }
                        enemyHit = false;
                    }
                }
                #endregion
            }
            #endregion
        }

        public override void MeleeAttack(GameTime time)
        {
            launchedMelee = true;
            timeSpan = TimeSpan.FromMilliseconds(270);
            enemyHitByMelee = false;
            speerRotation = 0;
            //Rest wird in der Update berechnet
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft > 0)
            {
                manaLeft -= 1;
                launchedRanged = true;
                currentCharacterPosition = this.Position;
                rangeOfDoppelgaenger = new BoundingSphere(currentCharacterPosition, 50);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    Entity doppelgaenger = new Entity(this.Position, "Players/Ophelia", Singleplayer.cont);
                    doppelgaenger.viewingDirection = viewingDirection;
                    doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-2.5f, -9f, -2.5f), this.position + new Vector3(2.5f, 9f, 2.5f));
                    attackList.Add(doppelgaenger);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    Entity doppelgaenger = new Entity(this.position, "Players/Ophelia", Arena.cont);
                    doppelgaenger.viewingDirection = viewingDirection;
                    doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-1f, -9f, -6f), this.position + new Vector3(1f, 9f, 6f));
                    attackList.Add(doppelgaenger);
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    Entity doppelgaenger = new Entity(this.position, "Players/Ophelia", Multiplayer.cont);
                    doppelgaenger.viewingDirection = viewingDirection;
                    doppelgaenger.boundary = new BoundingBox(this.position + new Vector3(-1f, -9f, -6f), this.position + new Vector3(1f, 9f, 6f));
                    attackList.Add(doppelgaenger);
                }
                #endregion

                //Rest wird in der Update berechnet
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
                    holyThunder = new Entity(this.Position, "OpheliasAoE", Singleplayer.cont);
                    holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -10, -20), this.position + new Vector3(20, 40, 20));
                    aoeList.Add(holyThunder);

                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity opheliasHolyThunder in aoeList)
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
                        foreach (Entity opheliasHolyThunder in aoeList)
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
                    holyThunder = new Entity(this.Position, "OpheliasAoE", Arena.cont);
                    holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -10, -20), this.position + new Vector3(20, 60, 20));
                    aoeList.Add(holyThunder);

                    foreach (Entity opheliasHolyThunder in aoeList)
                    {
                        if (opheliasHolyThunder.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.seraphinBossHUD.healthLeft -= 50;
                            Console.WriteLine("Ophelia hit Boss by AoE");
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    holyThunder = new Entity(this.Position, "OpheliasAoE", Multiplayer.cont);
                    holyThunder.boundary = new BoundingBox(this.position + new Vector3(-20, -10, -20), this.position + new Vector3(20, 60, 20));
                    aoeList.Add(holyThunder);

                    foreach (Entity opheliasHolyThunder in aoeList)
                    {
                        //if (opheliasHolyThunder.boundary.Intersects(Multiplayer.playerX.boundary))
                        //{
                        //    Multiplayer.playerXHUD.healthLeft -= 50;
                        //    Console.WriteLine("Ophelia hit PlayerX by AoE");
                        //}
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
                {
                    foreach (Entity opheliaSpeer in meleeAttackList)
                    {
                        opheliaSpeer.drawIn2DWorld(new Vector3(0.02f, 0.02f, 0.02f), 0, 0, MathHelper.ToRadians(-45) * viewingDirection.X * speerRotation);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity opheliaSpeer in meleeAttackList)
                    {
                        float speerBlickwinkelXZ = (float)Math.Atan2(-viewingDirection.Z, viewingDirection.X);
                        opheliaSpeer.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + speerBlickwinkelXZ, MathHelper.ToRadians(-45) * speerRotation*viewingDirection.X);
                    }
                }
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity doppelOphelia in attackList)
                    {
                        doppelOphelia.drawIn2DWorld(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) * doppelOphelia.viewingDirection.X, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity doppelOphelia in attackList)
                    {
                        float doppelBlickwinkel = (float)Math.Atan2(-doppelOphelia.viewingDirection.Z, doppelOphelia.viewingDirection.X);
                        doppelOphelia.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + doppelBlickwinkel, 0);
                    }
                }
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity thunder in aoeList)
                    {
                        holyThunder.drawIn2DWorld(Vector3.One, 0, 0, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity thunder in aoeList)
                    {
                        holyThunder.drawInArena(Vector3.One, 0, 0, 0);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
