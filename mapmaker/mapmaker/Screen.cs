using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace mapmaker
{
    [Serializable]
    public class Screen
    {
        public Screen()
        {
            InitaliseTiles();
        }

        public void InitaliseTiles()
        {
            for (int i = 0; i < 140; i++)
            {
                Tiles.Add(new Tile(4));
            }
        }

        public string GetDataAsString(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = count; i < 14 + count; i++)
            {
                sb.Append(Tiles[i].TileNumber + ",");
            }
            return sb.ToString();
        }

        [JsonIgnore]
        public List<Tile> Tiles { get; private set; } = new List<Tile>();
    }
}
