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
        // list von coltiles
        private List<ColTiles> colTiles = new List<ColTiles>();

        #region property  region
        public List<ColTiles> ColTiles
        {
            get { return colTiles; }
        }
        private int width, height;
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        #endregion

        #region constructor region
        public Tilemap() { }

        public void Generate(int[,] map, int size) // size von block
        {
            for (int x = 0; x < map.GetLength(1); x++)
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    if (number > 0)
                        colTiles.Add(new ColTiles(number, new Rectangle(x * size, y * size, size, size))); // position von rectangle(size)

                    //x+1/y+1 da wir bei 0 starten
                    width = (x + 1) * size;
                    height = (y + 1) * size;
                }
        #endregion

        }

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ColTiles tile in colTiles)
                tile.Draw(spriteBatch);
        }
        #endregion
    }
}
