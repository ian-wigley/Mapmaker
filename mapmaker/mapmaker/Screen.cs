using System;
using System.Collections.Generic;

namespace mapmaker
{
    [Serializable]
    class Screen
    {
        private int mScreen;
        private List<Tile> mTiles = new List<Tile>();
        
        public Screen(int screenCount)
        {
            mScreen = screenCount;
            InitaliseTiles();
        }

        public void InitaliseTiles()
        {
            for (int i = 0; i < 140; i++)
            {
                mTiles.Add(new Tile("4"));
            }
        }

        public void addTile(Tile toAdd)
        {
            mTiles.Add(toAdd);
        }

        public string GetDataAsString(int count)
        {
            string asciis = "";
            for (int i = count; i < 14 + count; i++)
            {
                asciis = asciis + this.Tiles[i].AsciiCode + ",";
            }
            return asciis;
        }

        public int UndergroudScreen
        {
            get { return mScreen; }
        }

        public List<Tile> Tiles
        {
            get { return mTiles; }
            set { mTiles = value; }
        }
    }
}
