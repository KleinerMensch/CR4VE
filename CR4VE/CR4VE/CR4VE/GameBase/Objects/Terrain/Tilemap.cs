using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameLogic.GameStates;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects.Terrain;

namespace CR4VE.GameBase.Objects.Terrain
{
    class Tilemap
    {
        #region Attributes
        private List<Tile> Tiles = new List<Tile>();
        #endregion

        #region Properties
        public List<Tile> TilesList
        {
            get { return Tiles; }
        }
        #endregion

        #region Constructors
        public Tilemap() { }
        #endregion

        #region Methods
        //map = Anordnung der Tiles
        //size = Groeße eines einzelnen Tiles
        public void Generate(int[,] map, int size)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    //momentane Zahl in der Tilemap
                    int number = map[y,x];

                    //Wenn Zahl ungleich 0, Tile erstellen und adden
                    if (number != 0)
                    {
                        Vector3 position = new Vector3(x*size, -y*size, 0);
                        BoundingBox boundary = new BoundingBox(position + new Vector3(-size / 2, -size / 2, -size / 2), position + new Vector3(size / 2, size / 2, size / 2));
                        
                        //harten String noch ersetzen
                        Tiles.Add(new Tile("10x10x10Box", number, position, boundary));
                    }
                }
            }
        }
       
        public void Draw(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            List<Tile> visibles = getVisibleTiles();

            foreach (Tile t in visibles)
            {
                t.drawIn2DWorldWithoutBones(scale, rotX, rotY, rotZ);
            }
        }


        public static List<Tile> getVisibleTiles()
        {
            List<Tile> result = new List<Tile>();

            foreach (Tile t in Singleplayer.terrainMap.TilesList)
            {
                if (Camera2D.BoundFrustum.Intersects(t.Boundary))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        #endregion
    }
}
