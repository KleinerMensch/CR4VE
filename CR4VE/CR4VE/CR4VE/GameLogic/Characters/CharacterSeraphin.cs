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
    class CharacterSeraphin : Character
    {
        #region Attributes
        public static new float manaLeft = 3;
        public static List<Entity> minionList = new List<Entity>();
        public List<Entity> meleeAttackList = new List<Entity>();
        Entity algaWhip;
        BoundingSphere rangeOfLaser;

        TimeSpan timeSpanForMinions = TimeSpan.FromSeconds(10);
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(270);

        Vector3 dir;
        float distance;

        Vector3 currentViewingDirection;
        Vector3 currentCharacterPosition;
        Vector3 algaWhipPosition;
        Vector3 offset = new Vector3(8,8,8);
        Vector3 laserOffset = new Vector3(25, 25, 25);
        float currentBlickwinkel;
        float speed = 1;
        float whipRotation = 0;
        float moveSpeed = 0.2f;
        bool enemyHit = false;
        bool enemyHitByMelee = false;
        //bool enemyHitByLaser = false;
        bool listContainsAlgaWhip = false;
        //bool listContainsLaser = false;
        #endregion

        #region Properties
        public override String CharacterType
        {
            get { return "Seraphin"; }
        }
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public CharacterSeraphin() : base() { }
        public CharacterSeraphin(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public CharacterSeraphin(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
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

                launchedMelee = false;
                listContainsAlgaWhip = false;
            }
            #endregion

            #region Update Melee By Characterposition
            if (launchedMelee)
            {
                whipRotation += 1;
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    algaWhipPosition = this.Position + viewingDirection * offset;
                    algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Singleplayer.cont);
                    algaWhip.boundary = new BoundingBox(algaWhipPosition + new Vector3(-8f, -2.5f, -2.5f), algaWhipPosition + new Vector3(8f, 2.5f, 2.5f));

                    if (!listContainsAlgaWhip)
                    {
                        attackList.Add(algaWhip);
                        listContainsAlgaWhip = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(algaWhip);
                    }

                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        foreach (Entity seraphinsWhip in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (seraphinsWhip.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
                                Console.WriteLine("Seraphin hit enemy by MeleeAttack");
                            }
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        foreach (Entity seraphinsWhip in meleeAttackList)
                        {
                            if (enemyHitByMelee)
                                break;
                            if (seraphinsWhip.boundary.Intersects(enemy.boundary))
                            {
                                enemy.hp -= 1;
                                enemyHitByMelee = true;
                                Console.WriteLine("Seraphin hit enemy by MeleeAttack");
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    algaWhipPosition = this.position + viewingDirection * offset;
                    algaWhip = new Entity(algaWhipPosition, "seraphinsWhip", Arena.cont);
                    algaWhip.viewingDirection = viewingDirection;
                    algaWhip.boundary = new BoundingBox(algaWhipPosition + new Vector3(-4f, -4f, -4f), algaWhipPosition + new Vector3(4f, 4f, 4f));

                    if (!listContainsAlgaWhip)
                    {
                        meleeAttackList.Add(algaWhip);
                        listContainsAlgaWhip = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(algaWhip);
                    }

                    foreach (Entity seraphinsWhip in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        if (seraphinsWhip.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.opheliaHud.healthLeft -= 5;
                            enemyHitByMelee = true;
                            Console.WriteLine("Seraphin hit Boss by MeleeAttack");
                        }
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    algaWhipPosition = this.position + viewingDirection * offset;
                    algaWhip = new Entity(algaWhipPosition, "5x5x5Box1", Multiplayer.cont);
                    algaWhip.viewingDirection = viewingDirection;
                    algaWhip.boundary = new BoundingBox(algaWhipPosition + new Vector3(-4f, -4f, -4f), algaWhipPosition + new Vector3(4f, 4f, 4f));

                    if (!listContainsAlgaWhip)
                    {
                        meleeAttackList.Add(algaWhip);
                        listContainsAlgaWhip = true;
                    }
                    else
                    {
                        meleeAttackList.Clear();
                        meleeAttackList.Add(algaWhip);
                    }

                    foreach (Entity seraphinsWhip in meleeAttackList)
                    {
                        if (enemyHitByMelee)
                            break;
                        //if (seraphinsWhip.boundary.Intersects(Multiplayer.playerX.boundary))
                        //{
                        //    Multiplayer.playerXHud.healthLeft -= 1;
                        //    enemyHitByMelee = true;
                        //    Console.WriteLine("Seraphin hit PlayerX by MeleeAttack");
                        //}
                    }
                }
                #endregion
            }
            #endregion

            #region MinionsFromRangedAttack
            #region Removing Minions After Defined Time
            timeSpanForMinions -= time.ElapsedGameTime;
            if (timeSpanForMinions <= TimeSpan.Zero && minionList.Count > 0)
            {
                minionList.RemoveAt(0);
                // Re initializes the timespan for the next time
                // minion vanishes after 10 seconds
                timeSpanForMinions = TimeSpan.FromSeconds(10);
                launchedRanged = false;
            }
            #endregion

            foreach (Entity minion in minionList)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    Vector3 direction = new Vector3(0, 0, 0);
                    float minDistance = float.MaxValue;

                    //calculates which enemy is next to it
                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        dir = enemy.position - minion.position;
                        distance = dir.Length();
                        if (distance < minDistance)
                        {
                            direction = dir;
                            minDistance = distance;
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        dir = enemy.position - minion.position;
                        distance = dir.Length();
                        if (distance < minDistance)
                        {
                            direction = dir;
                            minDistance = distance;
                        }
                    }
                    #endregion

                    direction.Normalize();
                    direction = moveSpeed * direction;
                    minion.viewingDirection = direction;
                    minion.position += direction;

                    //checking collision
                    #region enemyList1
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                    {
                        if (minion.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 0.01f;
                            Console.WriteLine("Seraphin hit enemy by RangedAttack");
                        }
                    }
                    #endregion
                    #region enemyList2
                    foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex2].EnemyList)
                    {
                        if (minion.boundary.Intersects(enemy.boundary))
                        {
                            enemy.hp -= 0.01f;
                            Console.WriteLine("Seraphin hit enemy by RangedAttack");
                        }
                    }
                    #endregion
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    Vector3 direction = Arena.boss.position - minion.position;
                    direction.Normalize();
                    direction = moveSpeed * direction;
                    minion.viewingDirection = direction;
                    minion.move(direction);

                    if (minion.boundary.Intersects(Arena.boss.boundary))
                    {
                        Arena.opheliaHud.healthLeft -= 1;
                        Console.WriteLine("Seraphin hit enemy by RangedAttack");
                    }
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    minion.boundary = new BoundingBox(minion.position + new Vector3(-2, -2, -2), minion.position + new Vector3(2, 2, 2));

                    //Vector3 direction = Multiplayer.playerX - minion.position;
                    //direction.Normalize();
                    //direction = moveSpeed * direction;
                    //minion.viewingDirection = direction;
                    //minion.move(direction);

                    //if (minion.boundary.Intersects(Multiplayer.playerX.boundary))
                    //{
                    //    Arena.opheliaHud.healthLeft -= 1;
                    //    Console.WriteLine("Seraphin hit Multiplayer.playerX by RangedAttack");
                    //}
                }
                #endregion
            }
            #endregion

            #region UpdateLaserFromSpecialAttack
            if (launchedSpecial)
            {
                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    #region alt
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Laser schießt nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfLaser))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedSpecial = false;
                            break;
                        }
                        else
                        {
                            launchedSpecial = true;

                            #region enemyList1
                            foreach (Enemy enemy in Singleplayer.currentMaps[Singleplayer.activeIndex1].EnemyList)
                            {
                                if (attackList.Count > 0)
                                {
                                    if (attackList[i].boundary.Intersects(enemy.boundary))
                                    {
                                        enemy.hp -= 3;
                                        enemyHit = true;
                                        Console.WriteLine("Seraphin hit enemy by SpecialAttack");
                                    }
                                    //verschwindet auch bei Kollision
                                    if (enemyHit)
                                    {
                                        if (attackList.Count == 0)
                                            launchedSpecial = false;
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
                                        enemy.hp -= 3;
                                        enemyHit = true;
                                        Console.WriteLine("Seraphin hit enemy by SpecialAttack");
                                    }
                                    //verschwindet auch bei Kollision
                                    if (enemyHit)
                                    {
                                        if (attackList.Count == 0)
                                            launchedSpecial = false;
                                        attackList.Remove(attackList[i]);
                                    }
                                }
                            }
                            #endregion
                        }
                        enemyHit = false;
                    }
                    #endregion
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    #region alt
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Laser schießt nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfLaser))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedSpecial = false;
                            break;
                        }
                        else
                        {
                            launchedSpecial = true;

                            if (attackList[i].boundary.Intersects(Arena.boss.boundary))
                            {
                                Arena.opheliaHud.healthLeft -= 40;
                                enemyHit = true;
                                Console.WriteLine("Seraphin hit Boss by SpecialAttack");
                            }
                            //verschwindet auch bei Kollision
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                    launchedSpecial = false;
                                attackList.Remove(attackList[i]);
                            }

                        }
                        enemyHit = false;
                    }
                    #endregion
                    #region anderer ansatz
                    /*laserPosition = this.position;
                    laser = new Entity(laserPosition, "seraphinsLaser", Arena.cont);
                    laser.viewingDirection = this.viewingDirection;
                    laser.boundary = new BoundingBox(laserPosition + new Vector3(-50, -3f, -3f), laserPosition + new Vector3(50, -3f, -3f));

                    if (!listContainsLaser)
                    {
                        attackList.Add(laser);
                        listContainsLaser = true;
                    }
                    else
                    {
                        attackList.Clear();
                        attackList.Add(laser);
                    }

                    foreach (Entity seraphinsLaser in attackList)
                    {
                        if (enemyHitByLaser)
                            break;
                        if (seraphinsLaser.boundary.Intersects(Arena.boss.boundary))
                        {
                            Arena.opheliaHud.healthLeft -= 1;
                            enemyHitByLaser = true;
                            Console.WriteLine("Seraphin hit Boss by Laser");
                            Console.WriteLine(this.position + " " + Arena.boss.position + "" + seraphinsLaser.position);
                        }
                    }*/
                    #endregion
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    #region alt
                    for (int i = 0; i < attackList.Count; i++)
                    {
                        //Laser schießt nach vorn
                        attackList[i].move(speed * attackList[i].viewingDirection);

                        //verschwindet nach 50 Einheiten
                        if (!attackList[i].boundary.Intersects(rangeOfLaser))
                        {
                            attackList.Remove(attackList[i]);
                            if (attackList.Count == 0)
                                launchedSpecial = false;
                            break;
                        }
                        else
                        {
                            launchedSpecial = true;

                            //if (attackList[i].boundary.Intersects(Multiplayer.playerX.boundary))
                            //{
                            //    Multiplayer.playerXHud.healthLeft -= 40;
                            //    enemyHit = true;
                            //    Console.WriteLine("Seraphin hit PlayerX by SpecialAttack");
                            //}
                            //verschwindet auch bei Kollision
                            if (enemyHit)
                            {
                                if (attackList.Count == 0)
                                    launchedSpecial = false;
                                attackList.Remove(attackList[i]);
                            }

                        }
                        enemyHit = false;
                    }
                    #endregion
                    #region anderer ansatz -> bounding box leider falsch aligned
                    /*laserPosition = this.position + viewingDirection * offset;
                    laser = new Entity(laserPosition, "seraphinsLaser", Multiplayer.cont);
                    laser.viewingDirection = this.viewingDirection;
                    laser.boundary = new BoundingBox(laserPosition + new Vector3(-3, -3, -3), laserPosition + new Vector3(3, 3, 3));

                    if (!listContainsLaser)
                    {
                        attackList.Add(laser);
                        listContainsLaser = true;
                    }
                    else
                    {
                        attackList.Clear();
                        attackList.Add(laser);
                    }

                    foreach (Entity seraphinsLaser in attackList)
                    {
                        if (enemyHitByLaser)
                            break;
                        //if (seraphinsWhip.boundary.Intersects(Multiplayer.playerX.boundary))
                        //{
                        //    Multiplayer.playerXHud.healthLeft -= 1;
                        //    enemyHitByLaser = true;
                        //    Console.WriteLine("Seraphin hit PlayerX by MeleeAttack");
                        //}
                    }*/
                    #endregion
                }
                #endregion
            }
            #endregion
        }

        public override void MeleeAttack(GameTime time)
        {
            whipRotation = 0;
            launchedMelee = true;
            timeSpan = TimeSpan.FromMilliseconds(270);
            enemyHitByMelee = false;
            //Rest wird in der Update berechnet
        }

        public override void RangedAttack(GameTime time)
        {
            if (manaLeft > 0)
            {
                manaLeft -= 1;
                launchedRanged = true;
                timeSpanForMinions = TimeSpan.FromSeconds(10);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Singleplayer.cont));
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Arena.cont));
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    minionList.Add(new Entity(this.position, "Enemies/EnemyEye", CR4VE.GameLogic.GameStates.Multiplayer.cont));
                }
                #endregion

                //max. 3 minions can be spawned
                if (minionList.Count > 3)
                    minionList.RemoveAt(0);
            }
        }

        public override void SpecialAttack(GameTime time)
        {
            //Laser kann nur bei vollem Mana benutzt werden
            if (manaLeft == 3)
            {
                manaLeft -= 3;
                launchedSpecial = true;
                currentCharacterPosition = this.Position;
                //timeSpan = TimeSpan.FromMilliseconds(270);
                rangeOfLaser = new BoundingSphere(currentCharacterPosition, 50);

                #region Singleplayer
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    Entity laser = new Entity(this.position, "seraphinsLaser", Singleplayer.cont);
                    laser.viewingDirection = this.viewingDirection;
                    laser.boundary = new BoundingBox(this.position + new Vector3(-3, -3, -3), this.position + new Vector3(3, 3, 3));
                    attackList.Add(laser);
                }
                #endregion
                #region Arena
                else if (Game1.currentState.Equals(Game1.EGameState.Arena))
                {
                    Entity laser = new Entity(this.position, "seraphinsLaser", Arena.cont);
                    laser.viewingDirection = this.viewingDirection;
                    laser.boundary = new BoundingBox(this.position + new Vector3(-5, -6, -5), this.position + new Vector3(5, 6, 5));
                    attackList.Add(laser);
                }
                #endregion
                #region Multiplayer
                else if (Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    Entity laser = new Entity(this.position, "seraphinsLaser", Multiplayer.cont);
                    laser.viewingDirection = this.viewingDirection;
                    laser.boundary = new BoundingBox(this.position + new Vector3(-5, -6, -5), this.position + new Vector3(5, 6, 5));
                    attackList.Add(laser);
                }
                #endregion

                //Rest wird in der Update berechnet
            }
        }

        public override void DrawAttacks()
        {
            #region DrawMelee
            if (launchedMelee)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity whip in meleeAttackList)
                    {
                        whip.drawIn2DWorld(new Vector3(1, 1, 1), 0, 0, MathHelper.ToRadians(-90) * viewingDirection.X);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity whip in meleeAttackList)
                    {
                        float whipBlickwinkel = (float)Math.Atan2(-whip.viewingDirection.Z, whip.viewingDirection.X);
                        whip.drawInArena(new Vector3(1, 1, 1), 0, MathHelper.ToRadians(90) + whipBlickwinkel, 0);
                    }
                }
            }
            #endregion
            #region DrawRanged
            if (launchedRanged)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity minion in minionList)
                    {
                        float minionBlickwinkel = (float)Math.Atan2(-minion.viewingDirection.Y, minion.viewingDirection.X);
                        minion.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, MathHelper.ToRadians(90) + minionBlickwinkel, 0);
                    }
                }
                else if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity minion in minionList)
                    {
                        float minionBlickwinkel = (float)Math.Atan2(-minion.viewingDirection.Z, minion.viewingDirection.X);
                        minion.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, MathHelper.ToRadians(90) + minionBlickwinkel, 0);
                    }
                }
            }
            #endregion
            #region DrawSpecial
            if (launchedSpecial)
            {
                if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
                {
                    foreach (Entity lazer in attackList)
                    {
                        lazer.drawIn2DWorld(new Vector3(0.01f, 0.01f, 0.01f), 0, MathHelper.ToRadians(90) * lazer.viewingDirection.X, 0);
                    }
                }
                if (Game1.currentState.Equals(Game1.EGameState.Arena) || Game1.currentState.Equals(Game1.EGameState.Multiplayer))
                {
                    foreach (Entity lazer in attackList)
                    {
                        float laserBlickwinkel = (float)Math.Atan2(-lazer.viewingDirection.Z, lazer.viewingDirection.X);
                        lazer.drawInArena(new Vector3(1f, 1f, 1f), 0, MathHelper.ToRadians(90) + laserBlickwinkel, 0);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
