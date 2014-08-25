using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.Terrain
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
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    //Zahl an Stelle (x,y) in der Tilemap
                    int number = map[y,x];

                    //Wenn ungleich 0, Tile erstellen und adden
                    if (number != 0)
                    {
                        Vector3 position = new Vector3(x*size, -y*size, 0);
                        BoundingBox bound = new BoundingBox(position + new Vector3(-size, 0, 0), position + new Vector3(0, size, size));

                        Tiles.Add(new Tile("4x4x4Qube", number, position, bound));
                    }
                }
            }
        }
       

        public void Draw(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            foreach (Tile t in Tiles)
            {
                t.drawIn2DWorld(scale, rotX, rotY, rotZ);
            }
        }
        #endregion
    }
}
