using System.Collections.Generic;

namespace mapmaker
{
    class Row
    {
        private List<Tile> mTiles = new List<Tile>();

        public Row()
        {
            InitialiseTiles();
        }

        public void InitialiseTiles()
        {
            for (int i = 0; i < 14; i++)
            {
                mTiles.Add(new Tile("4"));
            }
        }

        public List<Tile> Tiles
        {
            get { return mTiles; }
            set { mTiles = value; }
        }
    }
}
