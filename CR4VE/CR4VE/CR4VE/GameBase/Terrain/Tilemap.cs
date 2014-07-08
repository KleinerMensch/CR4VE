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
        private List<ColTiles> colTiles = new List<ColTiles>();
        bool test = true;
        #endregion

        #region Properties
        public List<ColTiles> ColTiles
        {
            get { return colTiles; }
        }
        #endregion

        #region Constructors
        public Tilemap() { }
        #endregion

        #region Methods
        //(laeuft, ist aber noch verbesserungswuerdig: Implementierung zu verschachtelt)
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

                    //Wenn ungleich 0, colTile erstellen und adden
                    if (number != 0)
                        colTiles.Add(new ColTiles("Box", number, new Vector3(x*size, -y*size, 0)));
                }
            }
        }
       

        public void Draw(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            foreach (ColTiles cT in colTiles)
            {
                cT.drawIn2DWorld(scale, rotX, rotY, rotZ);
                if (test)
                    Console.WriteLine(cT.position);
            }
            test = false;
        }
        #endregion
    }
}
