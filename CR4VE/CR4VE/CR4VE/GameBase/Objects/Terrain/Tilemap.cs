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

namespace CR4VE.GameBase.Objects.Terrain
{
    class Tilemap
    {
        #region Attributes
        private List<Tile> Tiles = new List<Tile>();
        private List<Checkpoint> Checkpoints = new List<Checkpoint>();
        private List<Powerup> Powerups = new List<Powerup>();
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
        public List<Powerup> PowerupList
        {
            get { return Powerups; }
        }
        #endregion

        #region Constructors
        public Tilemap() { }
        #endregion

        #region Methods
        //map = Anordnung der Tiles
        //size = Groeße eines einzelnen Tiles
        public void Generate(int[,] map, int size, Vector3 start)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    //momentane Zahl in der Tilemap
                    int number = map[y,x];

                    //Wenn Zahl zwischen 1 und 95, Tile erstellen und adden
                    switch (number)
                    {
                        default:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, size / 2, size / 2));

                                //define Tile damage
                                int damage = 0;

                                if (number == 4 || number == 8 || number == 14)
                                    damage = Tile.waterDmg;
                                else if (number == 16)
                                    damage = Tile.lethalDmg;

                                //harten String noch ersetzen
                                Tiles.Add(new Tile("Box", number, position, boundary, damage));
                            } break;

                        //do nothing
                        case 0:
                            break;

                        //Checkpoint
                        case 96:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(0, 4f, 0);

                                //spaeter noch nach Leveltyp differenzieren
                                Checkpoints.Add(new Checkpoint(position, "checkpoint_hell", Singleplayer.cont));
                            } break;

                        //Health Powerup
                        case 97:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0) + new Vector3(0, 2f, 0);
                                BoundingBox healthBound = new BoundingBox(position + new Vector3(-3, -3, -3), position + new Vector3(3, 3, 3));

                                //spaeter noch nach Leveltyp differenzieren
                                Powerups.Add(new Powerup(position, "powerup_hell_health", Singleplayer.cont, healthBound, "health", 50));
                            } break;

                        //Mana Powerup
                        case 98:
                            {
                                Vector3 position = start + new Vector3(x * size, -y * size, 0);
                                BoundingBox manaBound = new BoundingBox(position + new Vector3(-3, -3, -3), position + new Vector3(3, 3, 3));

                                //spaeter noch nach Leveltyp differenzieren
                                Powerups.Add(new Powerup(position, "powerup_hell_mana", Singleplayer.cont, manaBound, "mana", 1));
                            } break;
                    }
                            
                }
            }
        }
       
        public void Draw()
        {
            List<Tile> visibles = getVisibleTiles();

            //Tiles
            foreach (Tile t in visibles)
            {
                t.Draw();
            }

            //Checkpoints
            foreach (Checkpoint c in this.Checkpoints)
            {
                c.Draw();
            }

            //Powerups
            foreach (Powerup p in this.Powerups)
            {
                p.Draw();
            }
        }


        public static List<Tile> getVisibleTiles()
        {
            List<Tile> result = new List<Tile>();

            foreach (Tile t in Singleplayer.terrainMap.TileList)
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

        #endregion
    }
}
