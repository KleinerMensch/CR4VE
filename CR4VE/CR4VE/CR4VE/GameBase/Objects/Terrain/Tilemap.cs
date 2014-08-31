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
                        default:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, size / 2, size / 2));

                                //define Tile damage
                                int damage = 0;

                                if (number == 4)// || number == 8 || number == 14)
                                {
                                    damage = Tile.waterDmg;
                                    boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, -size/2, size / 2));
                                }
                            //    else if (number == 16)
                              //      damage = Tile.lethalDmg;

                                //harten String noch ersetzen
                                tiles.Add(new Tile("Box", number, position, boundary, damage));
                            } break;

                        //do nothing
                        case 0:
                            break;

                        //ceiling spikes
                        case 94:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
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

                        //Checkpoint
                        case 96:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(0, 4f, 0);

                                //spaeter noch nach Leveltyp differenzieren
                                checkpoints.Add(new Checkpoint(position, "checkpoint_hell", Singleplayer.cont));
                            } break;

                        //Health Powerup
                        case 97:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(0, 2f, 0);
                                BoundingBox healthBound = new BoundingBox(position + new Vector3(-3, -3, -3), position + new Vector3(3, 3, 3));

                                //spaeter noch nach Leveltyp differenzieren
                                powerups.Add(new Powerup(position, "powerup_hell_health", Singleplayer.cont, healthBound, "health", 50));
                            } break;

                        //Mana Powerup
                        case 98:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox manaBound = new BoundingBox(position + new Vector3(-3, -3, -3), position + new Vector3(3, 3, 3));

                                //spaeter noch nach Leveltyp differenzieren
                                powerups.Add(new Powerup(position, "powerup_hell_mana", Singleplayer.cont, manaBound, "mana", 1));
                            } break;
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
        }

        public static List<Tile> getVisibleTiles()
        {
            int i1 = Singleplayer.activeIndex1;
            int i2 = Singleplayer.activeIndex2;

            List<Tile> result = new List<Tile>();

            foreach (Tile t in Singleplayer.tileMaps[i1].TileList)
            {
                //slightly larger Frustum than Camera2D.BoundingFrustum to prevent clipping errors
                Matrix clippView = Matrix.CreateLookAt(Camera2D.FrustumPosition + new Vector3(0, 0, 50), Camera2D.FrustumTarget, Vector3.Up);

                BoundingFrustum clippingFrus = new BoundingFrustum(clippView * Camera2D.ProjectionMatrix);

                if (clippingFrus.Intersects(t.Boundary))
                {
                    result.Add(t);
                }
            }
            foreach (Tile t in Singleplayer.tileMaps[i2].TileList)
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

        public static void updateActiveTilemaps()
        {
            float switchRange = 150f;

            float deltaXRight;
            float deltaXLeft;

            //get distance to switch point
            if (GameControls.isGhost)
            {
                deltaXRight = Singleplayer.ghost.Position.X - Singleplayer.tileMaps[Singleplayer.activeIndex2].StartPosition.X;
                deltaXLeft = Singleplayer.ghost.Position.X - Singleplayer.tileMaps[Singleplayer.activeIndex1].StartPosition.X;
            }
            else
            {
                deltaXRight = Singleplayer.player.Position.X - Singleplayer.tileMaps[Singleplayer.activeIndex2].StartPosition.X;
                deltaXLeft = Singleplayer.player.Position.X - Singleplayer.tileMaps[Singleplayer.activeIndex1].StartPosition.X;
            }

            //change indices if necessary
            if (deltaXRight > switchRange)
            {
                Singleplayer.activeIndex1 = (int) MathHelper.Clamp(Singleplayer.activeIndex1 + 1, 0, Singleplayer.tileMaps.Length - 2);
                Singleplayer.activeIndex2 = (int) MathHelper.Clamp(Singleplayer.activeIndex2 + 1, 1, Singleplayer.tileMaps.Length-1);
            }

            if (deltaXLeft < switchRange)
            {
                Singleplayer.activeIndex1 = (int) MathHelper.Clamp(Singleplayer.activeIndex1 - 1, 0, Singleplayer.tileMaps.Length - 2);
                Singleplayer.activeIndex2 = (int) MathHelper.Clamp(Singleplayer.activeIndex2 - 1, 1, Singleplayer.tileMaps.Length-1);
            }
        }

        #endregion
    }
}
