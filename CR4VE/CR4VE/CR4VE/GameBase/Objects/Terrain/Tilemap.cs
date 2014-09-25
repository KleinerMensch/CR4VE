using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameLogic.GameStates;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameBase.Objects.Terrain;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.AI;

namespace CR4VE.GameBase.Objects.Terrain
{
    class Tilemap
    {
        #region Attributes
        private List<Tile> Tiles = new List<Tile>();
        private List<Checkpoint> Checkpoints = new List<Checkpoint>();
        private List<Enemy> Enemies = new List<Enemy>();
        private List<Powerup> Powerups = new List<Powerup>();

        private Vector3 startPos;

        #endregion

        #region Properties
        public List<Tile> TileList
        {
            get { return Tiles; }
        }
        public List<Checkpoint> CheckpointList
        {
            get { return Checkpoints; }
        }
        public List<Enemy> EnemyList
        {
            get { return Enemies; }
        }
        public List<Powerup> PowerupList
        {
            get { return Powerups; }
        }
        public Vector3 StartPosition
        {
            get { return startPos; }
        }
        #endregion

        #region Constructors
        public Tilemap() { }
        public Tilemap(List<Tile> tiles, List<Checkpoint> checkpoints, List<Enemy> enemies, List<Powerup> powerups, Vector3 start)
        {
            this.Tiles = tiles;
            this.Checkpoints = checkpoints;
            this.Enemies = enemies;
            this.Powerups = powerups;

            this.startPos = start;
        }
        #endregion

        #region Methods
        //map = Anordnung der Tiles
        //size = Groeße eines einzelnen Tiles
        public static Tilemap Generate(int[,] map, int size, Vector3 start)
        {
            List<Tile> tiles = new List<Tile>();
            List<Checkpoint> checkpoints = new List<Checkpoint>();
            List<Enemy> enemies = new List<Enemy>();
            List<Powerup> powerups = new List<Powerup>();

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    //momentane Zahl in der Tilemap
                    int number = map[y, x];

                    //Wenn Zahl zwischen 1 und 95, Tile erstellen und adden
                    switch (number)
                    {
                        #region Terrain Tiles
                        default:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, size / 2, size / 2));

                                //define Tile type
                                String type = "default";

                                if (number == 4 || number == 8 || number == 14)
                                {
                                    type = "water";
                                    boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, -size / 2 + 1, size / 2));
                                }
                                else if (number == 16)
                                {
                                    type = "lava";
                                    boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, size / 4, size / 2));
                                }

                                tiles.Add(new Tile("Box", number, position, boundary, type));
                            } break;

                        //do nothing
                        case 0:
                            break;
                        #endregion

                        #region Keys
                        //SoundTrigger (Gold)
                        case 87:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size*11 / 2, -size*7 / 2, -size / 2), position + new Vector3(size*13 / 2, size*2 / 2, size / 2));

                                Singleplayer.GoldSoundTrigger = new Entity(position, "5x5x5Box1", Singleplayer.cont, boundary);
                            } break;

                        //ArenaKey
                        case 88:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, size / 2, size / 2));
                                
                                Singleplayer.ArenaKey = new Entity(position, "OpheliasSpeer", Singleplayer.cont, boundary);
                            }
                            break;

                        //Singleplayerkey
                        case 89:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, size / 2, size / 2));

                                Singleplayer.TutorialExit = new Entity(position, "OpheliasSpeer", Singleplayer.cont, boundary);
                            } break;
                        #endregion

                        #region Enemies
                         //ice spikes
                        case 91:
                                {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(3, -size/2, 5);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, 0, -size / 2), position + new Vector3(size / 2, size / 8, size / 2));
                                
                                tiles.Add(new GroundSpikes("ceiling", position, boundary, Tile.lethalDmg));
                            } break;

                        //EnemyRedEye
                        case 92:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-3,-3,-3), position + new Vector3(3,3,3));

                                enemies.Add(new EnemyRedEye(position, "EnemyEye", Singleplayer.cont, boundary));
                            } break;

                        //EnemySkull
                        case 93:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-3,-3,-3), position + new Vector3(3,3,3));

                                enemies.Add(new EnemySkull(position, "skull", Singleplayer.cont, boundary));
                            } break;

                        //ceiling spikes
                        case 94:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(0, size/2, size/2);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2.5f, -size / 2), position + new Vector3(size / 2, 0, size / 2));
                                
                                enemies.Add(new Spikes(position, "spikes_ceiling", Singleplayer.cont, boundary));
                            } break;

                        //ground spikes
                        case 95:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(3, -size/2, 5);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, 0, -size / 2), position + new Vector3(size / 2, size / 8, size / 2));
                                
                                tiles.Add(new GroundSpikes("ground", position, boundary, Tile.lethalDmg));
                            } break;

                        //hands
                        case 99:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2.5f, -size / 2), position + new Vector3(size / 2, 0, size / 2));
                                
                                enemies.Add(new Hands(position, "spikes_ceiling", Singleplayer.cont, boundary));
                            } break;

                        #endregion

                        #region Checkpoints & Powerups
                        //Checkpoint
                        case 96:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(0, 4f, 0);

                                if (Singleplayer.isCrystal)
                                    checkpoints.Add(new Checkpoint(position, "checkpoint_crystal", Singleplayer.cont));
                                else
                                    checkpoints.Add(new Checkpoint(position, "checkpoint_hell", Singleplayer.cont));
                            } break;

                        //Health Powerup
                        case 97:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(0, 2f, 0);
                                BoundingBox healthBound = new BoundingBox(position + new Vector3(-3, -3, -3), position + new Vector3(3, 3, 3));

                                powerups.Add(new Powerup(position, "powerup_hell_health", Singleplayer.cont, healthBound, "health", 150));
                            } break;

                        //Mana Powerup
                        case 98:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox manaBound = new BoundingBox(position + new Vector3(-3, -3, -3), position + new Vector3(3, 3, 3));

                                if (Singleplayer.isCrystal)
                                    powerups.Add(new Powerup(position, "powerup_crystal_mana", Singleplayer.cont, manaBound, "mana", 1));
                                else
                                    powerups.Add(new Powerup(position, "powerup_hell_mana", Singleplayer.cont, manaBound, "mana", 1));
                            } break;
                        #endregion
                    }

                }
            }

            return new Tilemap(tiles, checkpoints, enemies, powerups, start);
        }

        public void Draw(List<Tile> visibles)
        {
            //Tiles
            foreach (Tile t in visibles)
            {
                t.Draw();
            }

            //Checkpoints
            foreach (Checkpoint c in this.CheckpointList)
            {
                c.Draw();
            }

            //Powerups
            foreach (Powerup p in this.PowerupList)
            {
                p.Draw();
            }

            //Enemies
            foreach (Enemy e in this.EnemyList)
            {
                e.Draw();
            }
        }

        public static List<Tile> getVisibleTiles(Tilemap[] currentMaps)
        {
            int i1 = Singleplayer.activeIndex1;
            int i2 = Singleplayer.activeIndex2;

            List<Tile> result = new List<Tile>();


            foreach (Tile t in currentMaps[i1].TileList)
            {
                //slightly larger Frustum than Camera2D.BoundingFrustum to prevent clipping errors
                Matrix clippView = Matrix.CreateLookAt(Camera2D.FrustumPosition + new Vector3(0, 0, 50), Camera2D.FrustumTarget, Vector3.Up);

                BoundingFrustum clippingFrus = new BoundingFrustum(clippView * Camera2D.ProjectionMatrix);

                if (clippingFrus.Intersects(t.Boundary))
                {
                    result.Add(t);
                }
            }
            foreach (Tile t in currentMaps[i2].TileList)
            {
                //slightly larger Frustum than Camera2D.BoundingFrustum to prevent clipping errors
                Matrix clippView = Matrix.CreateLookAt(Camera2D.FrustumPosition + new Vector3(0, 0, 50), Camera2D.FrustumTarget, Vector3.Up);

                BoundingFrustum clippingFrus = new BoundingFrustum(clippView * Camera2D.ProjectionMatrix);

                if (clippingFrus.Intersects(t.Boundary))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public static void updateActiveTilemaps(Tilemap[] currentMaps)
        {
            float switchRange = 150f;

            float deltaXRight;
            float deltaXLeft;

            //get distance to switch point
            if (GameControls.isGhost)
            {
                deltaXRight = Singleplayer.ghost.Position.X - currentMaps[Singleplayer.activeIndex2].StartPosition.X;
                deltaXLeft = Singleplayer.ghost.Position.X - currentMaps[Singleplayer.activeIndex1].StartPosition.X;
            }
            else
            {
                deltaXRight = Singleplayer.player.Position.X - currentMaps[Singleplayer.activeIndex2].StartPosition.X;
                deltaXLeft = Singleplayer.player.Position.X - currentMaps[Singleplayer.activeIndex1].StartPosition.X;
            }

            //change indices if necessary
            if (deltaXRight > switchRange)
            {
                Singleplayer.activeIndex1 = (int)MathHelper.Clamp(Singleplayer.activeIndex1 + 1, 0, currentMaps.Length - 2);
                Singleplayer.activeIndex2 = (int)MathHelper.Clamp(Singleplayer.activeIndex2 + 1, 1, currentMaps.Length - 1);
            }

            if (deltaXLeft < switchRange)
            {
                Singleplayer.activeIndex1 = (int)MathHelper.Clamp(Singleplayer.activeIndex1 - 1, 0, currentMaps.Length - 2);
                Singleplayer.activeIndex2 = (int)MathHelper.Clamp(Singleplayer.activeIndex2 - 1, 1, currentMaps.Length - 1);
            }
        }
        #endregion
    }
}
