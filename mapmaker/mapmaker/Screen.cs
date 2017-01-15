using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace mapmaker
{
    [Serializable]
    class Screen
    {
        private int mScreen;
        private List<Row> mRows = new List<Row>();
        private List<Tile> mTiles = new List<Tile>();

        public Screen(int screenCount)
        {
            mScreen = screenCount;
            InitialiseRows();
        }

        public void InitialiseRows()
        {
            for (int i = 0; i < 140; i++)
            {
                mTiles.Add(new Tile("4"));
            }
            for (int i = 0; i < 10; i++)
            {
                mRows.Add(new Row());
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

        [JsonIgnoreAttribute]
        public int UndergroudScreen
        {
            get { return mScreen; }
        }

        public List<Row> Rows
        {
            get { return mRows; }
            set { mRows = value; }
        }

        public List<Tile> Tiles
        {
            get { return mTiles; }
            set { mTiles = value; }
        }

    }
}
